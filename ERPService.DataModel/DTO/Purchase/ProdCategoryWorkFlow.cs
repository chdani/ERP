
using ERPService.Common.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPService.DataModel.DTO
{
    public class ProdCategoryWorkFlow : BaseEntity
    {
        public string ApprovalType { get; set; }
        public int ApprovalLevel { get; set; }
        public Guid ProdCategoryId { get; set; }
        public Guid ApprovalId { get; set; }
    }
}
