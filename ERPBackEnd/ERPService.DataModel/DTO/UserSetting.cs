using ERPService.Common.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPService.DataModel.DTO
{
    public class UserSetting : BaseEntity
    {
        public string ConfigKey { get; set; }
        public string ConfigValue { get; set; }
        public Guid UserMasterId { get; set; }
    }
}
