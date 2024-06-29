using CRET.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRET.Domain.Models.DataModel
{
    public class CsrRequestModel
    {
        public int NumberOfCertificate { get; set; }
        public Guid OrganizartionId { get; set; }
        public string OrganizationName { get; set; }
        public CertificateType Level { get; set; }
        public string CreatedBy { get; set; }
        public string Info { get; set; }
    }
}
