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
    public class OrganizationRepository : BaseRepository<Organization>, IOrganizationRepository
    {
        private readonly ILogger _logger;
        public OrganizationRepository(ApplicationDbContext dbContext, ILogger<OrganizationRepository> logger) : base(dbContext)
        {
            _logger = logger;
        }

        public void CreateOrganization(Organization organization)
        {
            try
            {
                Insert(organization);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        public Organization GetOrganizationById(Guid id)
        {
            Organization result = null;
            try
            {
                result = GetByID(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return result;
        }

        public List<Organization> GetOrganizations()
        {
            List<Organization> result = null;
            try
            {
                result = Get().ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return result;
        }

        public void UpdateOrganization(Organization organization)
        {
            try
            {
                Update(organization);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        public void DeleteOrganization(Organization organization)
        {
            try
            {
                Delete(organization);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
    }
}
