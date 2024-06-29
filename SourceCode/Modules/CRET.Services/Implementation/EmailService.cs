using CRET.Domain.Models.DataModel;
using CRET.Domain.Models.Entities;
using CRET.Repository.Interface;
using CRET.Services.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using QRCoder;
using System.Drawing;
using System.Net;
using System.Net.Mail;

namespace CRET.Services.Implementation
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ICertificateRepository _certificateRepository;
        private readonly ILogger<EmailService> _logger;
        public EmailService(ILogger<EmailService> logger, IConfiguration configuration, ICertificateRepository certificateRepository)
        {
            _logger = logger;
            _config = configuration;
            _certificateRepository = certificateRepository;
        }

        public async Task SendEmailAsync(QrImageModel qrCodeModel)
        {
            MailMessage mailMessage = new MailMessage();
            // Example in C#
            var qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = null;
            QRCode qrCode = null;
            Bitmap qrCodeImage = null;
            byte[] imageBytes;
            try
            {
                var fromEmail = _config["EmailSettings:From"];
                var host = _config["EmailSettings:SmtpServer"];
                var password = _config["EmailSettings:Password"];
                var port = _config["EmailSettings:SmtpPort"];
                List<Attachment> attachments = new List<Attachment>();
                using var smtpclient = new SmtpClient();
                var credential = new NetworkCredential
                {
                    UserName = fromEmail,
                    Password = password
                };

                smtpclient.Port = int.Parse(port);
                smtpclient.EnableSsl = true;
                smtpclient.UseDefaultCredentials = false;
                smtpclient.Credentials = credential;
                smtpclient.Host = host;
                smtpclient.DeliveryMethod = SmtpDeliveryMethod.Network;
                if (qrCodeModel != null)
                {
                    var body = "QR Code is Attached.";
                    if (qrCodeModel.CertificateIds.Count > 1)
                    {
                        body = "QR Codes are Attached.";
                    }
                    mailMessage.From = new MailAddress(fromEmail);
                    mailMessage.To.Add(new MailAddress(qrCodeModel.ToEmail));
                    mailMessage.Subject = "QR Code";
                    mailMessage.Body = body;

                    List<Guid> ids = new List<Guid>();
                    foreach (string certificateId in qrCodeModel.CertificateIds)
                    {
                        ids.Add(Guid.Parse(certificateId));
                    }
                    var certificates = _certificateRepository.GetCertificatesList(ids);
                    string subject = string.Empty;
                    foreach (Certificate certificate in certificates)
                    {
                        subject = $"n={certificate.IncidentNo};ou={certificate.Level.ToString()};o={certificate.OrganizationName}";

                        qrCodeData = qrGenerator.CreateQrCode(subject, QRCodeGenerator.ECCLevel.M);
                        qrCode = new QRCode(qrCodeData);
                        qrCodeImage = qrCode.GetGraphic(20, "#000000", "#ffffff", true); // Adjust size, dark color, light color


                        using (MemoryStream stream = new MemoryStream())
                        {
                            qrCodeImage.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                            imageBytes = stream.ToArray();
                        }

                        //string base64String = qrImage.Base64Image
                        string fileName = certificate.IncidentNo + ".png";

                        // Create an Attachment from the MemoryStream
                        Attachment attachment = new Attachment(new MemoryStream(imageBytes), fileName, "image/png");

                        // Add the attachment to the MailMessage
                        mailMessage.Attachments.Add(attachment);
                    }

                    await smtpclient.SendMailAsync(mailMessage);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                qrGenerator!.Dispose();
                qrCodeData!.Dispose();
                qrCode!.Dispose();
                qrCodeImage!.Dispose();
                foreach (var attachment in mailMessage.Attachments)
                {
                    attachment.Dispose();
                }
            }
        }
        public async Task SendEmailAsync(string subject, string body, string toEmail, List<Attachment> attachments, bool isBodyHTML)
        {
            try
            {
                if (attachments != null && attachments.Any())
                {
                    var fromEmail = _config["EmailSettings:From"];
                    var host = _config["EmailSettings:SmtpServer"];
                    var password = _config["EmailSettings:Password"];
                    var port = _config["EmailSettings:SmtpPort"];
                    bool enableSSL = Convert.ToBoolean(_config["EmailSettings:EnableSSL"]);

                    using var smtpclient = new SmtpClient
                    {
                        Port = int.Parse(port),
                        EnableSsl = enableSSL,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential
                        {
                            UserName = fromEmail,
                            Password = password
                        },
                        Host = host,
                        DeliveryMethod = SmtpDeliveryMethod.Network
                    };

                    var mailMessage = CreateMailMessage(subject, body, fromEmail, toEmail, attachments, isBodyHTML);

                    await smtpclient.SendMailAsync(mailMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }

        private MailMessage CreateMailMessage(string subject, string body, string fromEmail, string toEmail, List<Attachment> attachments, bool isBodyHTML)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail),
                To = { new MailAddress(toEmail) },
                Subject = subject,
                Body = body,
                IsBodyHtml = isBodyHTML,
            };

            foreach (var attachment in attachments)
            {
                mailMessage.Attachments.Add(attachment);
            }

            return mailMessage;
        }
    }
}
