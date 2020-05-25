using System.Collections.Generic;

namespace TextClustering.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public ICollection<DatasetClustering> DatasetClusterings { get; set; }

        public ICollection<Dataset> Datasets { get; set; }
    }
}