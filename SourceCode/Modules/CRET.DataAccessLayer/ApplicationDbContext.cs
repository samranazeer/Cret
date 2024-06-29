using CRET.DataAccessLayer.SeedingExtension;
using CRET.Domain.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRET.DataAccessLayer
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Override OnModelCreating if you need to configure your entities
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Seed Data
            modelBuilder.Seed();
            // Configure your entities here if needed
        }
        public DbSet<Organization> Organization => Set<Organization>();
        public DbSet<Certificate> Certificate => Set<Certificate>();
        public DbSet<Setting> Setting => Set<Setting>();
        public DbSet<AllowedUser> AllowedUser => Set<AllowedUser>();


    }
}
