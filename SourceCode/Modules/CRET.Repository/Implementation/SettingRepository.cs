using CRET.DataAccessLayer;
using CRET.Domain.Models.Entities;
using CRET.Repository.Interface;
using Microsoft.Extensions.Logging;


namespace CRET.Repository.Implementation
{
    public class SettingRepository : BaseRepository<Setting>, ISettingRepository
    {
        private readonly ILogger _logger;
        public SettingRepository(ApplicationDbContext dbContext, ILogger<SettingRepository> logger) : base(dbContext)
        {
            _logger = logger;
        }

        public Setting GetValiditySetting()
        {
            Setting result = null;
            try
            {
                result = Get().FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return result;
        }

        public void AddUpdateValiditySettings(Setting settings)
        {
            Setting? result = null;
            try
            {
                result = Get().FirstOrDefault();
                if (result == null)
                {
                    Insert(settings);
                }
                else
                {
                    result.ConsumerValidity = settings.ConsumerValidity;
                    result.ServiceValidity = settings.ServiceValidity;
                    result.PulseValidity = settings.PulseValidity;
                    result.LabValidity = settings.LabValidity;
                    result.ProductionValidity = settings.ProductionValidity;
                    result.BatchValidity = settings.BatchValidity;
                    result.InsValidity = settings.InsValidity;
                    Update(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        
    }
}
