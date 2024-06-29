using CRET.Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRET.Repository.Interface
{
    public interface ISettingRepository : IRepository<Setting>
    {
        Setting GetValiditySetting();
        void AddUpdateValiditySettings(Setting settings);
    }
}
