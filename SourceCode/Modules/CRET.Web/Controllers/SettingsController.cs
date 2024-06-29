using CRET.Domain.Models.Entities;
using CRET.Repository.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRET.Controllers
{
    public class SettingsController : BaseController
    {
        private readonly ILogger<SettingsController> _logger;
        private readonly ISettingRepository _settingRepository;

        public SettingsController(ILogger<SettingsController> logger, ISettingRepository settingRepository)
        {
            _logger = logger;

            _settingRepository = settingRepository;
        }

        [Authorize]
        [HttpGet("GetCertificateValidity")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var result = _settingRepository.GetValiditySetting();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }



        [HttpPost("AddUpdateCertificateValidity")]
        public async Task<IActionResult> AddUpdateCertificateValidity([FromBody] Setting setting)
        {
            try
            {
                if (setting == null)
                {
                    return BadRequest("Setting object is null");
                }
                var createdBy = string.Empty;
                if (User != null)
                {
                    var claim = User.Claims.FirstOrDefault(c => c.Type == "name");
                    if (claim != null)
                    {
                        createdBy = claim.Value;
                    }
                }
                setting.CreatedAt = DateTime.UtcNow;
                setting.CreatedBy = createdBy;
                _settingRepository.AddUpdateValiditySettings(setting);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}