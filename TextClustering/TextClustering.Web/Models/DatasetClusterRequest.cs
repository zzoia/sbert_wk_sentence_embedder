namespace TextClustering.Web.Models
{
    public class DatasetClusterRequest
    {
        public string DatasetName { get; set; }

        public string DatasetDescription { get; set; }

        public string[] Texts { get; set; }
    }
}
