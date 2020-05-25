using System.Collections.Generic;

namespace TextClustering.Web.Models
{
    public class Cluster
    {
        public int Id { get; set; }

        public int TextCount { get; set; }

        public ICollection<string> Texts { get; set; }

        public ICollection<Topic> Topics { get; set; }
    }
}