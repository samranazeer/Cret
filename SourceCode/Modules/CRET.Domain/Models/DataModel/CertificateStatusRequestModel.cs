
using CRET.Domain.Enum;

namespace CRET.Domain.Models.DataModel
{
    public class CertificateStatusRequestModel
    {
        public string CertificateId { get; set; }
        public CertificateStatus CertificateStatus { get; set; }

    }
}
