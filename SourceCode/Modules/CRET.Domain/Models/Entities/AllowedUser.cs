using CRET.Domain.Models.BaseClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRET.Domain.Models.Entities
{
    public class AllowedUser : BaseEntity
    {
        public string UserIdList { get; set; }
    }
}
