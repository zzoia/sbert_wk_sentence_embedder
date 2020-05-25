import pandas as pd
import numpy as np
from tqdm.auto import tqdm
import matplotlib.pyplot as plt
import seaborn as sns
import operator
import os
from collections import defaultdict

from sklearn.metrics.pairwise import cosine_similarity
import scipy.cluster.hierarchy as sch
import community as community_louvain
import networkx as nx

class DataSet:

  def __init__(self, path_or_tuples, analyzer):
    lines = []

    if isinstance(path_or_tuples, str):
      with open(path) as file:
        for line in file:
          line = line.strip().split("|")
          lines.append(tuple(line))
    else:
      lines = path_or_tuples

    self.analyzer = analyzer
    self.lines = np.array(lines)
    
  def plot_length_avg_stability(self):
    lengths = np.array(list(map(lambda line: len(line.split()), self.lines)))
    by_length = lengths.argsort()
    lengths = lengths[by_length]
    
    avgs, vars, ranges = self.analyzer.get_stability_coefficients_data(self.lines.tolist())

    avgs, = plt.plot(lengths, avgs)
    vars, = plt.plot(lengths, vars)
    ranges, = plt.plot(lengths, ranges)
    plt.legend([avgs, vars, ranges], ["avg", "var", "range"], loc="upper left")
    plt.show()

  def group_agglomerative(self, plot=True, print_results=True):
    """
    Performs multitopic clustering of texts from supplied indices or from
    the entire dataset if no indices were supplier in the arguments.
    """
    all_tokens, all_embs, line_indices = self.__get_stable_tokens(print_results=print_results, use_key_as_index=True)
    lines = self.__get_lines()

    linkage_matrix = sch.linkage(all_embs, metric="cosine", method="average")
    threshold = max(linkage_matrix[:, 2]) * 0.7

    if print_results:
      print("Cluster threshold:", np.round(threshold, 3))

    ax, jaccard_ax = None, None
    if plot:
      fig, (ax, jaccard_ax) = plt.subplots(1, 2, figsize=(7 * 2, 7))

    dendrogram = sch.dendrogram(
        linkage_matrix,
        labels=list(zip(all_tokens, line_indices)),
        orientation="right",
        color_threshold=threshold,
        ax=ax,
        no_plot=ax is None
    )

    cluster_numbers = sch.fcluster(linkage_matrix, threshold, criterion='distance')
    clusters = defaultdict(list)

    for token, sent_idx, number in zip(all_tokens, line_indices, cluster_numbers):
      clusters[number].append((token, sent_idx))
    
    filtered_clusters = []

    for cluster in clusters:
      cluster_value = clusters[cluster]

      topic_tokens = "{ " + ", ".join(set(map(operator.itemgetter(0), cluster_value))) + " }"
      indices = list(set(map(operator.itemgetter(1), cluster_value)))

      if len(indices) > 1:
        filtered_clusters.append(cluster_value)
        if print_results:
          print(len(filtered_clusters), topic_tokens, ":", indices)

    jaccard_matrix = self.__get_jaccard_matrix(filtered_clusters)
    if plot:
      sns.heatmap(jaccard_matrix, ax=jaccard_ax)

    topic_communities = self.__clusters_from_jaccard(jaccard_matrix)
    def filter_topic(tokens): return list(set(map(operator.itemgetter(0), tokens)))

    result = []

    for comm_idx, topic_community in enumerate(topic_communities):

      sentence_keys = []
      topics = []

      cluster_model = { "topicTokens": [] }

      for topic_idx in topic_community:
        topic_tokens = filtered_clusters[topic_idx] # [(0 - token, 1 - sentence index), ...]
        cluster_model["topicTokens"].append(filter_topic(topic_tokens))

        topics.append("{ " + ", ".join(cluster_model["topicTokens"][-1]) + " }")
        sentence_keys.extend(set(map(operator.itemgetter(1), topic_tokens)))

      sentence_keys = list(set(sentence_keys))
      sentences = filter(lambda line: any(filter(lambda key: key == line[0], sentence_keys)), lines)
      sentences = map(operator.itemgetter(1), sentences)

      cluster_model["sentenceKeys"] = sentence_keys
      result.append(cluster_model)

      if print_results:
        print("#", comm_idx, topics)
        print(os.linesep.join(sentences))
        print("-" * 40)

    return {
      "clusters": result
    }

  def __cluster_key_words(self, sim_map, all_tokens):
    sim_sum = (sim_map.sum(axis=0) - 1) / (sim_map.shape[0] - 1)

    # Cluster key tokens
    graph = nx.from_numpy_matrix(sim_map)
    partition = community_louvain.best_partition(graph)
    clusters = self.__partition_to_dict(partition, all_tokens)

    data = []
    for key in clusters:
      tokens = list(map(operator.itemgetter(0), clusters[key]))
      max_sim = None
      centroid = None
      for token, idx in clusters[key]:
        if max_sim is None or max_sim < sim_sum[idx]:
          max_sim = sim_sum[idx]
          centroid = token
      data.append((centroid, max_sim, tokens))

    data.sort(key=operator.itemgetter(1), reverse=True)
    return data

  def label_clusters(self, indices=[], plot=True, print_results=True):
    """
    Extracts key words and theirs embeddings from lines. Computes similarity map
    between flattened key token embeddings from all lines. Cluster key tokens 
    from all lines. Calculate main token in cluster by maximum aggregate similarity
    to this token from similarity map.
    """
    all_tokens, all_embs, line_indices = self.__get_stable_tokens(indices, print_results=print_results, use_key_as_index=False)

    sim_map = cosine_similarity(all_embs)
    tick_labels = list(zip(all_tokens, line_indices))

    # Plots
    if plot:
      fig, (sims_ax) = plt.subplots(1, 1, figsize=(5, 4))
      sns.heatmap(sim_map, vmax=1, vmin=0, xticklabels=tick_labels, yticklabels=tick_labels, ax=sims_ax)

    def sim_from_others(token_idx, sentence_idx):
      other_indices = list(map(
          operator.itemgetter(0), 
          filter(lambda x: x[1][1] != sentence_idx, enumerate(tick_labels))
      ))
      sim_row = sim_map[token_idx, other_indices]
      return sim_row.mean(), sim_row.var()

    def format_number(number): return "{:.2f}".format(number)

    pairs = []
    for q_idx, (query_token, query_sen_idx) in enumerate(zip(all_tokens, line_indices)):
      mean_sim, var = sim_from_others(q_idx, query_sen_idx)
      pairs.append({
        "token": query_token, 
        "from_others": format_number(mean_sim),
        "var_from_others": format_number(var),
        "from_other_times_var": format_number(mean_sim * var)
      })

    return self.__cluster_key_words(sim_map, all_tokens), pd.DataFrame(pairs)

  def __get_stable_tokens(self, print_results=True, use_key_as_index=True):
    lines = self.lines

    all_embs = []
    all_tokens = []
    line_indices = []
    for idx, (key, line) in enumerate(lines):
      tokens, _, embeddings = self.analyzer.rank_tokens(line,
                                                        token_filter=["no_special_tokens", "above_average"],
                                                        embedding_layer=-3,
                                                        average_subwords=True)
      all_embs.extend(embeddings)
      all_tokens.extend(tokens)
      line_indices.extend([key if use_key_as_index else idx] * len(tokens))
      if print_results:
        print(line, tokens)

    assert len(all_tokens) == len(all_embs)
    assert len(all_tokens) == len(line_indices)

    all_tokens = np.array(all_tokens)
    if print_results:
      print(os.linesep)
    return all_tokens, all_embs, line_indices

  def __partition_to_dict(self, partition, data_source):
    clusters = defaultdict(list)
    for key in partition:
      cluster = partition[key]
      clusters[cluster].append((data_source[key], key))
    return clusters

  def __get_lines(self): return self.lines
   
  def __jaccard_similarity(self, list1, list2):
    s1 = set(list1)
    s2 = set(list2)
    return len(s1.intersection(s2)) / len(s1.union(s2)) # min(len(list1), len(list2))

  def __get_jaccard_matrix(self, filtered_clusters):
    jaccard_matrix = np.zeros((len(filtered_clusters), len(filtered_clusters)))
    for o_i, cluster in enumerate(filtered_clusters):
      indices = list(set(map(operator.itemgetter(1), cluster)))
      
      for i_i, inner in enumerate(filtered_clusters):
        inner_indices = list(set(map(operator.itemgetter(1), inner)))
        jaccard_sim = self.__jaccard_similarity(indices, inner_indices)
        jaccard_matrix[o_i, i_i] = jaccard_sim
    return jaccard_matrix

  def __clusters_from_jaccard(self, jaccard_matrix):
    graph = nx.from_numpy_matrix(jaccard_matrix)
    partition = community_louvain.best_partition(graph)
    clusters = defaultdict(list)
    for key in partition:
      clusters[partition[key]].append(key)
    return list(clusters.values())