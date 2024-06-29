using CRET.Domain.Models.DataModel;
using System.Drawing;
using System.Net.Mail;


namespace CRET.Services.Interface
{
    public interface IQRCodeService
    {
        List<Attachment> GenerateQrCodeAttachments(List<QrCodeGenerateModel> lstQRCodeGenerateModel);

        byte[] GenerateQrCodeImageBytes(string qrCodeTextData);



    }
}
