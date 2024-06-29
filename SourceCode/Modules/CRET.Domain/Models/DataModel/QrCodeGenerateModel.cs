
namespace CRET.Domain.Models.DataModel
{
    public class QrCodeGenerateModel
    {
        public string QRCodeContent { get; set; }
        public string fileName { get; set; }

        public QrCodeGenerateModel()
        {
            QRCodeContent = string.Empty;
            fileName = string.Empty;
        }
    }
}
