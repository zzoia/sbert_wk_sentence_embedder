from transformers import BertModel, BertTokenizer
from sklearn.metrics.pairwise import cosine_similarity
from scipy.special import softmax
from tqdm.auto import tqdm
import numpy as np
import torch
import typing
import math
from .TokenizerWrapper import TokenizerWrapper


class ModelAndTokenizer:

    def __init__(self, model: BertModel, tokenizer: TokenizerWrapper, device=None):
        self.device = torch.device(device if device is not None else "cuda" if torch.cuda.is_available() else "cpu")
        self.model = model.to(self.device)
        self.tokenizer = tokenizer


    def forward(self, data, squeeze, output_length=False):
        model_input = self.tokenizer.get_inputs_for_model(data, self.device)
        with torch.no_grad():
            model_output = self.model(**model_input)
        
        def detach(input_data): 
            if isinstance(input_data, tuple):
                return np.stack(list(map(detach, input_data)), axis=0)
            numpy_data = input_data.cpu().detach().numpy()
            return numpy_data.squeeze() if squeeze else numpy_data
            
        last_hidden_state, pooler_output, hidden_states, attention = tuple(map(detach, model_output))

        if output_length:
            lengths = detach(model_input["attention_mask"].sum(axis=1))
            return last_hidden_state, pooler_output, hidden_states, attention, lengths

        return last_hidden_state, pooler_output, hidden_states, attention


    def get_hidden_states(self, data):
        _, _, hidden_states, _ = self.forward(data, True)
        return hidden_states


    def get_evolution(self, sentence: str):
        tokens = self.tokenizer.tokenize(sentence)
        _, _, hidden_states, _ = self.forward(sentence, True)
        _, seq_len, _ = hidden_states.shape

        stability_coefficients = self.get_stability_coefficients(hidden_states=hidden_states)
        cosines = self.cosine(hidden_states)

        sim_maps = []
        for token_idx in range(seq_len):
            sim = cosine_similarity(hidden_states[:, token_idx, :])
            sim_maps.append(sim)

        return tokens, sim_maps, stability_coefficients, cosines


    def parse_filtering_pipeline(self, token_filter):
        if token_filter is None:
            return lambda token, coef, all_coefs: True

        def parse_filter(single_filter):
            if single_filter == "above_average":
                return lambda token, coef, all_coefs: coef >= np.average(all_coefs)
            if single_filter == "no_special_tokens":
                return lambda token, coef, all_coefs: token not in [self.tokenizer.tokenizer.cls_token, self.tokenizer.tokenizer.sep_token]
            raise Exception(f"No predefined filter '{single_filter}'")
        
        filters = []
        if isinstance(token_filter, list):
            for filt in token_filter:
                filters.append(parse_filter(filt))
        else:
            filters.append(parse_filter(token_filter))

        return lambda token, coef, all_coefs: all(map(lambda filt: filt(token, coef, all_coefs), filters))


    def get_stability_coefficients_data(self, sentences):
        _, _, hidden_states, _ = self.forward(sentences, False)
        _, batch_size, _, _ = hidden_states.shape

        assert batch_size == len(sentences)

        avgs = []
        variances = []
        ranges = []
        for idx in tqdm(range(batch_size)):
            stability_coefficients = self.get_stability_coefficients(hidden_states=hidden_states[:, idx, :, :])
            avgs.append(np.average(stability_coefficients))
            variances.append(np.var(stability_coefficients))
            ranges.append(np.max(stability_coefficients) - np.min(stability_coefficients))

        return avgs, variances, ranges


    def rank_tokens(self, sentence, order=False, token_filter=None, embedding_layer=-1, average_subwords=False):
        tokens = self.tokenizer.tokenize(sentence)
        _, _, hidden_states, _ = self.forward(sentence, True)

        stability_coefficients = self.get_stability_coefficients(hidden_states=hidden_states)
        embeddings = hidden_states[embedding_layer, :, :]

        if order:
            indices = np.argsort(stability_coefficients)[::-1]
            tokens = tokens[indices]
            stability_coefficients = stability_coefficients[indices]
            embeddings = embeddings[indices, :]

        filter_pipeline = self.parse_filtering_pipeline(token_filter)
        filtered_indices = []
        _, seq_len, _ = hidden_states.shape
        for idx, (token, coef) in enumerate(zip(tokens, stability_coefficients)):
            if filter_pipeline(token, coef, stability_coefficients):
                filtered_indices.append(idx)

        if average_subwords:
            tokens, stability_coefficients, embeddings = self.complete_subwords(tokens, embeddings, stability_coefficients, filtered_indices)
        else:
            tokens, stability_coefficients, embeddings = tokens[filtered_indices], stability_coefficients[filtered_indices], embeddings[filtered_indices, :]

        return tokens, stability_coefficients, embeddings


    def complete_subwords(self, all_tokens, all_embeddings, all_coeffs, filtered_indices):
        def subwords_groups(tokens):
            grouped = []
            for idx, token in enumerate(tokens):
                if token.startswith("##"):
                    start = grouped.pop()
                    start.append(idx)
                    grouped.append(start)
                else:
                    grouped.append([idx])
            return grouped

        def contains(group): return any(subword_idx for subword_idx in group if subword_idx in filtered_indices)

        groups = subwords_groups(all_tokens)
        result = list(filter(contains, groups))

        tokens = []
        embeddings = []
        coeffs = []

        for group in result:
            current_embeddings = []
            current_tokens = []
            current_coeffs = []

            for token_idx in group:
                current_tokens.append(all_tokens[token_idx])
                current_embeddings.append(all_embeddings[token_idx])
                current_coeffs.append(all_coeffs[token_idx])

            tokens.append("".join(current_tokens).replace("##", ""))
            embeddings.append(np.mean(current_embeddings, axis=0))
            coeffs.append(np.mean(current_coeffs))

        return np.array(tokens), np.array(coeffs), np.stack(embeddings, axis=0)


    def cosine(self, hidden_states):
        _, seq_len, _ = hidden_states.shape
        sims = []
        for token_idx in range(seq_len):
            sims.append(cosine_similarity(
                [hidden_states[0, token_idx, :]],
                [hidden_states[-1, token_idx, :]]).squeeze())
        return sims


    def get_stability_coefficients(self, 
                                   sentence: str=None, 
                                   hidden_states: typing.Union[torch.Tensor, np.array]=None):

        if sentence:
            tokens = self.tokenizer.tokenize(sentence)
            _, _, hidden_states, _ = self.forward(sentence, True)

        if hidden_states is None:
            raise Exception("Please, provide sentence or hidden_states") 

        layer_num, seq_len, _ = hidden_states.shape
        assert layer_num % 2 == 1
        middle = layer_num // 2
        left = np.arange(middle + 1, layer_num)[::-1]
        right = np.arange(0, middle)
        index_pairs = list(zip(left, right))

        stability_coefficients = []
        for token_idx in range(seq_len):
            stability = 0

            for left_idx, right_idx in index_pairs:
                stability += cosine_similarity(
                    [hidden_states[left_idx, token_idx, :]], 
                    [hidden_states[right_idx, token_idx, :]]).squeeze()

            stability = stability / len(index_pairs)
            stability_coefficients.append(stability)

        stability_coefficients = np.array(stability_coefficients)

        if sentence:
            return tokens, stability_coefficients
        else:
            return stability_coefficients


    def average_embeddings(self, sentence, layer=-1):
        _, _, hidden_states, _, length = self.forward(sentence, False, output_length=True)
        embeddings = hidden_states[layer]
        mean = []
        for i in range(len(length)):
            mean.append(embeddings[i, :length[i], :].mean(axis=0))
        mean = np.array(mean)
        return mean.squeeze() if isinstance(sentence, str) else mean


    def pooled_similarity(self, query, reference):
        return cosine_similarity(
            [self.average_embeddings(query)],
            [self.average_embeddings(reference)]).squeeze()


    def pairwise_similarity(self, query, reference, layer, token_filter=["above_average", "no_special_tokens"]):
        query_hs = self.get_hidden_states(query)
        reference_hs = self.get_hidden_states(reference)
        sim_map = cosine_similarity(query_hs[layer, :, :], reference_hs[layer, :, :])

        flat = sim_map.flatten()
        indices = flat.argsort()[::-1]
        _, cols = sim_map.shape
        sorted_similarity = [(i // cols, i % cols, flat[i]) for i in indices]

        query_stability = self.get_stability_coefficients(hidden_states=query_hs)
        reference_stability = self.get_stability_coefficients(hidden_states=reference_hs)

        query_tokens = self.tokenizer.tokenize(query)
        reference_tokens = self.tokenizer.tokenize(reference)
        
        filtering = self.parse_filtering_pipeline(token_filter)

        pairs = [(query_tokens[row], reference_tokens[col], similarity) for row, col, similarity in sorted_similarity 
                if filtering(query_tokens[row], query_stability[row], query_stability) and filtering(
                    reference_tokens[col], reference_stability[col], reference_stability)]

        return sim_map, pairs