using ERPService.Common;
using ERPService.Common.Shared;
using ERPService.DataModel.CTO;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPService.DataModel.DTO
{
    public class PurchaseOrderDet : BaseEntity
    {

        public Guid PurchaseOrderId { get; set; }
        [AuditableAttribute("PURCHASE_PROD_NAME")]
        public Guid ProductMasterId { get; set; }
        [AuditableAttribute("PURCHASE_UNIT_NAME")]
        public Guid UnitMasterId { get; set; }
        [AuditableAttribute("PURCHASE_PRICE", ERPConstants.CURRENCY_WITH_DECIMAL_FORMAT)]
        [PDFExport(headerText = "PRICE", order = 5, currencyFormat = ERPConstants.CURRENCY_WITH_DECIMAL_FORMAT, width = 78, cellAlign = 2)]
        public decimal Price { get; set; }
        [AuditableAttribute("PURCHASE_QUANTITY", ERPConstants.CURRENCY_WITH_DECIMAL_FORMAT)]
        [PDFExport(headerText = "QUANTITY", order = 4, currencyFormat = ERPConstants.CURRENCY_WITH_DECIMAL_FORMAT, width = 78, cellAlign = 2)]
        public decimal Quantity { get; set; }
        [AuditableAttribute("PURCHASE_AMOUNT", ERPConstants.CURRENCY_WITH_DECIMAL_FORMAT)]
        [PDFExport(headerText = "AMOUNT", order = 6, currencyFormat = ERPConstants.CURRENCY_WITH_DECIMAL_FORMAT, width = 78, cellAlign = 2)]
        public decimal Amount { get; set; }
        [AuditableAttribute("APP_REMARKS")]
        [PDFExport(headerText = "REMARKS", order = 7, width = 78, cellAlign = 2)]
        public string Remarks { get; set; }
        [NotMapped]
        [PDFExport(headerText = "PRODUCTMASTER", order = 2, width = 78, cellAlign = 2)]
        public string ProductName { get; set; }
        [NotMapped]
        [PDFExport(headerText = "UNITMASTER", order = 3, width = 78, cellAlign = 2)]
        public string UnitName { get; set; }
        [NotMapped]
        public string Remark { get; set; }
        [NotMapped]
        [PDFExport(headerText = "SNO", order = 1, width = 78, cellAlign = 2)]
        public int SNo { get; set; }
    }
}
