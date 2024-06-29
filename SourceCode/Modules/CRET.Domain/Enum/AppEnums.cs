
using System.ComponentModel;

namespace CRET.Domain.Enum
{
    public enum CertificateType
    {
        [Description("Consumer")]
        Con,
        [Description("Service")]
        Ser,
        [Description("Pulse")]
        Pul,
        [Description("Laboratory")]
        Lab,
        [Description("Production")]
        Pro,
        [Description("Batch")]
        Bat,
        [Description("Installer")]
        Ins,
        
    }

    public enum CertificateStatus
    {
        Created = 1,
        Assigned,
        CsrReceived,
        CertificateCreated
    }
}
