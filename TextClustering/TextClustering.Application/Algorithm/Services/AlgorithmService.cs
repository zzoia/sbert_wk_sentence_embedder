using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextClustering.Application.Algorithm.Http;
using TextClustering.Domain.Entities;

namespace TextClustering.Application.Algorithm
{
    public class AlgorithmService : IAlgorithmService
    {
        private readonly IClusteringApi _clusteringApi;

        public AlgorithmService(IClusteringApi clusteringApi)
        {
            _clusteringApi = clusteringApi;
        }

        public async Task<DatasetClustering> GetClusteringResult(Dataset dataset)
        {
            var request = new ClusterRequest
            {
                Texts = dataset.Texts.Select(text => new TextRequest
                {
                    Key = text.Key,
                    Text = text.Text
                }).ToList()
            };

            var clusters = await _clusteringApi.Cluster(request);

            return new DatasetClustering
            {
                Clusters = clusters.Clusters.Select(clusterModel =>
                {
                    IList<ClusterDatasetText> texts = dataset.Texts
                        .Where(text => clusterModel.SentenceKeys.Contains(text.Key))
                        .Select(text => new ClusterDatasetText
                        {
                            DatasetText = text
                        })
                        .ToList();

                    return new Cluster
                    {
                        Topics = clusterModel.TopicTokens.Select(collection => new Topic
                        {
                            Tokens = collection.Select(token => new TopicToken
                            {
                                Token = token
                            }).ToList()
                        }).ToList(),
                        ClusterDatasetTexts = texts
                    };
                }).ToList(),
                CreatedAtUtc = DateTimeOffset.UtcNow,
                Dataset = dataset,
                Description = "Clustering created Python default Transformers"
            };
        }
    }
}