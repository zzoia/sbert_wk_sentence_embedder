using System;
using System.Collections.Generic;

namespace TextClustering.Domain.Entities
{
    public class DatasetClustering
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public DateTimeOffset CreatedAtUtc { get; set; }

        public int CreatedById { get; set; }

        public User CreatedByUser { get; set; }

        public int DatasetId { get; set; }

        public Dataset Dataset { get; set; }

        public ICollection<Cluster> Clusters { get; set; }
    }
}