using CRET.Domain.Constants;
using CRET.Domain.Enum;
using CRET.Domain.Models.DataModel;
using CRET.Domain.Models.Entities;
using CRET.Repository.Interface;
using CRET.Services.Interface;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace CRET.Services.Implementation
{
    public class CertificateService : ICertificateService
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly IQRCodeService _qRCodeService;
        private readonly ILogger<CertificateService> _logger;
        private readonly IEmailService _emailService;
        private readonly ISettingRepository _settingRepository;
        public CertificateService(ICertificateRepository certificateRepository, IQRCodeService qRCodeService, ILogger<CertificateService> logger, IEmailService emailService, ISettingRepository settingRepository)
        {
            _certificateRepository = certificateRepository;
            _qRCodeService = qRCodeService;
            _logger = logger;
            _emailService = emailService;
            _settingRepository = settingRepository;
        }

        public List<Certificate> GetCertificatesList(List<string> certificateIds)
        {
            List<Guid> ids = new List<Guid>();
            foreach (string certificateId in certificateIds)
            {
                ids.Add(Guid.Parse(certificateId));
            }

            return GetCertificatesList(ids);
        }

        public List<Certificate> GetCertificatesList(List<Guid> certificateIds)
        {
            var lstCertificates = _certificateRepository.GetCertificatesList(certificateIds);
            return lstCertificates;
        }

        public Setting GetValiditySetting()
        {
            return _settingRepository.GetValiditySetting();
        }

        public async Task SendCSRTokenAsQRCode(QrImageModel qrCodeModel, string createdBy)
        {
            var certificates = GetCertificatesList(qrCodeModel.CertificateIds);
            List<QrCodeGenerateModel> lstQrCodeGenerateModel = new List<QrCodeGenerateModel>();
            QrCodeGenerateModel qrCodeGenerateModel = new QrCodeGenerateModel();
            bool isCertificateCreated = false;
            if (certificates != null && certificates.Count > 0)
            {
                var cert = certificates.FirstOrDefault();
                if (cert != null && cert.CertificateStatus == Domain.Enum.CertificateStatus.CertificateCreated)
                {
                    isCertificateCreated = true;
                }

                foreach (Certificate certificate in certificates)
                {
                    qrCodeGenerateModel = new QrCodeGenerateModel();
                    if (certificate != null)
                    {
                        string subject = string.Empty;
                        if (isCertificateCreated && !string.IsNullOrEmpty(certificate.CertificateContent))
                        {
                            subject = certificate.CertificateContent;
                            qrCodeGenerateModel.QRCodeContent = subject;
                            qrCodeGenerateModel.fileName = $"{certificate.OrganizationName}_{certificate.Level}_{certificate.Tag}.png";
                            lstQrCodeGenerateModel.Add(qrCodeGenerateModel);
                        }
                        else
                        {
                            subject = $"n={certificate.IncidentNo};ou={certificate.Level.ToString()};o={certificate.OrganizationName}";
                            qrCodeGenerateModel.QRCodeContent = subject;
                            qrCodeGenerateModel.fileName = $"{certificate.IncidentNo}.png";
                            lstQrCodeGenerateModel.Add(qrCodeGenerateModel);
                        }

                    }
                }
            }
            var lstAttachments = _qRCodeService.GenerateQrCodeAttachments(lstQrCodeGenerateModel);
            Attachment instrauctionManual = new Attachment("Files/How to handle certificates_01.pdf", MediaTypeNames.Application.Pdf);
            lstAttachments.Add(instrauctionManual);
            _logger.LogInformation("QR Codes have been Generated");

            string emailBody = string.Empty;
            string emailSubject = string.Empty;
            if (isCertificateCreated)
            {
                emailSubject = "Certificate";
                emailBody = Constants.EmailBodyForCertificate;
            }
            else
            {
                emailSubject = "CSR Token";
                emailBody = Constants.EmailBodyForCSR;
            }
            await _emailService.SendEmailAsync(emailSubject, emailBody, qrCodeModel.ToEmail, lstAttachments, true);
            _logger.LogInformation("QR Codes have been sent as email attachment");
            foreach (var certificate in certificates)
            {
                if (certificate != null)
                {
                    if (certificate.CertificateStatus != CertificateStatus.CsrReceived && certificate.CertificateStatus != CertificateStatus.CertificateCreated)
                    {
                        certificate.CreatedAt = DateTime.UtcNow;
                        certificate.CreatedBy = createdBy;
                        certificate.CertificateStatus = CertificateStatus.Assigned;
                        _certificateRepository.UpdateCertificate(certificate);
                    }
                }
            }
        }

        public List<Certificate> GetAllCertificates()
        {
            var certificates = _certificateRepository.GetCertificates();
            certificates= certificates.Where(w=>w.CertificateStatus == CertificateStatus.CertificateCreated).ToList();
            return certificates;
        }
    }
}
