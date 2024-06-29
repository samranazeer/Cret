

using CRET.Domain.Models.Entities;

namespace CRET.DataAccessLayer.SeedingExtension
{
    public class DefaultCertificateSetting
    {
        public static Setting CertificateSetting()
        {
            return new Setting
            {
                Id = Guid.Parse("6c628d2c-9322-469f-861d-436472fe2627"),
                CreatedAt  = new DateTime(2024, 3, 28, 15, 40, 41, 0, DateTimeKind.Unspecified),
                CreatedBy = "Admin",
                ConsumerValidity = 730,
                 ServiceValidity = 90,
                 InsValidity = 90,
                 ProductionValidity = 90,
                 LabValidity = 90,
                 BatchValidity =90,
                 PulseValidity = 90
            };
        }
    }
}
