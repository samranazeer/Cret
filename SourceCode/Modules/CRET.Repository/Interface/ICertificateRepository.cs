using CRET.Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRET.Repository.Interface
{
    public interface ICertificateRepository : IRepository<Certificate>
    {
        void CreateCertificate(Certificate certificate);
        void UpdateCertificate(Certificate certificate);
        void DeleteCertificate(Certificate certificate);
        List<Certificate> GetCertificates();
        Certificate GetCertificateById(Guid id);
        List<Certificate> GetCertificatesList(List<Guid> certificateIds);

    }
}
