using CRET.DataAccessLayer;
using CRET.Domain.Models.Entities;
using CRET.Repository.Interface;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRET.Repository.Implementation
{
    public class UserRepository : BaseRepository<AllowedUser>, IUserRepository
    {
        private readonly ILogger _logger;
        public UserRepository(ApplicationDbContext dbContext, ILogger<UserRepository> logger) : base(dbContext)
        {
            _logger = logger;
        }
        public AllowedUser GetUserIdList()
        {
            AllowedUser result = null;
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

        public void AddAllowedUser(AllowedUser allowedUser)
        {
            try
            {
                var res = Get().FirstOrDefault();
                if (res == null)
                {
                    Insert(allowedUser);
                }
                else
                {
                    res.UserIdList = allowedUser.UserIdList;
                    Update(res);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

    }
}
