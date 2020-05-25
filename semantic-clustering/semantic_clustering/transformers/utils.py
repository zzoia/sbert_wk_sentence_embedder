import logging
import os
import json
import torch
import shutil
from zipfile import ZipFile
from sentence_transformers import __DOWNLOAD_SERVER__, util

def load_model(model_name_or_path: str):
    logging.info("Load pretrained SentenceTransformer: {}".format(
        model_name_or_path))

    logging.info(
        "Did not find a '/' or '\\' in the name. Assume to download model from server.")
    model_name_or_path = __DOWNLOAD_SERVER__ + model_name_or_path + '.zip'

    model_url = model_name_or_path
    folder_name = model_url.replace(
        "https://", "").replace("http://", "").replace("/", "_")[:250]

    try:
        from torch.hub import _get_torch_home
        torch_cache_home = _get_torch_home()
    except ImportError:
        torch_cache_home = os.path.expanduser(
            os.getenv('TORCH_HOME', os.path.join(
                os.getenv('XDG_CACHE_HOME', '~/.cache'), 'torch')))

    default_cache_path = os.path.join(
        torch_cache_home, 'sentence_transformers')
    model_path = os.path.join(default_cache_path, folder_name)
    os.makedirs(model_path, exist_ok=True)

    if not os.listdir(model_path):
        if model_url[-1] == "/":
            model_url = model_url[:-1]
        logging.info("Downloading sentence transformer model from {} and saving it at {}".format(
            model_url, model_path))
        try:
            zip_save_path = os.path.join(model_path, 'model.zip')
            util.http_get(model_url, zip_save_path)
            with ZipFile(zip_save_path, 'r') as zip:
                zip.extractall(model_path)
        except Exception as e:
            shutil.rmtree(model_path)
            raise e

    with open(os.path.join(model_path, 'modules.json')) as fIn:
        contained_modules = json.load(fIn)
    return model_path, contained_modules


def load_model_path(model_name_or_path: str, model_type: str):
    model_path, contained_modules = load_model(model_name_or_path)

    model_type = "sentence_transformers.models." + model_type
    for module_config in contained_modules:
        if model_type == module_config["type"]:
            return os.path.join(model_path, module_config["path"])