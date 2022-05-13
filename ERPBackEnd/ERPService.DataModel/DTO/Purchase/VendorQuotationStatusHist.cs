using ERPService.Common.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPService.DataModel.DTO
{
   public class VendorQuotationStatusHist : BaseEntity
    {
        public Guid VendorQuotationId { get; set; }
        public string Status { get; set; }
        public string Comments { get; set; }
        [NotMapped]
        public string UserName { get; set; }

    }
}
