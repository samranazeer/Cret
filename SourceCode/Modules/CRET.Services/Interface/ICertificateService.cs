using CRET.Domain.Models.DataModel;
using CRET.Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRET.Services.Interface
{
    public interface ICertificateService
    {
        List<Certificate> GetCertificatesList(List<string> certificateIds);
        List<Certificate> GetCertificatesList(List<Guid> certificateIds);
        Task SendCSRTokenAsQRCode(QrImageModel qrCodeModel, string createdBy);
        Setting GetValiditySetting();
        List<Certificate> GetAllCertificates();
    }
}
