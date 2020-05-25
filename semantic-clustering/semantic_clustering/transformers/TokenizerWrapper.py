from transformers import BertTokenizer
import numpy as np
import torch

class TokenizerWrapper:

    def __init__(self, tokenizer: BertTokenizer):
        self.tokenizer = tokenizer

    def tokenize(self, sentence):
        return np.array([self.tokenizer.cls_token] + self.tokenizer.tokenize(sentence) + [self.tokenizer.sep_token])

    def tokens_to_string(self, tokens):
        ids = self.tokenizer.convert_tokens_to_ids(tokens)
        return self.tokenizer.decode(ids, skip_special_tokens=True)

    def get_inputs_for_model(self, texts, device:torch.device=None):
        texts = self.convert_to_string_list(texts)
        features = self.tokenizer.batch_encode_plus(
            texts,
            return_tensors="pt",
            add_special_tokens=True,
            padding=True)

        if device is None or device.type == "cpu":
            return features 

        for name in features:
            features[name] = features[name].to(device)

        return features 


    def convert_to_string_list(self, data):
        if isinstance(data, str):
            string_input = [data]
        elif isinstance(data, list):
            string_input = data
        else:
            raise Exception("Input not supported")
        return string_input