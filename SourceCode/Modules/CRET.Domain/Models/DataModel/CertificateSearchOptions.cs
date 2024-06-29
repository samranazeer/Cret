
using CRET.Domain.Enum;
using System.Text.Json.Serialization;

namespace CRET.Domain.Models.DataModel
{
    public class CertificateSearchOptions
    {
        [JsonPropertyName("organizationName")]
        public string? OrganizationName { get; set; }

        [JsonPropertyName("certificateLevel")]
        public CertificateType? CertificateLevel { get; set; }

        [JsonPropertyName("certificateStatus")]
        public CertificateStatus? CertificateStatus { get; set; }
        [JsonPropertyName("startDate")]
        public DateTime? DateFrom { get; set; }

        [JsonPropertyName("endDate")]
        public DateTime? DateTo { get; set; }
    }
}

