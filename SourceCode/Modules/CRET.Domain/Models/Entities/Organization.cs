using CRET.Domain.Models.BaseClass;

namespace CRET.Domain.Models.Entities
{
    public class Organization : BaseEntity
    {
        public string CustomerNumber { get; set; }
        public string CustomerName { get; set; }
        public string OrganizationName { get; set; }
        public string Email { get; set; }
        public string Contact { get; set; }
        public int IsActive { get; set; }
    }
}
