using ERPService.Common.Shared;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using ERPService.DataModel.CTO;
using System.Collections.Generic;
using ERPService.Common;

namespace ERPService.DataModel.DTO
{
    public class DirectInvPostPayment : BaseEntity
    {
        [PDFExport(headerText = "TRANSNO", order = 1, width = 50, cellAlign = 1)]
        [ExcelExport(headerText = "TRANSNO", order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public Int64 TransNo { get; set; }
        [AuditableAttribute("DIRPOST_FIN_YEAR")]
        public string FinYear { get; set; }

        [AuditableAttribute("CASH_TRANSFER_ORG")]
        public Guid OrgId { get; set; }
        public Guid VendorMasterId { get; set; }
        public Guid DirInvPrePaymentId { get; set; }
        [PDFExport(headerText = "INVOICENO", order = 3, width = 50, cellAlign = 1)]
        [ExcelExport(headerText = "INVOICENO", order = 3)]
        [AuditableAttribute("DIRECT_INVOICE_INVNO")]
        public string InvoiceNo { get; set; }

        [PDFExport(headerText = "INVOICEDATE", order = 4, width = 78, dateFormat = "dd-MM-yyyy", cellAlign = 1)]
        [ExcelExport(headerText = "INVOICEDATE", dateFormat = "dd-MM-yyyy", order = 4)]
        [AuditableAttribute("DIRECT_INVOICE_INVDATE", ERPConstants.DATE_FORMAT)]
        public DateTime InvoiceDate { get; set; }

        [PDFExport(headerText = "DOCUMENTDATE", order = 5, width = 78, dateFormat = "dd-MM-yyyy", cellAlign = 1)]
        [ExcelExport(headerText = "DOCUMENTDATE", dateFormat = "dd-MM-yyyy", order = 5)]
        [AuditableAttribute("DIRECT_INVOICE_DOCDATE", ERPConstants.DATE_FORMAT)]
        public DateTime DocumentDate { get; set; }
        [PDFExport(headerText = "LEDGERCODE", order = 6, width = 50, cellAlign = 1)]
        [ExcelExport(headerText = "LEDGERCODE", order = 6)]
        [AuditableAttribute("DIRECT_INVOICE_LEDGER")]
        public int LedgerCode { get; set; }
        [PDFExport(headerText = "COSTCENTERCODE", order = 7, width = 50, cellAlign = 1)]
        [ExcelExport(headerText = "COSTCENTERCODE", order = 7)]
        [AuditableAttribute("DIRECT_INVOICE_CC")]
        public string CostCenterCode { get; set; }
        [PDFExport(headerText = "AMOUNT", order = 8, width = 50, currencyFormat = "#,###", cellAlign = 1)]
        [ExcelExport(headerText = "AMOUNT", currencyFormat = "#,###", order = 8)]
        [AuditableAttribute("DIRECT_INVOICE_AMOUNT", ERPConstants.CURRENCY_FORMAT)]
        public decimal Amount { get; set; }
        [PDFExport(headerText = "REMARKS", order = 9, width = 50, cellAlign = 1)]
        [ExcelExport(headerText = "REMARKS", order = 9)]
        [AuditableAttribute("DIRECT_INVOICE_REMARKS")]
        public string Remarks { get; set; }
        [AuditableAttribute("APP_STATUS")]
        public string Status { get; set; }
        public Guid? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        [AuditableAttribute("APPROVER_REMARKS")]
        public string ApproverRemarks { get; set; }
        [NotMapped]
        [PDFExport(headerText = "VENDORNAME", order = 2, width = 50, cellAlign = 1)]
        [ExcelExport(headerText = "VENDORNAME", order = 2)]
        public string VendorName { get; set; }
        [NotMapped]
        public List<AppDocument> AppDocuments { get; set; }

    }
}
