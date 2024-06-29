using CRET.Domain.Enum;
using CRET.Domain.Models.BaseClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRET.Domain.Models.Entities
{
    public class Setting : BaseEntity
    {
        public int ConsumerValidity { get; set; }
        public int ServiceValidity { get; set; }
        public int PulseValidity { get; set; }
        public int LabValidity { get; set; }
        public int ProductionValidity { get; set; }
        public int BatchValidity { get; set; }
        public int InsValidity { get; set; }
    }
}
