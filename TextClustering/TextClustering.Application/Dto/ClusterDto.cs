using TextClustering.Domain.Entities;

namespace TextClustering.Application.Dto
{
    public class ClusterDto
    {
        public Cluster Cluster { get; set; }

        public int TextCount { get; set; }
    }
}