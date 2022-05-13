using ERPService.Common.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPService.DataModel.DTO
{
   public class PurchaseOrdDetHist:BaseEntity
    {
        public Guid PurchaseOrderDetId { get; set; }
        public string FieldName { get; set; }
        public string PrevValue { get; set; }
        public string CurrentValue { get; set; }

        [NotMapped]
        public string UserName { get; set; }
    }
}
