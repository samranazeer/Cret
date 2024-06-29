
using CRET.Domain.Models.DataModel;
using System.Net.Mail;

namespace CRET.Services.Interface
{
    public interface IEmailService
    {
        Task SendEmailAsync(QrImageModel qrCodeModel);
        Task SendEmailAsync(string subject, string body, string toEmail, List<Attachment> attachments, bool isBodyHTML);
    }
}
