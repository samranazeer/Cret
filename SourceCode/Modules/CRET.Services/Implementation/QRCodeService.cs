using CRET.Domain.Models.DataModel;
using CRET.Domain.Models.Entities;
using CRET.Repository.Interface;
using CRET.Services.Interface;
using QRCoder;
using System.Drawing;
using System.Net.Mail;
using System.Net.Mime;

namespace CRET.Services.Implementation
{
    public class QRCodeService : IQRCodeService
    {
        private readonly ICertificateRepository _certificateRepository;

        public QRCodeService(ICertificateRepository certificateRepository)
        {
            _certificateRepository = certificateRepository;
        }



        public List<Attachment> GenerateQrCodeAttachments(List<QrCodeGenerateModel> lstQRCodeGenerateModel)
        {
            List<Attachment> lstAttachments = new List<Attachment>();
            foreach (QrCodeGenerateModel QRCodeGenerateModel in lstQRCodeGenerateModel)
            {
                if (!string.IsNullOrEmpty(QRCodeGenerateModel.QRCodeContent))
                {
                    var qrCodeData = GenerateQrCodeData(QRCodeGenerateModel.QRCodeContent);
                    var qrCodeImage = GenerateQrCodeImage(qrCodeData);

                    var imageBytes = ImageToByteArray(qrCodeImage);
                    //var fileName = $"{certificate.IncidentNo}.png";
                    var fileName = QRCodeGenerateModel.fileName;

                    var attachment = new Attachment(new MemoryStream(imageBytes), fileName, "image/png");
                    lstAttachments.Add(attachment);
                }
            }

            return lstAttachments;
        }


        public byte[] GenerateQrCodeImageBytes(string qrCodeTextData)
        {
            var qrCodeData = GenerateQrCodeData(qrCodeTextData);
            var qrCodeImage = GenerateQrCodeImage(qrCodeData);
            var imageBytes = ImageToByteArray(qrCodeImage);

            return imageBytes;
        }
        private QRCodeData GenerateQrCodeData(string qrCodeTextData)
        {
            var qrGenerator = new QRCodeGenerator();
            return qrGenerator.CreateQrCode(qrCodeTextData, QRCodeGenerator.ECCLevel.M);
        }

        private Bitmap GenerateQrCodeImage(QRCodeData qrCodeData)
        {
            var qrCode = new QRCode(qrCodeData);
            return qrCode.GetGraphic(20, "#000000", "#ffffff", true);
        }

        private byte[] ImageToByteArray(Bitmap qrCodeImage)
        {
            using (var stream = new MemoryStream())
            {
                qrCodeImage.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }
    }
}
