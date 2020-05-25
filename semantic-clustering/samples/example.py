from sklearn.metrics.pairwise import cosine_similarity
from semantic_clustering.transformers import TokenizerWrapper, ModelAndTokenizer, load_model_path
from transformers import BertTokenizer, BertModel
from sentence_transformers import SentenceTransformer
import torch

device = torch.device("cuda" if torch.cuda.is_available() else "cpu")

sentences = [
    "The invention of reusable rockets was a key step in commercial space travel",
    "Elon Musk is the founder of SpaceX"
]

def check_same_models():
    sbert_model_name = "bert-base-nli-mean-tokens"
    tokenizer_wrapper = TokenizerWrapper(BertTokenizer.from_pretrained("bert-base-uncased", do_lower_case=True))

    sbert_wk_analyzer = ModelAndTokenizer(
        BertModel.from_pretrained("binwang/bert-base-nli", output_hidden_states=True, output_attentions=True), 
        tokenizer_wrapper,
        device=device)

    sbert_analyzer = ModelAndTokenizer(
        BertModel.from_pretrained(load_model_path(sbert_model_name, "BERT"), output_hidden_states=True, output_attentions=True), 
        tokenizer_wrapper,
        device=device)

    embedder = SentenceTransformer(sbert_model_name)

    embedders = [
        ("SBERT-WK", sbert_wk_analyzer.average_embeddings),
        ("SBERT", sbert_analyzer.average_embeddings),
        ("SentenceTransformer", embedder.encode)
    ]

    for name, embedder in embedders:
        embedding = embedder(sentences)
        print(name, cosine_similarity([embedding[0]], [embedding[1]]))

