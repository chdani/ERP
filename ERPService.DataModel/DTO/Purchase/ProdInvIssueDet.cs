using ERPService.Common;
using ERPService.Common.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPService.DataModel.DTO
{
    public class ProdInvIssueDet : BaseEntity
    {

        public Guid ProdInvIssueId { get; set; }

        [AuditableAttribute("SR_PROD_NAME")]
        public Guid ProductMasterId { get; set; }

        [AuditableAttribute("SR_UNIT_NAME")]
        public Guid UnitMasterId { get; set; }

        [AuditableAttribute("GRN_WH_LOCATION")]
        public Guid WareHouseLocationId { get; set; }

        [AuditableAttribute("GRN_QUANTITY", ERPConstants.NUMBER_FORMAT)]
        public decimal Quantity { get; set; }

        [AuditableAttribute("GRN_EXPIRYE_DATE")]
        public DateTime? ExpiryDate { get; set; }

        [AuditableAttribute("APP_REMARKS")]
        public string Remarks { get; set; }
        [NotMapped]
        public string ProductName { get; set; }
        [NotMapped]
        public string UnitName { get; set; }
        [NotMapped]
        public string WareHouse { get; set; }
        [NotMapped]
        public string Remark { get; set; }
        [NotMapped]
        public List<ProductInventory> Inventories { get; set; }
    }
}
