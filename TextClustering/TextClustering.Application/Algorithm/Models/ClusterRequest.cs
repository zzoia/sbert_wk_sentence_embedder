using System.Collections.Generic;

namespace TextClustering.Application.Algorithm
{
    public class ClusterRequest
    {
        public IReadOnlyCollection<TextRequest> Texts { get; set; }
    }
}