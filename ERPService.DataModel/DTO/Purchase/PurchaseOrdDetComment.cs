using ERPService.Common.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPService.DataModel.DTO
{
   public class PurchaseOrdDetComment:BaseEntity
    {
        public Guid PurchaseOrderDetId { get; set; }
        public string Comments { get; set; }
        [NotMapped]
        public string UserName { get; set; }

        [NotMapped]
        public List<AppDocument> AppDocuments { get; set; }
    }
}
