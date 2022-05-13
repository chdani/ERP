using ERPService.Common;
using ERPService.Common.Shared;
using ERPService.DataModel.CTO;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPService.DataModel.DTO
{
    public class GoodsRecNoteDet : BaseEntity
    {

        public Guid GoodsReceiptNoteId { get; set; }

        [AuditableAttribute("SR_PROD_NAME")]
        public Guid ProductMasterId { get; set; }

        public string ShelveNo { get; set; }

        public string Barcode { get; set; }


        [AuditableAttribute("SR_UNIT_NAME")]
        public Guid UnitMasterId { get; set; }


        [AuditableAttribute("GRN_PRICE", ERPConstants.NUMBER_WITH_DECIMAL_FORMAT)]
        [PDFExport(headerText = "PRICE", order = 6, currencyFormat = ERPConstants.CURRENCY_WITH_DECIMAL_FORMAT, width = 78, cellAlign = 2)]
        public decimal Price { get; set; }

        [AuditableAttribute("GRN_QUANTITY", ERPConstants.NUMBER_FORMAT)]
        [PDFExport(headerText = "QUANTITY", order = 5, currencyFormat = ERPConstants.CURRENCY_WITH_DECIMAL_FORMAT, width = 78, cellAlign = 2)]
        public decimal Quantity { get; set; }

        [AuditableAttribute("GRN_AMOUNT", ERPConstants.NUMBER_WITH_DECIMAL_FORMAT)]
        [PDFExport(headerText = "AMOUNT", order = 7, currencyFormat = ERPConstants.CURRENCY_WITH_DECIMAL_FORMAT, width = 78, cellAlign = 2)]
        public decimal Amount { get; set; }

        [AuditableAttribute("GRN_EXPIRYE_DATE")]
        public DateTime? ExpiryDate { get; set; }

        [AuditableAttribute("APP_REMARKS")]
        [PDFExport(headerText = "REMARKS", order = 8, width = 78, cellAlign = 2)]
        public string Remarks { get; set; }
        [NotMapped]
        [PDFExport(headerText = "PRODUCTMASTER", order = 3, width = 78, cellAlign = 2)]
        public string ProductName { get; set; }
        [NotMapped]
        [PDFExport(headerText = "UNITMASTER", order = 4, width = 78, cellAlign = 2)]
        public string UnitName { get; set; }
        [NotMapped]
        [PDFExport(headerText = "WAREHOUSE", order = 2, width = 78, cellAlign = 2)]
        public string WareHouseLocation { get; set; }
        [NotMapped]
        public string Remark { get; set; }
        [NotMapped]
        [PDFExport(headerText = "SNO", order = 1, width = 78, cellAlign = 2)]
        public int SNo { get; set; }
    }
}
