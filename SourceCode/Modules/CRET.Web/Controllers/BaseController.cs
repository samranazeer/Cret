using CRET.Domain.Models.DataModel;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

namespace CRET.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BaseController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        protected ApplicationUser ApplicationUser { get; set; }

        public BaseController()
        {

        }
        public BaseController(IConfiguration configuration) : base()
        {
            _configuration = configuration;
        }
        protected bool IsSuperAdmin()
        {
            bool isSuperAdmin = false;
            List<string> superAdmins = _configuration.GetSection("SuperAdmins").Get<List<string>>();
            // Extract User Email from the request header
            if (HttpContext.Request.Headers.TryGetValue("user-email", out var userEmailHeader))
            {
                string userEmail = userEmailHeader.ToString().ToLower();
                // Check if the User Email exists in the configured SuperAdmins
                bool isMatch = superAdmins.Any(s => s.ToLower() == userEmail);
                if (isMatch)
                {
                    isSuperAdmin = true;
                }
            }
            return isSuperAdmin;
        }
        protected async Task<ResponseDataTable<T>> LoadVuetifyTableAsync<T>(IQueryable<T> fetchData, string sort, string page, string per_page, Expression<Func<T, bool>> searchCriteria = null) where T : class
        {
            try
            {
                //paging Size(default = 10)
                int pageSize = string.IsNullOrEmpty(per_page) ? 10 : Convert.ToInt32(per_page);

                //current page(default = 1)
                int currentPage = string.IsNullOrEmpty(page) ? 1 : Convert.ToInt32(page);
                int recordsTotal = 0;

                // searchCriteria
                if (searchCriteria != null)
                {
                    fetchData = fetchData.Where(searchCriteria);
                }

                //total number of rows
                recordsTotal = await fetchData.CountAsync();

                //last page and last row in current page
                int lastPage = (int)Math.Ceiling(Convert.ToDecimal(recordsTotal) / Convert.ToDecimal(pageSize));
                int rowTo = Math.Min(currentPage * pageSize, recordsTotal);

                //number of rows to skip
                int skip = (currentPage - 1) * pageSize;

                var requestUrl = string.Format("{0}://{1}{2}?", Request.Scheme, Request.Host, Request.Path);

                // sorting
                if (!string.IsNullOrEmpty(sort))
                {
                    requestUrl = string.Format("{0}{1}&", requestUrl, sort);

                    var orderBy = sort.Replace("|", " ");
                    fetchData = fetchData.OrderBy(orderBy);
                }

                //paging
                if (pageSize > 0)
                {
                    fetchData = fetchData.Skip(skip).Take(pageSize);
                }
                var data = await fetchData.ToListAsync();

                var response = new ResponseDataTable<T>
                {
                    total = recordsTotal,
                    per_page = pageSize,
                    current_page = currentPage,
                    last_page = lastPage,
                    next_page_url = currentPage != lastPage ? String.Format("{0}page={1}", requestUrl, currentPage + 1) : null,
                    prev_page_url = currentPage != 1 ? String.Format("{0}page={1}", requestUrl, currentPage - 1) : null,
                    from = ((currentPage - 1) * pageSize) + 1,
                    to = rowTo,
                    data = data
                };

                return response;
            }
            catch (Exception ex)
            {
                return new ResponseDataTable<T>();
            }
        }

        public static string Print(byte[] Buffer) => string.Join(',', Buffer.Select(_ => $"{_:X2}"));
    }
}