using CRET.DataAccessLayer;
using CRET.Domain.Models.DataModel;
using CRET.Domain.Models.Entities;
using CRET.Repository.Interface;
using CRET.Web.Utills;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRET.Controllers
{
    public class OrganizationController : BaseController
    {
        private readonly ILogger<OrganizationController> _logger;
        private readonly IOrganizationRepository _organizationRepository;
        ApplicationDbContext _context;
        public OrganizationController(ILogger<OrganizationController> logger, IOrganizationRepository organizationRepository, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _organizationRepository = organizationRepository;
            _context = dbContext;
        }

        [HttpGet("GetOrganizations")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var result = _organizationRepository.GetOrganizations();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


        [HttpGet("OrganizationById/{id}", Name = "OrganizationById")]
        public async Task<IActionResult> OrganizationById(Guid id)
        {
            try
            {
                var result = _organizationRepository.GetOrganizationById(id);

                return (result != null) ? Ok(result) : NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ResponseDataTable<Organization>> Datatable(string? sort, string? page, string? per_page, string? filter)
        {
            var predict = PredicateBuilder.True<Organization>();
            predict = predict.And(p => p.IsActive == 1);

            if (!string.IsNullOrEmpty(filter))
            {
                filter = filter.ToLower();
                predict = (x) => (x.OrganizationName.ToLower().Contains(filter) || x.Email.ToLower().Contains(filter) || x.Contact.ToLower().Contains(filter));
            }
            return await LoadVuetifyTableAsync(_context.Organization, sort, page, per_page, predict);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrganization([FromBody] Organization organization)
        {
            try
            {
                if (organization == null)
                {
                    return BadRequest("Contact object is null");
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
                var res = await _context.Organization.FirstOrDefaultAsync(x => x.OrganizationName.ToLower().Equals(organization.OrganizationName.ToLower()));
                if (res != null)
                {

                    if(res.IsActive == 1)
                    {
                        return Conflict();
                    }
                    else
                    {
                        res.CustomerNumber = organization.CustomerNumber;
                        res.CustomerName = organization.CustomerName;
                        res.Email = organization.Email;
                        res.Contact = organization.Contact;
                        res.CreatedAt = DateTime.UtcNow;
                        res.CreatedBy = createdBy;
                        res.IsActive = 1;
                        _organizationRepository.UpdateOrganization(res);

                    }
                }
                else
                {
                    organization.CreatedAt = DateTime.UtcNow;
                    organization.CreatedBy = createdBy;
                    _organizationRepository.CreateOrganization(organization);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrganizatoin(Guid id, Organization organization)
        {
            try
            {
                if (organization == null)
                {
                    return BadRequest("object is null");
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
                var result = _organizationRepository.GetOrganizationById(id);

                if (result == null)
                {
                    return NotFound();
                }

                result.CustomerNumber = organization.CustomerNumber;
                result.OrganizationName = organization.OrganizationName;
                result.CustomerName = organization.CustomerName;
                result.Email = organization.Email;
                result.Contact = organization.Contact;
                result.CreatedAt = DateTime.UtcNow;
                result.CreatedBy = createdBy;

                _organizationRepository.UpdateOrganization(result);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrganization(Guid id)
        {
            try
            {
                var result = _organizationRepository.GetOrganizationById(id);

                if (result == null)
                {
                    return NotFound();
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
                result.IsActive = 0;
                result.CreatedAt = DateTime.UtcNow;
                result.CreatedBy = createdBy;
                _organizationRepository.UpdateOrganization(result);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}