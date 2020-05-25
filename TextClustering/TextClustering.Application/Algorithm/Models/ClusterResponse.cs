namespace TextClustering.Application.Algorithm
{
    public class ClusterResponse
    {
        public ClusterItemResponse[] Clusters { get; set; }

        public double AlgorithmSeconds { get; set; }

        public double PreparationSeconds { get; set; }
    }
}