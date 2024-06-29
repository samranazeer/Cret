using CRET.Domain.Models.Entities;
using CRET.Repository.Interface;
using Microsoft.AspNetCore.Mvc;

namespace CRET.Controllers
{
    public class UserController : BaseController
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserRepository _userRepository;
        public UserController(ILogger<UserController> logger, IUserRepository userRepository, IConfiguration configuration) : base(configuration)
        {
            _logger = logger;
            _userRepository = userRepository;
        }
        [HttpGet("GetAllowedStatus")]
        public async Task<IActionResult> GetAllowedStatus()
        {
            try
            {
                bool isAuthorized = true;
                bool isSuperAdmin = IsSuperAdmin();
                var result = new
                {
                    isAuthorized = isAuthorized,
                    isSuperAdmin = isSuperAdmin
                };
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("GetAllowedUsers")]
        public async Task<IActionResult> GetAllowedUsers()
        {
            try
            {
                var res = _userRepository.GetUserIdList();
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddAllowedUser([FromBody] AllowedUser allowedUser)
        {
            try
            {
                if (allowedUser == null)
                {
                    return BadRequest("User object is null");
                }
                allowedUser.CreatedAt = DateTime.UtcNow;
                _userRepository.AddAllowedUser(allowedUser);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

    }
}