from semantic_clustering import DataSet
from semantic_clustering.transformers import load_model_path, TokenizerWrapper, ModelAndTokenizer
from transformers import BertModel, BertTokenizer
import os

tokenizer = BertTokenizer.from_pretrained(
    "bert-base-uncased", 
    do_lower_case=True)

path = load_model_path("bert-base-nli-mean-tokens", "BERT")
sbert_model = BertModel.from_pretrained(
    path,
    output_hidden_states=True,
    output_attentions=True)

tokenizer_wrapper = TokenizerWrapper(tokenizer)

sbert_analyzer = ModelAndTokenizer(sbert_model, tokenizer_wrapper)

dataset = DataSet(os.path.join("..", "text_clustering_web", "data", "small_sents.txt"), sbert_analyzer)
result = dataset.group_agglomerative()
print(result)