using CRET.Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRET.Repository.Interface
{
    public interface IOrganizationRepository : IRepository<Organization>
    {
        public void CreateOrganization(Organization organization);
        public Organization GetOrganizationById(Guid id);
        public List<Organization> GetOrganizations();
        public void UpdateOrganization(Organization organization);
        public void DeleteOrganization(Organization organization);
    }
}
