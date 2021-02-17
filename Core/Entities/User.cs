﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Core.Entities
{
    public class User : IdentityUser<Guid>
    {
        public virtual ICollection<Query> Queries { get; set; }
    }
}
