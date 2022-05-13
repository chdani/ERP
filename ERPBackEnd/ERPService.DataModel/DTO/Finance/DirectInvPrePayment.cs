using ERPService.Common.Shared;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using ERPService.DataModel.CTO;
using System.Collections.Generic;
using Microsoft.SqlServer.Server;

namespace ERPService.DataModel.DTO
{
    public class DirectInvPrePayment : BaseEntity
    {
        public string FinYear { get; set; }
        public Guid OrgId { get; set; }

        [PDFExport(headerText = "TRANSNO", order = 1, width = 50, cellAlign = 1)]
        [ExcelExport(headerText = "TRANSNO", order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public Int64 TransNo { get; set; }

        public Guid VendorMasterId { get; set; }


        [AuditableAttribute("DIRECT_INVOICE_INVNO")]
        public string InvoiceNo { get; set; }


        [AuditableAttribute("DIRECT_INVOICE_INVDATE")]
        public DateTime InvoiceDate { get; set; }

        [PDFExport(headerText = "DOCUMENTDATE", order = 4, width = 78, dateFormat = "dd-MM-yyyy", cellAlign = 1)]
        [ExcelExport(headerText = "DOCUMENTDATE", dateFormat = "dd-MM-yyyy", order = 4)]
        [AuditableAttribute("DIRECT_INVOICE_DOCDATE")]
        public DateTime DocumentDate { get; set; }

        [PDFExport(headerText = "LEDGERCODE", order = 5, width = 78, cellAlign = 1)]
        [ExcelExport(headerText = "LEDGERCODE", order = 5)]
        [AuditableAttribute("DIRECT_INVOICE_LEDGER")]
        public int LedgerCode { get; set; }

        [PDFExport(headerText = "COSTCENTERCODE", order = 6, width = 78, cellAlign = 1)]
        [ExcelExport(headerText = "COSTCENTERCODE", order = 6)]
        [AuditableAttribute("DIRECT_INVOICE_CC")]
        public string CostCenterCode { get; set; }

        [PDFExport(headerText = "AMOUNT", order = 7, width = 78, currencyFormat = "#,###", cellAlign = 1)]
        [ExcelExport(headerText = "AMOUNT", currencyFormat = "#,###", order = 7)]
        [AuditableAttribute("DIRECT_INVOICE_AMOUNT")]
        public decimal Amount { get; set; }
        [PDFExport(headerText = "DUEAMOUNT", order = 8, width = 78, currencyFormat = "#,###", cellAlign = 2)]
        [ExcelExport(headerText = "DUEAMOUNT", currencyFormat = "#,###", order = 8)]
        public decimal DueAmount { get; set; }

        [PDFExport(headerText = "REMARKS", order = 9, width = 78, cellAlign = 1)]
        [ExcelExport(headerText = "REMARKS", order = 9)]
        [AuditableAttribute("DIRECT_INVOICE_REMARKS")]
        public string Remarks { get; set; }
        public string Status { get; set; }
        public Guid PurchaseOrderId { get; set; }
        [PDFExport(headerText = "DOCUMENTNO", order = 3, width = 78, cellAlign = 1)]
        [ExcelExport(headerText = "DOCUMENTNO", order = 3)]
        public long DocumentNo { get; set; }
        public Guid? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        [AuditableAttribute("APPROVER_REMARKS")]
        public string ApproverRemarks { get; set; }
        [NotMapped]

        [PDFExport(headerText = "VENDORNAME", order = 2, width = 78, cellAlign = 1)]
        [ExcelExport(headerText = "VENDORNAME", order = 2)]
        public string VendorName { get; set; }

        [NotMapped]
        public List<AppDocument> AppDocuments { get; set; }
    }
}
