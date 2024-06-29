using CRET.DataAccessLayer;
using CRET.Domain.Enum;
using CRET.Domain.Models.DataModel;
using CRET.Domain.Models.Entities;
using CRET.Repository.Interface;
using CRET.Web.Utills;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using CRET.Domain.Helper;
using System.Text.Json;
using System.Net.Mail;
using System.Net.Mime;
using CRET.Domain.Constants;
using CRET.Services.Interface;
using Azure;
using System.Drawing;
using System.Net;
using System.Runtime.ConstrainedExecution;

namespace CRET.Controllers
{
    public class CertificateController : BaseController
    {
        private readonly ILogger<CertificateController> _logger;
        private readonly ICertificateRepository _certificateRepository;
        private readonly ICertificateService _certificateService;
        protected readonly IEmailService _emailService;
        protected readonly IQRCodeService _qRCodeService;

        ApplicationDbContext _context;


        public CertificateController(ILogger<CertificateController> logger, ICertificateRepository certificateRepository, ApplicationDbContext dbContext,
            IEmailService emailService, IQRCodeService qRCodeService, IConfiguration configuration, ICertificateService certificateService) : base(configuration)
        {
            _logger = logger;
            _context = dbContext;
            _emailService = emailService;
            _certificateRepository = certificateRepository;
            _qRCodeService = qRCodeService;
            _certificateService = certificateService;
        }

