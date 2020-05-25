from flask import Flask
from collections import defaultdict
from semantic_clustering.transformers import load_model_path, TokenizerWrapper, ModelAndTokenizer
from transformers import BertModel, BertTokenizer
import logging
from sys import stdout

logger = logging.getLogger(__name__)
logger.setLevel(logging.DEBUG)

consoleHandler = logging.StreamHandler(stdout)
consoleHandler.setFormatter(logging.Formatter("%(name)-12s %(asctime)s %(levelname)-8s %(filename)s:%(funcName)s %(message)s"))

logger.addHandler(consoleHandler)

app = Flask(__name__)

def init_analyzers():
    tokenizer = BertTokenizer.from_pretrained(
        "bert-base-uncased", 
        do_lower_case=True)

    model_name = "bert-base-nli-mean-tokens"
    model_path = load_model_path(model_name, "BERT")
    
    sbert_model = BertModel.from_pretrained(
        model_path,
        output_hidden_states=True,
        output_attentions=True)

    sbert_analyzer = ModelAndTokenizer(sbert_model, TokenizerWrapper(tokenizer))
    
    analyzers = defaultdict(lambda: sbert_analyzer)
    analyzers[model_name] = sbert_analyzer
    return analyzers

analyzers = init_analyzers()

def get_analyzer(analyzer): return analyzers[analyzer]

from flaskr import routes