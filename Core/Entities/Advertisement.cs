namespace Core.Entities
{
    public class Advertisement : EntityBase
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public string Url { get; set; }
        public string ImageUrl { get; set; }
        public long QueryId { get; set; }
    }
}