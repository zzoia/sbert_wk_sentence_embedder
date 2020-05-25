using System.Collections.Generic;

namespace TextClustering.Domain.Entities
{
    public class DatasetText
    {
        public int Id { get; set; }

        public int DatasetId { get; set; }

        public Dataset Dataset { get; set; }

        public ICollection<ClusterDatasetText> ClusterDatasetTexts { get; set; }

        public string Key { get; set; }

        public string Text { get; set; }
    }
}