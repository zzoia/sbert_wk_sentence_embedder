using TextClustering.Domain.Entities;

namespace TextClustering.Application.Dto
{
    public class DatasetClusteringDto
    {
        public DatasetClustering DatasetClustering { get; set; }

        public int TextCount { get; set; }

        public int ClusterCount { get; set; }
    }
}