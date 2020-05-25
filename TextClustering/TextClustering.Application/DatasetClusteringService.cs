using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TextClustering.Application.Algorithm;
using TextClustering.Application.Dto;
using TextClustering.Domain;
using TextClustering.Domain.Entities;

namespace TextClustering.Application
{
    public class DatasetClusteringService : IDatasetClusteringService
    {
        private readonly TextClusteringDbContext _dbContext;

        private readonly IAlgorithmService _algorithmService;

        public DatasetClusteringService(
            TextClusteringDbContext dbContext,
            IAlgorithmService algorithmService)
        {
            _dbContext = dbContext;
            _algorithmService = algorithmService;
        }

        public async Task<IReadOnlyCollection<DatasetClusteringDto>> GetAll(
            int userId, 
            int? datasetClusteringId)
        {
            List<DatasetClustering> clusterings = await _dbContext.DatasetClusterings
                .Include(clustering => clustering.CreatedByUser)
                .Include(clustering => clustering.Dataset)
                .Where(clustering => clustering.Id == datasetClusteringId || !datasetClusteringId.HasValue)
                .ToListAsync();

            var textsCount = await _dbContext.DatasetTexts.GroupBy(
                    text => text.DatasetId,
                    (datasetId, texts) => new {DatasetId = datasetId, TextsCount = texts.Count()})
                .ToListAsync();

            var clustersCount = await _dbContext.Clusters.GroupBy(
                    cluster => cluster.DatasetClusteringId,
                    (clusteringId, clusters) => new {ClusteringId = clusteringId, ClustersCount = clusters.Count()})
                .ToListAsync();

            List<DatasetClusteringDto> dtos = clusterings.Select(clustering =>
            {
                return new DatasetClusteringDto
                {
                    DatasetClustering = clustering,
                    ClusterCount = clustersCount.SingleOrDefault(arg => arg.ClusteringId == clustering.Id)
                        .ClustersCount,
                    TextCount = textsCount.SingleOrDefault(arg => arg.DatasetId == clustering.DatasetId).TextsCount
                };
            }).ToList();
            
            return dtos;
        }
        
        public async Task<DatasetClusteringDto> CreateAndClusterDataset(
            string datasetName,
            string datasetDescription,
            string[] texts, 
            int userId)
        {
            var datasetTexts = ToDatasetTexts(texts);

            var dataset = new Dataset
            {
                CreatedById = userId,
                Description = datasetDescription,
                Name = datasetName,
                Texts = datasetTexts
            };

            DatasetClustering result = await _algorithmService.GetClusteringResult(dataset);
            result.CreatedById = userId;

            await _dbContext.AddAsync(result);
            await _dbContext.SaveChangesAsync();

            var saved = await GetAll(userId, dataset.Id);
            return saved.Single();
        }

        public async Task<IReadOnlyCollection<ClusterDto>> GetClusters(
            int datasetClusteringId,
            int userId)
        {
            var clusters = await _dbContext.Clusters
                .Include(cluster => cluster.ClusterDatasetTexts)
                .ThenInclude(text => text.DatasetText)
                .Include(cluster => cluster.Topics)
                .ThenInclude(topic => topic.Tokens)
                .Where(cluster => cluster.DatasetClusteringId == datasetClusteringId)
                .Where(cluster => cluster.DatasetClustering.CreatedById == userId)
                .ToListAsync();

            var textCounts = await _dbContext.ClusterDatasetTexts
                .Where(text => text.Cluster.DatasetClusteringId == datasetClusteringId)
                .GroupBy(
                    text => text.ClusterId,
                    (clusterId, texts) => new { ClusterId = clusterId, TextsCount = texts.Count() })
                .ToListAsync();

            var result = new List<ClusterDto>();

            foreach (var cluster in clusters)
            {
                var clusterTextCount = textCounts.Single(data => data.ClusterId == cluster.Id);

                result.Add(new ClusterDto
                {
                    Cluster = cluster,
                    TextCount = clusterTextCount.TextsCount
                });
            }

            return result;
        }

        private static ICollection<DatasetText> ToDatasetTexts(string[] texts) =>
            texts.Select(text => new DatasetText
            {
                Key = Guid.NewGuid().ToString(),
                Text = text
            }).ToList();
    }
}