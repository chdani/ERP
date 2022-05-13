using ERPService.Common.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ERPService.DataModel.DTO
{
   public class VendorContact: BaseEntity
    {
        public Guid VendorMasterId { get; set; }
        public string EmailId { get; set; }
        public string MobileNo { get; set; }
        public string ContactName { get; set; }
    }
}
