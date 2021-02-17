using Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace Api.Infrastructure
{
    public class DatabaseContext : IdentityDbContext<User, Role, Guid>  
    {
        public DbSet<Query> Queries { get; set; }
        public DbSet<Advertisement> Advertisements { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        { }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
