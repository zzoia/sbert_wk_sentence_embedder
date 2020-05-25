from flaskr import app, get_analyzer
from flask import request
from semantic_clustering import DataSet
from timeit import default_timer as timer


@app.route("/cluster", methods=["POST"])
def index():
    cluster_request = request.json["texts"]
    analyzer = request.json["analyzer"] if "analyzer" in request.json else None
    lines = []
    for text in cluster_request:
        lines.append((text["key"], text["text"]))

    start = timer()
    dataset = DataSet(lines, get_analyzer(analyzer))
    end = timer()

    preparation_time = end - start
    print("Preparation time", preparation_time, "seconds")


    start = timer()
    clusters = dataset.group_agglomerative(plot=False, print_results=False)
    end = timer()

    algorithm_time = end - start
    print("Algorithm time", algorithm_time, "seconds")


    clusters["preparationSeconds"] = preparation_time
    clusters["algorithmSeconds"] = algorithm_time

    return clusters