using ERPService.Common.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPService.DataModel.DTO
{
   public class PurchaseOrdHist:BaseEntity
    {
        public Guid PurchaseOrderId { get; set; }
        public string FieldName { get; set; }
        public string PrevValue { get; set; }
        public string CurrentValue { get; set; }

        [NotMapped]
        public string UserName { get; set; }
    }
}
