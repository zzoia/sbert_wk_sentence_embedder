import numpy as np
import pandas as pd
from sklearn.metrics.pairwise import cosine_similarity

class EmbeddingDataFrameWrapper:

    def __init__(self, path_to_csv, embedder=None, text_column="text", pooling="embedding", print_results=True):
        self.embed_df = pd.read_pickle(path_to_csv)
        self.embedder = embedder

        self.text_column = text_column
        self.pooling = pooling
        self.print_results = print_results

    def search(self, query, top_n=None, threshold=None):
        if isinstance(query, str):
            query_embedding = self.embedder(query)
        elif isinstance(query, int):
            query_embedding = self.__get_embedding_by_index(query)
        else:
            query_embedding = query

        index, text, cosine = self.__get_top_similar(query_embedding, top_n, threshold)
        if self.print_results:
            self.__print_similar(zip(index, text, cosine))
        return index, text, cosine

    def get_random_sample(self):
        rnd = self.embed_df.sample(1)
        return (rnd.index[0], rnd[self.text_column].values[0])

    def get_text_by_index(self, index):
        return self.embed_df[self.embed_df.index == index][self.text_column].values[0]

    def __get_embedding_by_index(self, index):
        df = self.embed_df[self.embed_df.index == index]
        if isinstance(self.pooling, str):
            series = df[self.pooling]
        elif callable(self.pooling):
            series = df.apply(self.pooling, axis=1)
        else:
            raise Exception("No pooling strategy provided")
        return series.values[0]

    def __pool(self):
        if isinstance(self.pooling, str):
            series = self.embed_df[self.pooling]
        elif callable(self.pooling):
            series = self.embed_df.apply(self.pooling, axis=1)
        else:
            raise Exception("No pooling strategy provided")
        return series.values.tolist()

    def __get_top_similar(self, query, top_n, threshold):
        similarity = cosine_similarity([query], self.__pool())
        similarity = np.squeeze(similarity)

        indices = np.argsort(similarity)[::-1]
        if threshold:
            indices = indices[:np.sum(similarity > threshold)]
        if top_n:
            indices = indices[:top_n+1]

        texts = self.embed_df.iloc[indices][self.text_column].values

        return self.embed_df.iloc[indices].index.values, texts, similarity[indices]
    
    def __print_similar(self, similar):
        for index, result, cosine in similar:
            str_format = "[{:<7} ({:.2f})] - {}"
            print(str_format.format(f"#{index}", cosine, result))