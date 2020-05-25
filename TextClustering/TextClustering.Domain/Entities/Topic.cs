using System.Collections.Generic;

namespace TextClustering.Domain.Entities
{
    public class Topic
    {
        public int Id { get; set; }

        public int ClusterId { get; set; }

        public Cluster Cluster { get; set; }

        public ICollection<TopicToken> Tokens { get; set; }
    }
}