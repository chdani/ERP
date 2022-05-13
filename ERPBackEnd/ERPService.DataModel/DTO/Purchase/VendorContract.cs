using ERPService.Common.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ERPService.DataModel.DTO
{
   public class VendorContract : BaseEntity
    {
        public Guid VendorMasterId { get; set; }
        public int Duration { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string PaymentTerm { get; set; }
        public int LedgerCode { get; set; }
        public decimal AmountToHold { get; set; }
        public string Description { get; set; }
        [NotMapped]
        public List<AppDocument> AppDocuments { get; set; }
    }
}
