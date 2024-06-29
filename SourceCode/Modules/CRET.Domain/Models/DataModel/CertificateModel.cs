using CRET.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRET.Domain.Models.DataModel
{
    public class CertificateModel
    {
        public string OrganizationName { get; set; }
        public string SerialNo { get; set; }
        public byte[] Content { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public bool IsActive { get; set; }
        public CertificateType CertificateType { get; set; }
        public string IncidentNumber { get; set; }
    }
}
