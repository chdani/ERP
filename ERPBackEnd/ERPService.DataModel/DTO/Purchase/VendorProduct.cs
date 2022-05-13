using ERPService.Common.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPService.DataModel.DTO
{
   public class VendorProduct: BaseEntity
    {
        public Guid VendorMasterId { get; set; }
        public Guid ProductMasterId { get; set; }
        [NotMapped]
        public string ProductName { get; set; }


    }
}
