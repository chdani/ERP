using ERPService.Common;
using ERPService.Common.Shared;
using ERPService.DataModel.CTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPService.DataModel.DTO
{
    public class PurchaseRequest : BaseEntity
    {
        [PDFExport(headerText = "TRANSNO", order = 1, width = 50, cellAlign = 1)]
        [ExcelExport(headerText = "TRANSNO", order = 1)]
        public string TransNo { get; set; }
        public long SeqNo { get; set; }

        [PDFExport(headerText = "TRANSDATE", order = 2, width = 50, dateFormat = "dd-MM-yyyy", cellAlign = 1)]
        [ExcelExport(headerText = "TRANSDATE", order = 2)]
        public DateTime TransDate { get; set; }

        [AuditableAttribute("PR_VENDORQUOTATION")]
        public Guid VendorQuotationId { get; set; }
        [AuditableAttribute("PR_VENDORMASTER")]
        public Guid VendorMasterId { get; set; }
        [AuditableAttribute("APP_REMARKS")]
        [PDFExport(headerText = "REMARKS", order = 5, width = 50, cellAlign = 1)]
        [ExcelExport(headerText = "REMARKS", order = 5)]
        public string Remarks { get; set; }
        public string Status { get; set; }
        [NotMapped]
        public List<PurchaseRequestDet> PurchaseRequestDetList { get; set; }
        [NotMapped]
        public string ExportType { get; set; }
        [NotMapped]
        public string ExportHeaderText { get; set; }
        [NotMapped]
        public long VendorQuotationNo { get; set; }
        [NotMapped]
        public DateTime FromTransDate { get; set; }
        [NotMapped]
        public DateTime ToTransDate { get; set; }
        [NotMapped]
        public string ApproverRemarks { get; set; }
        [NotMapped]
        [PDFExport(headerText = "VENDORQUOTATION", order = 3, width = 50, cellAlign = 1)]
        [ExcelExport(headerText = "VENDORQUOTATION", order = 3)]
        public string VendorQuotation { get; set; }
        [NotMapped]
        [PDFExport(headerText = "VENDORMASTER", order = 4, width = 50, cellAlign = 1)]
        [ExcelExport(headerText = "VENDORMASTER", order = 4)]
        public string VendorMaster { get; set; }
        [NotMapped]
        [PDFExport(subHeaderText = "PRODUCTMASTER", order = 1, width = 50, cellAlign = 1)]
        [ExcelExport(subHeaderText = "PRODUCTMASTER", order = 1)]
        public string ProductName { get; set; }
        [NotMapped]
        [PDFExport(subHeaderText = "UNITMASTER", order = 2, width = 50, cellAlign = 1)]
        [ExcelExport(subHeaderText = "UNITMASTER", order = 2)]
        public string UnitName { get; set; }
        [NotMapped]
        [PDFExport(subHeaderText = "PRICE", order = 3, currencyFormat = ERPConstants.NUMBER_WITH_DECIMAL_FORMAT, width = 50, cellAlign = 1)]
        [ExcelExport(subHeaderText = "PRICE", order = 3)]
        public decimal Price { get; set; }
        [NotMapped]
        [PDFExport(subHeaderText = "QUANTITY", order = 4, currencyFormat = ERPConstants.NUMBER_WITH_DECIMAL_FORMAT, width = 50, cellAlign = 1)]
        [ExcelExport(subHeaderText = "QUANTITY", order = 4)]
        public decimal Quantity { get; set; }
        [NotMapped]
        [PDFExport(subHeaderText = "AMOUNT", order = 5, currencyFormat = ERPConstants.NUMBER_WITH_DECIMAL_FORMAT, width = 50, cellAlign = 1)]
        [ExcelExport(subHeaderText = "AMOUNT", order = 5)]
        public decimal Amount { get; set; }

        [NotMapped]
        [PDFExport(subHeaderText = "REMARKS", order = 6, width = 50, cellAlign = 1)]
        [ExcelExport(subHeaderText = "REMARKS", order = 6)]
        public string Remark { get; set; }

    }
}
