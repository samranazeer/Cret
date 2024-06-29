
namespace CRET.Domain.Models.DataModel
{
    public class QrImageModel
    {
        public string ToEmail { get; set; }
        public List<string> CertificateIds { get; set; }

        public QrImageModel()
        {
            this.CertificateIds = new List<string>();
        }
    }
}
