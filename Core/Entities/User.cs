using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Core.Entities
{
    public class User : IdentityUser<long>
    {
        public virtual ICollection<Query> Queries { get; set; }
    }
}
