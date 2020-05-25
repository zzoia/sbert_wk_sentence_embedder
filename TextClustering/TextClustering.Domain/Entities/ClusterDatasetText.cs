namespace TextClustering.Domain.Entities
{
    public class ClusterDatasetText
    {
        public int DatasetTextId { get; set; }

        public DatasetText DatasetText { get; set; }

        public int ClusterId { get; set; }

        public Cluster Cluster { get; set; }
    }
}