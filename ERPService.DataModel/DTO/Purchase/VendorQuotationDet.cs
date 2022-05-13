using ERPService.Common.Shared;
using ERPService.DataModel.CTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPService.DataModel.DTO 
{
    public class VendorQuotationDet : BaseEntity
    {
      
        public Guid VendorQuotationId { get; set; }

        [AuditableAttribute("PRODUCT_NAME")]
        public Guid ProductMasterId { get; set; }

        [AuditableAttribute("PRODUCT_UNIT_NAME")]
        public Guid UnitMasterId { get; set; }

        [AuditableAttribute("PRODUCT_PRICE")]
        public decimal? Price { get; set; }

        [AuditableAttribute("PRODUCT_QUANTITY")]
        public decimal? Quantity { get; set; }

        [AuditableAttribute("PRODUCT_AMOUNT")]
        public decimal? Amount { get; set; }

        [AuditableAttribute("PRODUCT_REMARKS")]
        public string Remarks { get; set; }

        [NotMapped]
        public string ProductName { get; set; }
        [NotMapped]
        public string Remark { get; set; }
        [NotMapped]
        public string UnitName { get; set; }
        [NotMapped]
        public ProductMaster productDetail { get;set; }
        [NotMapped]
        public ProdUnitMaster prodUnit { get;set; }


    }
}
