using System.Collections.Generic;

namespace TextClustering.Domain.Entities
{
    public class Dataset
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int CreatedById { get; set; }

        public User CreatedByUser { get; set; }

        public ICollection<DatasetText> Texts { get; set; }

        public ICollection<DatasetClustering> DatasetClusterings { get; set; }
    }
}