using ERPService.Common.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERPService.DataModel.DTO
{
    public class PurchaseRequestDet : BaseEntity
    {
        public Guid PurchaseRequestId { get; set; }
        [AuditableAttribute("PR_PROD_NAME")]
        public Guid ProductMasterId { get; set; }
        [AuditableAttribute("PR_UNIT_NAME")]
        public Guid UnitMasterId { get; set; }
        [AuditableAttribute("PR_PRICE")]
        public decimal Price { get; set; }
        [AuditableAttribute("PR_QUANTITY")]
        public decimal Quantity { get; set; }
        [AuditableAttribute("PR_AMOUNT")]
        public decimal Amount { get; set; }
        [AuditableAttribute("APP_REMARKS")]
        public string Remarks { get; set; }
        [NotMapped]
        public string ProductName { get; set; }
        [NotMapped]
        public string UnitName { get; set; }
        [NotMapped]
        public string Remark { get; set; }
    }
}