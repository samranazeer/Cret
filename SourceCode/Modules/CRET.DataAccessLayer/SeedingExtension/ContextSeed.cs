
using CRET.Domain.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRET.DataAccessLayer.SeedingExtension
{
    public static class ContextSeed
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            SeedCertificateSettings(modelBuilder);
        }
        private static void SeedCertificateSettings(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Setting>().HasData(DefaultCertificateSetting.CertificateSetting());
        }

        private static void SeedMigration(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<DatabaseMigration>().HasData(DefaultMigration.InitialMigration());
        }
    }
}
