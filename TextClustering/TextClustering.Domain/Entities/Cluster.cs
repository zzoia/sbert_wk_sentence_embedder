using System.Collections.Generic;

namespace TextClustering.Domain.Entities
{
    public class Cluster
    {
        public int Id { get; set; }

        public int DatasetClusteringId { get; set; }

        public DatasetClustering DatasetClustering { get; set; }

        public ICollection<ClusterDatasetText> ClusterDatasetTexts { get; set; }

        public ICollection<Topic> Topics { get; set; }
    }
}