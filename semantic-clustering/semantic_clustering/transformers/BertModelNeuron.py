from transformers import modeling_bert as mb
import torch
from torch import nn
import math

# 15/06/2020

class BertModelNeuron(mb.BertModel):
    
    def __init__(self, config):
        config.output_attentions = True
        super().__init__(config)

        self.encoder = mb.BertEncoder(self.config)
        self.encoder.layer = nn.ModuleList([self.__build_bert_layer() for _ in range(self.config.num_hidden_layers)])


    def __build_bert_layer(self):
        layer = mb.BertLayer(self.config)
        layer.attention = self.__build_bert_attention()
        if layer.is_decoder:
            layer.crossattention = self.__build_bert_attention()
        return layer


    def __build_bert_attention(self):
        attention = mb.BertAttention(self.config)
        attention.self = BertSelfAttentionNeuron(self.config)
        return attention


class BertSelfAttentionNeuron(mb.BertSelfAttention):

    def __init__(self, config):
        super().__init__(config) 


    def forward(
        self,
        hidden_states,
        attention_mask=None,
        head_mask=None,
        encoder_hidden_states=None,
        encoder_attention_mask=None,
        output_attentions=False):

        outputs = super().forward(hidden_states,
                                  attention_mask=attention_mask,
                                  head_mask=head_mask,
                                  encoder_hidden_states=encoder_hidden_states,
                                  encoder_attention_mask=encoder_attention_mask,
                                  output_attentions=output_attentions)

        context_layer, attention_probs = outputs

        mixed_query_layer = self.query(hidden_states)
        query_layer = self.transpose_for_scores(mixed_query_layer)

        mixed_key_layer = self.key(encoder_hidden_states if encoder_hidden_states is not None else hidden_states)
        key_layer = self.transpose_for_scores(mixed_key_layer)

        attn_data = {
            "attn": attention_probs,
            "queries": query_layer,
            "keys": key_layer
        }
        return context_layer, attn_data