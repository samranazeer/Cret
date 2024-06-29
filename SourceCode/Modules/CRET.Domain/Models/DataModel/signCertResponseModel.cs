using CRET.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRET.Domain.Models.DataModel
{
    public class signCertResponseModel
    {
        public string certificate { get; set; }
        public string serial_number { get; set; }
        public string response_format { get; set; }
    }
}
