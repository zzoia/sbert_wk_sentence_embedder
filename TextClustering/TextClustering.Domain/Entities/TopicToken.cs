namespace TextClustering.Domain.Entities
{
    public class TopicToken
    {
        public int Id { get; set; }

        public int TopicId { get; set; }

        public Topic Topic { get; set; }

        public string Token { get; set; }
    }
}