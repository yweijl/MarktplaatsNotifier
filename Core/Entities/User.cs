using System.Collections.Generic;

namespace Core.Entities
{
    public class User : EntityBase
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public virtual ICollection<Query> Queries { get; set; }
    }
}
