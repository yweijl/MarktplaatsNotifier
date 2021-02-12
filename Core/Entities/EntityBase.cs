using System;

namespace Core.Entities
{
    public abstract class EntityBase
    {
        public long Id { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime MutationDate { get; set; }
    }
}