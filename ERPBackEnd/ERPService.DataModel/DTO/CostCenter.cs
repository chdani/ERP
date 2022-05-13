using ERPService.Common.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPService.DataModel.DTO
{
    public class CostCenter : BaseEntity
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
