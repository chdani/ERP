using ERPService.Common.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPService.DataModel.DTO
{
    public class QuotaReqVendorDet : BaseEntity
    {
        public Guid VendorMasterId { get; set; }
        public Guid QuotationRequestId { get; set; }
        [NotMapped]
        public string Email { get; set; }
        [NotMapped]
        public string EmailId { get; set; }
        [NotMapped]
        public string VendorName { get; set; }

    }
}
