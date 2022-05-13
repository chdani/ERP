using ERPService.Common.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPService.DataModel.DTO
{
    public class SystemSetting : BaseEntity
    {
        public string ConfigKey { get; set; }
        public string ConfigValue { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        [NotMapped]
        public string LangDescription { get; set; }
    }
}
