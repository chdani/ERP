using ERPService.Common.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPService.DataModel.DTO
{
    public class ProductCategory : BaseEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int ApprovalLevels { get; set; }    
        
        [NotMapped]
      public List<ProdCategoryWorkFlow> approvalWorkFlow { get; set; }
        [NotMapped]
        public List<ProdSubCategory> prodsubCategory { get; set; }
    }
}
