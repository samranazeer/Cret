using CRET.DataAccessLayer;
using CRET.Domain.Models.Entities;
using CRET.Repository.Interface;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRET.Repository.Implementation
{
    public class CertificateRepository : BaseRepository<Certificate>, ICertificateRepository
    {
        private readonly ILogger _logger;
        public CertificateRepository(ApplicationDbContext dbContext, ILogger<CertificateRepository> logger) : base(dbContext)
        {
            _logger = logger;
        }

        public void CreateCertificate(Certificate certificate)
        {
            try
            {
                Insert(certificate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        public Certificate GetCertificateById(Guid id)
        {
            Certificate result = null;
            try
            {
                result = GetByID(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return result;
        }

        public List<Certificate> GetCertificates()
        {
            List<Certificate> result = null;
            try
            {
                result = Get().ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return result;
        }

        public void UpdateCertificate(Certificate certificate)
        {
            try
            {
                Update(certificate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        public void DeleteCertificate(Certificate certificate)
        {
            try
            {
                Delete(certificate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        public List<Certificate> GetCertificatesList(List<Guid> certificateIds)
        {
            List<Certificate> result = null;
            try
            {
                result = Get(w => certificateIds.Contains(w.Id)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return result;
        }

    }
}
