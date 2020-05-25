using System.Collections.Generic;

namespace TextClustering.Application.Algorithm
{
    public class ClusterItemResponse
    {
        public IReadOnlyCollection<IReadOnlyCollection<string>> TopicTokens { get; set; }

        public IReadOnlyCollection<string> SentenceKeys { get; set; }
    }
}