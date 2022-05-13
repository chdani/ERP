using ERPService.Common;
using ERPService.Common.Shared;
using ERPService.DataModel.CTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPService.DataModel.DTO
{
    public class QuotationReqDet : BaseEntity
    {
        public Guid QuotationRequestId { get; set; }
        [AuditableAttribute("QUOTATION_REQUEST_PRODUCTMASTER")]
        public Guid ProductMasterId { get; set; }
        [AuditableAttribute("QUOTATION_REQUEST_QUANTITY", ERPConstants.CURRENCY_WITH_DECIMAL_FORMAT)]
        [PDFExport(headerText = "QUANTITY", order = 4, currencyFormat = ERPConstants.NUMBER_WITH_DECIMAL_FORMAT, width = 78, cellAlign = 2)]
        public decimal Quantity { get; set; }
        [AuditableAttribute("QUOTATION_REQUEST_REMARKS")]
        [PDFExport(headerText = "REMARKS", order = 5, width = 78, cellAlign = 2)]
        public string Remarks { get; set; }
        [AuditableAttribute("QUOTATION_REQUEST_UNITMASTER ")]
        public Guid UnitMasterId { get; set; }
        [NotMapped]
        [PDFExport(headerText = "PRODUCTMASTER", order = 2, width = 78, cellAlign = 2)]
        public string ProductName { get; set; }

        [NotMapped]
        [PDFExport(headerText = "UNITMASTER", order = 3, width = 78, cellAlign = 2)]
        public string UnitName { get; set; }

        [NotMapped]
        [PDFExport(headerText = "SNO", order = 1, width = 78, cellAlign = 2)]
        public int SNo { get; set; }
        [NotMapped]
        public string Remark { get; set; }
    }
}
