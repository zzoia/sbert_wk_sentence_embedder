using System.Collections.Generic;
using System.Threading.Tasks;
using TextClustering.Application.Dto;

namespace TextClustering.Application
{
    public interface IDatasetClusteringService
    {
        Task<IReadOnlyCollection<DatasetClusteringDto>> GetAll(
            int userId,
            int? datasetClusteringId);

        Task<IReadOnlyCollection<ClusterDto>> GetClusters(int datasetClusteringId, int userId);

        Task<DatasetClusteringDto> CreateAndClusterDataset(
            string datasetName,
            string datasetDescription,
            string[] texts,
            int userId);
    }
}