using System;

namespace TextClustering.Web.Models
{
    public class DatasetClustering
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public DateTimeOffset CreatedAtUtc { get; set; }

        public User CreatedByUser { get; set; }

        public int DatasetId { get; set; }

        public string DatasetName { get; set; }

        public string DatasetDescription { get; set; }

        public int TextCount { get; set; }

        public int ClusterCount { get; set; }
    }
}