        [HttpGet("GetCertificates")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var result = _certificateRepository.GetCertificates();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


        [HttpGet("CertificateById/{id}", Name = "CertificateById")]
        public async Task<IActionResult> CertificateById(Guid id)
        {
            try
            {
                var result = _certificateRepository.GetCertificateById(id);

                return (result != null) ? Ok(result) : NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ResponseDataTable<Certificate>> Datatable(string? sort, string? page, string? per_page, string? filter, string? filterOptions)
        {
            var predict = PredicateBuilder.True<Certificate>();

            if (!string.IsNullOrEmpty(filterOptions))
            {
                var options = JsonSerializer.Deserialize<CertificateSearchOptions>(filterOptions);
                if (options != null)
                {

                    if (!string.IsNullOrEmpty(options.OrganizationName))
                    {
                        predict = predict.And(p => p.OrganizartionId.Equals(Guid.Parse(options.OrganizationName)));
                    }

                    if (options.CertificateLevel != null)
                    {
                        predict = predict.And(p => p.Level == options.CertificateLevel);
                    }

                    if (options.CertificateStatus != null)
                    {
                        predict = predict.And(p => p.CertificateStatus == options.CertificateStatus);
                    }

                    if (options.DateFrom.HasValue && options.DateTo.HasValue)
                    {
                        predict = predict.And(p => p.CreatedAt.Date >= options.DateFrom.Value.Date && p.CreatedAt.Date <= options.DateTo.Value.Date);

                    }
                    else if (options.DateFrom.HasValue)
                    {
                        predict = predict.And(p => p.CreatedAt.Date >= options.DateFrom.Value.Date);
                    }
                    else if (options.DateTo.HasValue)
                    {
                        predict = predict.And(p => p.CreatedAt.Date <= options.DateTo.Value.Date);

                    }
                }

            }


            if (!string.IsNullOrEmpty(filter))
            {
                filter = filter.ToLower();
                predict = (x) => x.Info.ToLower().Contains(filter);
            }
            return await LoadVuetifyTableAsync(_context.Certificate, sort, page, per_page, predict);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCertificate([FromBody] CsrRequestModel csrRequestModel)
        {
            try
            {
                if (csrRequestModel == null)
                {
                    return BadRequest("Contact object is null");
                }
                var createdBy = string.Empty;
                if (User != null)
                {
                    var claim = User.Claims.FirstOrDefault(c => c.Type == "name");
                    if (claim != null)
                    {
                        createdBy = claim.Value;
                    }
                }
                using (var dbContextTransaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        List<Certificate> certificatesToInsert = new List<Certificate>();

                        for (int i = 0; i < csrRequestModel.NumberOfCertificate; i++)
                        {
                            Guid certId = Guid.NewGuid();
                            Certificate certificate = new Certificate
                            {
                                Id = certId,
                                IncidentNo = certId.ToString().ToLower(),
                                OrganizartionId = csrRequestModel.OrganizartionId,
                                OrganizationName = csrRequestModel.OrganizationName,
                                Level = csrRequestModel.Level,
                                CertificateName = csrRequestModel.Level.GetDescription(),
                                Info = csrRequestModel.Info,
                                CreatedAt = DateTime.UtcNow,
                                CreatedBy = createdBy,
                                CertificateStatus = CertificateStatus.Created
                            };
                            certificatesToInsert.Add(certificate);
                        }

                        _context.Certificate.AddRange(certificatesToInsert);
                        _context.SaveChanges();

                        // Commit the transaction if all operations succeed
                        dbContextTransaction.Commit();

                        return Ok();
                    }
                    catch (Exception ex)
                    {
                        // Roll back the transaction if any exception occurs
                        dbContextTransaction.Rollback();
                        return BadRequest(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }
        [HttpPost("UpdateCertificateStatus")]
        public async Task<IActionResult> UpdateCertificateStatus([FromBody] CertificateStatusRequestModel certificateStatusRequest)
        {

            try
            {
                var res = _certificateRepository.GetCertificateById(new Guid(certificateStatusRequest.CertificateId));
                var createdBy = string.Empty;
                if (User != null)
                {
                    var claim = User.Claims.FirstOrDefault(c => c.Type == "name");
                    if (claim != null)
                    {
                        createdBy = claim.Value;
                    }
                }
                if (res != null)
                {
                    res.CertificateStatus = certificateStatusRequest.CertificateStatus;
                    res.CreatedAt = DateTime.UtcNow;
                    res.CreatedBy = createdBy;
                    _certificateRepository.Update(res);
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCertificate(Guid id)
        {
            try
            {
                var result = _certificateRepository.GetCertificateById(id);

                if (result == null)
                {
                    return NotFound();
                }
                var createdBy = string.Empty;
                if (User != null)
                {
                    var claim = User.Claims.FirstOrDefault(c => c.Type == "name");
                    if (claim != null)
                    {
                        createdBy = claim.Value;
                    }
                }
                result.CreatedAt = DateTime.UtcNow;
                result.CreatedBy = createdBy;
                _certificateRepository.DeleteCertificate(result);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [HttpPost("ImportCsr", Name = "ImportCsr")]
        public async Task<IActionResult> ImportCsr([FromBody] ImportFileModel csrContent)
        {
            try
            {
                var createdBy = string.Empty;
                if (User != null)
                {
                    var claim = User.Claims.FirstOrDefault(c => c.Type == "name");
                    if (claim != null)
                    {
                        createdBy = claim.Value;
                    }
                }
                var apiKey = "af1fb532-7151-43f3-8a5b-171b456e1636";
                bool certificateGenerated = false;
                string importCertificateError = string.Empty;
                var CSRInPEMFormat=  Utilities.ExtractCSR(csrContent.Content);
                if (string.IsNullOrEmpty(CSRInPEMFormat))
                {
                    throw new Exception("Error Parsing CSR");
                }
                var pemReader = new PemReader(new StringReader(csrContent.Content));
                var csrObject = pemReader.ReadObject();
                if (csrObject is Pkcs10CertificationRequest csr)
                {
                    var subject = csr.GetCertificationRequestInfo().Subject.ToString();
                    var subjectArr = subject.Split('\u002C');
                    var organizationName = subjectArr.Where(_ => _.Trim().ToLower().StartsWith("o=")).FirstOrDefault();
                    var organizationUnit = subjectArr.Where(_ => _.Trim().ToLower().StartsWith("ou=")).FirstOrDefault();

                    organizationUnit = organizationUnit?.Trim().Replace("OU=", string.Empty).Replace("ou=", string.Empty);
                    organizationUnit = string.IsNullOrEmpty(organizationUnit) ? string.Empty : organizationUnit.ToLower();

                    organizationName = organizationName?.Trim().Replace("O=", string.Empty).Replace("o=", string.Empty);
                    organizationName = string.IsNullOrEmpty(organizationName) ? string.Empty : organizationName.Replace("\\", "");

                    var incidentNo = subjectArr.Where(_ => _.Trim().ToLower().StartsWith("uid=")).FirstOrDefault();
                    incidentNo = incidentNo?.Trim().Replace("UID=", string.Empty).Replace("uid=", string.Empty);

                    incidentNo = string.IsNullOrEmpty(incidentNo) ? string.Empty : incidentNo.ToLower();
                    var tag = subjectArr.Where(_ => _.Trim().ToLower().StartsWith("t=")).FirstOrDefault();
                    tag = tag?.Trim().Replace("T=", string.Empty).Replace("t=", string.Empty);
                    CertificateType? certificateLevel = null;
                    if (Enum.TryParse<CertificateType>(organizationUnit, true, out CertificateType result))
                    {
                        certificateLevel = result;
                    }

                    var certificate = _certificateRepository.Get(x => x.IncidentNo.ToLower().Equals(incidentNo)).FirstOrDefault();
                    if (certificate == null)
                    {
                        return NotFound("Certificate token not found for this CSR.");

                    }

                    if (!(certificateLevel != null && certificate.Level == certificateLevel.Value))
                    {
                        return NotFound("Certificate level does not match with CSR Token.");
                    }

                    if (!(certificate.OrganizationName.Equals(organizationName)))
                    {
                        return NotFound("Organization Name does not match with CSR Token.");

                    }
                    var settings = _certificateService.GetValiditySetting();
                    int validity = 0;
                    switch (certificateLevel)
                    {
                        case CertificateType.Con:
                            validity = settings.ConsumerValidity;
                            break;
                        case CertificateType.Ser:
                            validity = settings.ServiceValidity;
                            break;
                        case CertificateType.Pul:
                            validity = settings.PulseValidity;
                            break;
                        case CertificateType.Lab:
                            validity = settings.LabValidity;
                            break;
                        case CertificateType.Pro:
                            validity = settings.ProductionValidity;
                            break;
                        case CertificateType.Bat:
                            validity = settings.BatchValidity;
                            break;
                        case CertificateType.Ins:
                            validity = settings.InsValidity;
                            break;
                        default:
                            validity = 730;
                            break;

                    }
                    var signCertRequest = new signCertRequestModel()
                    {
                        csr = csrContent.Content,
                        user_profile = organizationUnit,
                        duration_days = validity,
                        uuid = incidentNo,
                    };
                    var signCertResponse = new signCertResponseModel();
                    var requestData = JsonSerializer.Serialize(signCertRequest);
                    var formData = new Dictionary<string, string>
                        {
                            { "api-key", apiKey },
                            { "data", requestData }
                        };
                    using (var client = new HttpClient())
                    using (var requestContent = new MultipartFormDataContent())
                    {
                        foreach (var kvp in formData)
                        {
                            requestContent.Add(new StringContent(kvp.Value), kvp.Key);
                        }

                        try
                        {
                            var response = await client.PostAsync("https://cret.landisgyr.net:8080/signCert", requestContent);

                            if (response.IsSuccessStatusCode)
                            {
                                var responseContent = await response.Content.ReadAsStringAsync();
                                _logger.LogInformation("Response Content:");
                                _logger.LogInformation(responseContent);
                                signCertResponse = JsonSerializer.Deserialize<signCertResponseModel>(responseContent);
                                if (signCertResponse != null)
                                {
                                    const string beginTag = "-----BEGIN CERTIFICATE-----";
                                    const string endTag = "-----END CERTIFICATE-----";
                                    signCertResponse.certificate = $"{beginTag}\n{signCertResponse.certificate}\n{endTag}";
                                    certificateGenerated = true;
                                }
                            }
                            else
                            {
                                importCertificateError = await response.Content.ReadAsStringAsync();
                                throw new Exception(importCertificateError);
                            }
                        }
                        catch (Exception ex)
                        {
                            importCertificateError = "PKI system is currently unavailable";
                            _logger.LogError($"Exception while Generating Certificate: {ex.Message}");
                        }
                    }

                    if (certificate.CertificateStatus != CertificateStatus.CertificateCreated)
                    {
                        if (certificateGenerated && signCertResponse != null)
                        {
                            certificate.CertificateStatus = CertificateStatus.CertificateCreated;
                            certificate.CertificateContent = signCertResponse.certificate;
                            var cert = new X509Certificate2(Encoding.ASCII.GetBytes(certificate.CertificateContent));
                            certificate.ActivationDate = cert.NotBefore;
                            certificate.ExpirationDate = cert.NotAfter;
                        }
                        else
                        {

                            certificate.CertificateStatus = CertificateStatus.CsrReceived;
                            //Update Error Property
                            certificate.ImportCSRError = importCertificateError;
                        }
                        certificate.Tag = tag;
                        certificate.CsrContent = csrContent.Content;
                        certificate.CreatedAt = DateTime.UtcNow;
                        certificate.CreatedBy = createdBy;
                        _certificateRepository.Update(certificate);
                    }
                    if (string.IsNullOrEmpty(importCertificateError))
                    {
                        return Ok();
                    }
                    else
                    {
                        return NotFound(importCertificateError);
                    }

                }
                else
                {
                    return NotFound("Imported file contains an invalid CSR");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error parsing CSR: " + ex.Message);
                return BadRequest(ex);
            }
        }
        [HttpPost("ImportCertificate", Name = "ImportCertificate")]
        public async Task<IActionResult> ImportCertificate([FromBody] ImportFileModel csrContent)
        {
            try
            {
                string importCertificateError = string.Empty;
                _logger.LogInformation($"CSR Content: {csrContent.Content}");
                string certInPEMFormat = Utilities.ExtractCertificate(csrContent.Content);
                _logger.LogInformation($"certInPEMFormat: {certInPEMFormat}");
                var cert = new X509Certificate2(Encoding.ASCII.GetBytes(certInPEMFormat));
                string incidentNumberKey = "oid.0.9.2342.19200300.100.1.1=";
                var subject = cert.Subject;
                _logger.LogInformation($"Certificate Subject: {cert.Subject}");
                var subjectArr = subject.Split('\u002C');
                var incidentNo = subjectArr.Where(_ => _.Trim().ToLower().StartsWith(incidentNumberKey)).FirstOrDefault();
                if (string.IsNullOrEmpty(incidentNo))
                {
                    incidentNumberKey = "userid=";
                    incidentNo = subjectArr.Where(_ => _.Trim().ToLower().StartsWith(incidentNumberKey)).FirstOrDefault();
                }
                if (string.IsNullOrEmpty(incidentNo))
                {
                    incidentNumberKey = "uid=";
                    incidentNo = subjectArr.Where(_ => _.Trim().ToLower().StartsWith(incidentNumberKey)).FirstOrDefault();
                }
                if (string.IsNullOrEmpty(incidentNo))
                {
                    return NotFound("Could not Extract Incident Number from Certificate");
                }
                incidentNo = incidentNo.ToLower().Trim();
                incidentNo = incidentNo.Replace(incidentNumberKey, string.Empty);
                var organizationName = subjectArr.Where(_ => _.Trim().ToLower().StartsWith("o=")).FirstOrDefault();
                var organizationUnit = subjectArr.Where(_ => _.Trim().ToLower().StartsWith("ou=")).FirstOrDefault();
                organizationUnit = organizationUnit?.Trim().Replace("OU=", string.Empty).Replace("ou=", string.Empty);
                organizationUnit = string.IsNullOrEmpty(organizationUnit) ? string.Empty : organizationUnit.ToLower();

                organizationName = organizationName?.Trim().Replace("O=", string.Empty).Replace("o=", string.Empty);
                organizationName = string.IsNullOrEmpty(organizationName) ? string.Empty : organizationName.Replace("\\", "");

                var tag = subjectArr.Where(_ => _.Trim().ToLower().StartsWith("t=")).FirstOrDefault();
                tag = tag?.Trim().Replace("T=", string.Empty).Replace("t=", string.Empty);
                var createdBy = string.Empty;
                if (User != null)
                {
                    var claim = User.Claims.FirstOrDefault(c => c.Type == "name");
                    if (claim != null)
                    {
                        createdBy = claim.Value;
                    }
                }
                _logger.LogInformation("Getting Certificate from database");
                _logger.LogInformation($"Incidence Number: {incidentNo}");
                //var certificate = _certificateRepository.Get(x => x.IncidentNo.ToLower().Equals(incidentNo)).FirstOrDefault();
                CertificateType? certificateLevel = null;
                if (Enum.TryParse<CertificateType>(organizationUnit, true, out CertificateType result))
                {
                    certificateLevel = result;
                }

                var certificate = _certificateRepository.Get(x => x.IncidentNo.ToLower().Equals(incidentNo)).FirstOrDefault();
                if (certificate == null)
                {
                    return NotFound("Certificate token not found for this CSR.");

                }

                if (!(certificateLevel != null && certificate.Level == certificateLevel.Value))
                {
                    return NotFound("Certificate level does not match with CSR Token.");
                }
                var orgNameInDB = $"\"{certificate.OrganizationName}\"";
                if (!(orgNameInDB.Equals(organizationName)))
                {
                    return NotFound("Certificate level does not match with CSR Token.");

                }

                certificate.CertificateStatus = CertificateStatus.CertificateCreated;
                certificate.CertificateContent = csrContent.Content;
                certificate.Tag = tag;
                certificate.CreatedAt = DateTime.UtcNow;
                certificate.CreatedBy = createdBy;
                certificate.ActivationDate = cert.NotBefore;
                certificate.ExpirationDate = cert.NotAfter;
                _certificateRepository.Update(certificate);

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error parsing Certificate: " + ex.Message);
                return BadRequest(ex);
            }
        }

        [HttpGet("DownloadQrCode/{id}", Name = "DownloadQrCode")]
        public async Task<IActionResult> DownloadQrCode(Guid id)
        {
            try
            {
                var certificate = _certificateRepository.GetCertificateById(id);

                var createdBy = string.Empty;
                if (User != null)
                {
                    var claim = User.Claims.FirstOrDefault(c => c.Type == "name");
                    if (claim != null)
                    {
                        createdBy = claim.Value;
                    }
                }
                if (certificate != null)
                {
                    var subject = $"n={certificate.IncidentNo};ou={certificate.Level.ToString()};o={certificate.OrganizationName}";
                    var qrCodeBytes = _qRCodeService.GenerateQrCodeImageBytes(subject);
                    if (certificate.CertificateStatus != CertificateStatus.CsrReceived && certificate.CertificateStatus != CertificateStatus.CertificateCreated)
                    {
                        certificate.CertificateStatus = CertificateStatus.Assigned;
                        certificate.CreatedAt = DateTime.UtcNow;
                        certificate.CreatedBy = createdBy;
                        _certificateRepository.UpdateCertificate(certificate);
                    }
                    return File(qrCodeBytes, "image/png", $"{certificate.OrganizationName}_{certificate.Level.ToString()}_{certificate.IncidentNo}.png");
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("SendQrCode", Name = "SendQrCode")]
        public async Task<IActionResult> SendQrCode(QrImageModel qrCodeModel)
        {
            try
            {
                _logger.LogInformation("SendQrCode as attachment of email: " + qrCodeModel.ToEmail);
                var createdBy = string.Empty;
                if (User != null)
                {
                    var claim = User.Claims.FirstOrDefault(c => c.Type == "name");
                    if (claim != null)
                    {
                        createdBy = claim.Value;
                    }
                }
                await _certificateService.SendCSRTokenAsQRCode(qrCodeModel, createdBy);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error While Sending QRCode as attachment in Email");
                _logger.LogError(ex.Message);
                return BadRequest(ex);
            }
        }
        [NonAction]
        public CertificateModel GetCertificateDetail(X509Certificate2 certificate)
        {
            CertificateModel certificateEntity = null;

            if (certificate != null)
            {
                if (!string.IsNullOrEmpty(certificate.Subject))
                {
                    var subjectArr = certificate.Subject.Split('\u002C');
                    var n = subjectArr.Where(_ => _.Trim().ToLower().StartsWith("oid.0.9.2342.19200300.100.1.1=")).FirstOrDefault();
                    var organizationName = subjectArr.Where(_ => _.Trim().ToLower().StartsWith("o=")).FirstOrDefault();
                    var organizationUnit = subjectArr.Where(_ => _.Trim().ToLower().StartsWith("ou=")).FirstOrDefault();
                    if (!string.IsNullOrEmpty(organizationUnit) && !string.IsNullOrEmpty(organizationName))
                    {
                        n = n?.Trim().Replace("oid.0.9.2342.19200300.100.1.1=", string.Empty);
                        n = n?.Trim().Replace("OID.0.9.2342.19200300.100.1.1=", string.Empty);
                        if (n == null)
                        {
                            n = string.Empty;
                        }
                        organizationUnit = organizationUnit.Trim().Replace("ou=", string.Empty);
                        organizationUnit = organizationUnit.Trim().Replace("OU=", string.Empty);
                        organizationName = organizationName?.Trim().Replace("o=", string.Empty);
                        organizationName = organizationName?.Trim().Replace("O=", string.Empty);
                        object certType;
                        if (Enum.TryParse(typeof(CertificateType), organizationUnit, true, out certType))
                        {
                            byte[] derCert = certificate.Export(X509ContentType.Cert);
                            CertificateType certificateType = (CertificateType)certType;
                            certificateEntity = new CertificateModel
                            {
                                SerialNo = certificate.SerialNumber,
                                OrganizationName = organizationName,
                                Content = derCert,
                                CertificateType = certificateType,
                                ValidFrom = certificate.NotBefore,
                                ValidTo = certificate.NotAfter,
                                IncidentNumber = n
                            };
                        }
                    }
                }
            }
            return certificateEntity;
        }

        [HttpGet("DownloadCertificate/{id}", Name = "DownloadCertificate")]
        public async Task<IActionResult> DownloadCertificate(Guid id)
        {
            try
            {
                var certificate = _certificateRepository.GetCertificateById(id);

                if (certificate != null && !string.IsNullOrEmpty(certificate.CertificateContent))
                {
                    var fileName = $"{certificate.OrganizationName}_{certificate.Level.ToString()}_{certificate.Tag}.png";
                    var qrCodeBytes = _qRCodeService.GenerateQrCodeImageBytes(certificate.CertificateContent);

                    return File(qrCodeBytes, "image/png", fileName);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("SignCertificate/{id}", Name = "SignCertificate")]
        public async Task<IActionResult> SignCertificate(Guid id)
        {
            try
            {

                var apiKey = "af1fb532-7151-43f3-8a5b-171b456e1636";
                bool certificateGenerated = false;
                string importCertificateError = string.Empty;

                var certificate = _certificateRepository.Get(x => x.IncidentNo.ToLower().Equals(id.ToString().ToLower())).FirstOrDefault();
                if (certificate != null)
                {
                    var createdBy = string.Empty;
                    if (User != null)
                    {
                        var claim = User.Claims.FirstOrDefault(c => c.Type == "name");
                        if (claim != null)
                        {
                            createdBy = claim.Value;
                        }
                    }
                    if (certificate.CertificateStatus == CertificateStatus.CsrReceived && !string.IsNullOrEmpty(certificate.CsrContent))
                    {
                        var settings = _certificateService.GetValiditySetting();
                        int validity = 0;
                        switch (certificate.Level)
                        {
                            case CertificateType.Con:
                                validity = settings.ConsumerValidity;
                                break;
                            case CertificateType.Ser:
                                validity = settings.ServiceValidity;
                                break;
                            case CertificateType.Pul:
                                validity = settings.PulseValidity;
                                break;
                            case CertificateType.Lab:
                                validity = settings.LabValidity;
                                break;
                            case CertificateType.Pro:
                                validity = settings.ProductionValidity;
                                break;
                            case CertificateType.Bat:
                                validity = settings.BatchValidity;
                                break;
                            case CertificateType.Ins:
                                validity = settings.InsValidity;
                                break;
                            default:
                                validity = 730;
                                break;

                        }
                        var signCertRequest = new signCertRequestModel()
                        {
                            csr = certificate.CsrContent,
                            user_profile = certificate.Level.ToString(),
                            duration_days = validity,
                            uuid = certificate.IncidentNo,
                        };
                        var signCertResponse = new signCertResponseModel();
                        var requestData = JsonSerializer.Serialize(signCertRequest);
                        var formData = new Dictionary<string, string>
                        {
                            { "api-key", apiKey },
                            { "data", requestData }
                        };
                        using (var client = new HttpClient())
                        using (var requestContent = new MultipartFormDataContent())
                        {
                            foreach (var kvp in formData)
                            {
                                requestContent.Add(new StringContent(kvp.Value), kvp.Key);
                            }

                            try
                            {
                                var response = await client.PostAsync("https://cret.landisgyr.net:8080/signCert", requestContent);

                                if (response.IsSuccessStatusCode)
                                {
                                    var responseContent = await response.Content.ReadAsStringAsync();
                                    _logger.LogInformation("Response Content:");
                                    _logger.LogInformation(responseContent);
                                    signCertResponse = JsonSerializer.Deserialize<signCertResponseModel>(responseContent);
                                    if (signCertResponse != null)
                                    {
                                        const string beginTag = "-----BEGIN CERTIFICATE-----";
                                        const string endTag = "-----END CERTIFICATE-----";
                                        signCertResponse.certificate = $"{beginTag}\n{signCertResponse.certificate}\n{endTag}";
                                        certificateGenerated = true;
                                    }
                                }
                                else
                                {
                                    importCertificateError = await response.Content.ReadAsStringAsync();
                                    throw new Exception(importCertificateError);
                                }
                            }
                            catch (Exception ex)
                            {
                                importCertificateError = ex.Message;
                                _logger.LogError($"Exception while Generating Certificate: {ex.Message}");
                            }
                        }

                        if (certificate.CertificateStatus != CertificateStatus.CertificateCreated)
                        {
                            if (certificateGenerated && signCertResponse != null)
                            {
                                certificate.CertificateStatus = CertificateStatus.CertificateCreated;
                                certificate.CertificateContent = signCertResponse.certificate;
                            }
                            else
                            {

                                certificate.CertificateStatus = CertificateStatus.CsrReceived;
                                //Update Error Property
                                certificate.ImportCSRError = importCertificateError;
                            }
                            certificate.CreatedBy = createdBy;
                            certificate.CreatedAt = DateTime.UtcNow;
                            _certificateRepository.Update(certificate);
                        }
                        if (string.IsNullOrEmpty(importCertificateError))
                        {
                            return Ok();
                        }
                        else
                        {
                            return NotFound(importCertificateError);
                        }
                    }
                    else
                    {
                        return NotFound("Please Import CSR against this Certificate Token for Signing.");
                    }
                }
                else
                {
                    return NotFound("Certificate token not found for this CSR.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error parsing CSR: " + ex.Message);
                return BadRequest(ex);
            }
        }

        [HttpGet("UpdateActivationDates")]
        public async Task<IActionResult> UpdateActivationDates()
        {
            try
            {
                var certificates = _certificateService.GetAllCertificates();
                int total = certificates.Count;
                int processed = 0;
                X509Certificate2 x509Certificate = null;
                foreach (var certificate in certificates)
                {
                    if (certificate != null && !string.IsNullOrEmpty(certificate.CertificateContent))
                    {
                        x509Certificate = new X509Certificate2(Encoding.ASCII.GetBytes(certificate.CertificateContent));
                        if (x509Certificate != null)
                        {
                            certificate.ActivationDate = x509Certificate.NotBefore;
                            certificate.ExpirationDate = x509Certificate.NotAfter;
                            processed++;
                        }
                    }
                }
                _certificateRepository.UpdateRange(certificates);
                return Ok($"Total Certificates: {total} Processed Certificates: {processed}");
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
