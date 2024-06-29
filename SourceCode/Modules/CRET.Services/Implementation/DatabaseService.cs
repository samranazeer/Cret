using CRET.DataAccessLayer;
using CRET.Domain.Models.Entities;
using CRET.Repository.Implementation;
using CRET.Repository.Interface;
using CRET.Services.Interface;
using Microsoft.Extensions.Logging;
using Certificate = CRET.Domain.Models.Entities.Certificate;

namespace CRET.Services.Implementation
{
    public class DatabaseService : IDatabaseService
    {
        private IRepository<Organization> OrganizationRepo;
        private IRepository<Certificate> CertificateRepo;
        private IRepository<Setting> SettingRepo;
        private IRepository<AllowedUser> AllowedUserRepo;


        private ILogger logService;
        public DatabaseService(ApplicationDbContext dBContext, ILogger<DatabaseService> logger)
        {
            OrganizationRepo = new BaseRepository<Organization>(dBContext);
            CertificateRepo = new BaseRepository<Certificate>(dBContext);
            AllowedUserRepo = new BaseRepository<AllowedUser>(dBContext);
            SettingRepo = new BaseRepository<Setting>(dBContext);
            logService = logger;
        }

       
        
    }
}
