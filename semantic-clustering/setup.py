from setuptools import setup, find_packages

with open("README.md", "r") as fh:
    long_description = fh.read()

setup(
    name="semantic_clustering",
    version="0.0.1",
    author="Zoia Ostapiuk",
    author_email="zoeostapiuk@gmail.com",
    description="Batch implementation for SBERT-WK algorithm by Wang, Bin and Kuo, C-C Jay",
    long_description=long_description,
    long_description_content_type="text/markdown",
    url="https://github.com/zzoia/semantic_clustering",
    classifiers=[
        "Programming Language :: Python :: 3",
        "License :: OSI Approved :: MIT License",
        "Operating System :: OS Independent",
    ],
    packages=find_packages(),
    package_data={"": ["*.js"]},
    include_package_data=True,
    install_requires=[
        "transformers==3.0.2",
        "tqdm",
        "torch===1.4.0",
        "torchvision===0.5.0",
        "torch>=1.2.0",
        "torchvision",
        "numpy",
        "scikit-learn==0.22.2",
        "sentence-transformers==0.3.2",
        "pandas==1.0.5",
        "seaborn",
        "networkx",
        "python-louvain",
        "flask==1.1.2",
        "python-dotenv",
        "gunicorn==20.0.4"
    ],
    dependency_links=[
        "https://download.pytorch.org/whl/torch_stable.html"
    ]
)
