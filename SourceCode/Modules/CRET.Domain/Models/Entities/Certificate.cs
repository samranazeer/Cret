using CRET.Domain.Enum;
using CRET.Domain.Models.BaseClass;


namespace CRET.Domain.Models.Entities
{
    public class Certificate : BaseEntity
    {
        public string IncidentNo { get; set; }
        public Guid OrganizartionId { get; set; }
        public string OrganizationName { get; set; }
        public string CertificateName { get; set; }
        public CertificateType Level { get; set; }
        public CertificateStatus CertificateStatus { get; set; }
        public string? Tag { get; set; }
        public string? CsrContent { get; set; }
        public string? CertificateContent { get; set; }
        public string Info { get; set; }
        public string? ImportCSRError { get; set; }
        public DateTime ActivationDate { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
