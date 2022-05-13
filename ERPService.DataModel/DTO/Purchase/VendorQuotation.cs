using ERPService.Common;
using ERPService.Common.Shared;
using ERPService.DataModel.CTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ERPService.DataModel.DTO
{
    public class VendorQuotation : BaseEntity
    {

        [AuditableAttribute("QUOTATION_REQUEST_TRANS_NO")]
        public Guid QuotationRequestId { get; set; }

        [AuditableAttribute("VENDOR_QUOTATION_VENDOR_NAME")]
        public Guid VendorMasterId { get; set; }

        [PDFExport(headerText = "VENDOR_QUO_TRNASNO", order = 1, width = 50, cellAlign = 1)]
        [ExcelExport(headerText = "VENDOR_QUO_TRNASNO", order = 1)]
        public string TransNo { get; set; }
        public long SeqNo { get; set; }

        public DateTime TransDate { get; set; }

        [AuditableAttribute("VENDOR_QUOTATION_NO")]
        [PDFExport(headerText = "VENDOR_QUO_REFNO", order = 2, width = 50, cellAlign = 1)]
        [ExcelExport(headerText = "VENDOR_QUO_REFNO", order = 2)]
        public string QuotationNo { get; set; }


        [AuditableAttribute("VENDOR_QUOTATION_DATE")]
        [PDFExport(headerText = "VENDOR_QUO_DATE", order = 3, width = 50, dateFormat = "dd-MM-yyyy", cellAlign = 1)]
        [ExcelExport(headerText = "VENDOR_QUO_DATE", order = 3, dateFormat = "dd-MM-yyyy")]
        public DateTime QuotationdDate { get; set; }

        [AuditableAttribute("VENDOR_QUOTATION_APPROVAL")]
        public bool IsApproved { get; set; }
        public string Status { get; set; }

        [AuditableAttribute("VENDOR_QUOTATION_REMARKS")]
        [PDFExport(headerText = "REMARKS", order = 6, width = 50, cellAlign = 1)]
        [ExcelExport(headerText = "REMARKS", order = 6)]
        public string Remarks { get; set; }


        [NotMapped]
        public List<VendorQuotationDet> vendorquotationDets { get; set; }

        [NotMapped]
        public QuotationRequest quotationRequest { get; set; }

        [NotMapped]
        [PDFExport(headerText = "VENDOR_REQ_TRANSNO", order = 4, width = 50, cellAlign = 1)]
        [ExcelExport(headerText = "VENDOR_REQ_TRANSNO", order = 4)]
        public string QuotationReqTransNo { get; set; }
        [NotMapped]
        [PDFExport(headerText = "VENDOR_NAME", order = 5, width = 50, cellAlign = 1)]
        [ExcelExport(headerText = "VENDOR_NAME", order = 5)]
        public string VendorName { get; set; }

        [NotMapped]
        public List<QuotationReqDet> quotationDets { get; set; }
        [NotMapped]
        public List<QuotaReqVendorDet> quotationReqVendordet { get; set; }


        [NotMapped]
        public VendorMaster vendaorData { get; set; }


        [NotMapped]
        public List<AppDocument> AppDocuments { get; set; }
        [NotMapped]
        public string ApproverRemarks { get; set; }
        [NotMapped]
        public string ExportType { get; set; }
        [NotMapped]
        public string ExportHeaderText { get; set; }
        [NotMapped]
        [PDFExport(subHeaderText = "PRODUCT_NAME", order = 1, width = 50, cellAlign = 1)]
        [ExcelExport(subHeaderText = "PRODUCT_NAME", order = 1)]
        public string ProductName { get; set; }
        [NotMapped]
        [PDFExport(subHeaderText = "UNITMASTER", order = 2, width = 50, cellAlign = 1)]
        [ExcelExport(subHeaderText = "UNITMASTER", order = 2)]
        public string UnitName { get; set; }
        [NotMapped]
        [PDFExport(subHeaderText = "QUANTITY", order = 3, width = 50, currencyFormat = ERPConstants.NUMBER_WITH_DECIMAL_FORMAT, cellAlign = 1)]
        [ExcelExport(subHeaderText = "QUANTITY", currencyFormat = ERPConstants.NUMBER_WITH_DECIMAL_FORMAT, order = 3)]
        public decimal? Quantity { get; set; }
        [NotMapped]
        [PDFExport(subHeaderText = "PRICE", order = 4, width = 50, currencyFormat = ERPConstants.NUMBER_WITH_DECIMAL_FORMAT, cellAlign = 1)]
        [ExcelExport(subHeaderText = "PRICE", currencyFormat = ERPConstants.NUMBER_WITH_DECIMAL_FORMAT, order = 4)]
        public decimal? Price { get; set; }
        [NotMapped]
        [PDFExport(subHeaderText = "AMOUNT", order = 5, currencyFormat = ERPConstants.NUMBER_WITH_DECIMAL_FORMAT, width = 50, cellAlign = 1)]
        [ExcelExport(subHeaderText = "AMOUNT", currencyFormat = ERPConstants.NUMBER_WITH_DECIMAL_FORMAT, order = 5)]
        public decimal? Amount { get; set; }
        [NotMapped]
        [PDFExport(subHeaderText = "REMARKS", order = 6, width = 50, cellAlign = 1)]
        [ExcelExport(subHeaderText = "REMARKS", order = 6)]
        public string Remark { get; set; }











    }
}
