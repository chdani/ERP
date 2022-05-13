using ERPService.Common;
using ERPService.Common.Shared;
using ERPService.DataModel.CTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPService.DataModel.DTO
{
    public class QuotationRequest : BaseEntity
    {
        [PDFExport(headerText = "TRANSNO", order = 1, width = 50, cellAlign = 1)]
        [ExcelExport(headerText = "TRANSNO", order = 1)]
        public string TransNo { get; set; }
        public long SeqNo { get; set; }
        [AuditableAttribute("QUOTATION_REQUEST_TRANSDATE", ERPConstants.DATE_FORMAT)]
        [PDFExport(headerText = "TRANSDATE", order = 2, width = 50, dateFormat = "dd-MM-yyyy", cellAlign = 1)]
        [ExcelExport(headerText = "TRANSDATE", order = 2, dateFormat = "dd-MM-yyyy")]
        public DateTime TransDate { get; set; }
        [AuditableAttribute("QUOTATION_REQUEST_REMARKS")]
        [PDFExport(headerText = "REMARKS", order = 3, width = 50, cellAlign = 1)]
        [ExcelExport(headerText = "REMARKS", order = 3)]
        public string Remarks { get; set; }
        public string Status { get; set; }

        [NotMapped]
        public string ApproverRemarks { get; set; }

        [NotMapped]
        public List<QuotationReqDet> QuotationReqDet { get; set; }
        [NotMapped]
        public List<QuotaReqVendorDet> QuotaReqVendorDets { get; set; }

        [NotMapped]
        [PDFExport(subHeaderText = "PRODUCTMASTER", order = 1, width = 50, cellAlign = 1)]
        [ExcelExport(subHeaderText = "PRODUCTMASTER", order = 1)]
        public string ProductName { get; set; }
        [NotMapped]
        [PDFExport(subHeaderText = "UNITMASTER", order = 2, width = 50, cellAlign = 1)]
        [ExcelExport(subHeaderText = "UNITMASTER", order = 2)]
        public string UnitName { get; set; }
        [NotMapped]
        [PDFExport(subHeaderText = "QUANTITY", order = 3, currencyFormat = ERPConstants.NUMBER_WITH_DECIMAL_FORMAT, width = 50, cellAlign = 1)]
        [ExcelExport(subHeaderText = "QUANTITY", order = 3, currencyFormat = ERPConstants.NUMBER_WITH_DECIMAL_FORMAT)]
        public decimal Quantity { get; set; }
        [NotMapped]
        [PDFExport(subHeaderText = "REMARKS", order = 3, width = 50, cellAlign = 1)]
        [ExcelExport(subHeaderText = "REMARKS", order = 3)]
        public string Remark { get; set; }

    }
}
