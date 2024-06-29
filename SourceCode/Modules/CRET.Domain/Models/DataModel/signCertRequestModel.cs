using CRET.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRET.Domain.Models.DataModel
{
    public class signCertRequestModel
    {
        public string csr { get; set; }
        public int duration_days { get; set; }
        public string user_profile { get; set; }
        public string uuid { get; set; }
    }
}
