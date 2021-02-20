using System.Collections.Generic;

namespace Core.Entities
{
    public class Query : EntityBase
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public int Interval { get; set; }
        public long UserId { get; set; }
        public virtual ICollection<Advertisement>  Advertisements { get; set; }
    }
}