using System.Collections.Generic;

namespace TextClustering.Web.Models
{
    public class Topic
    {
        public int Id { get; set; }

        public ICollection<string> Tokens { get; set; }
    }
}