using Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure
{
    public class DatabaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Query> Queries { get; set; }
        public DbSet<Advertisement> Advertisements { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        { }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
          
        }
    }
}
