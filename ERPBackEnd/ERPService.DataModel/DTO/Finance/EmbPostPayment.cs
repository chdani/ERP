using ERPService.Common;
using ERPService.Common.Shared;
using ERPService.DataModel.CTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using static ERPService.Common.Shared.AuditUtility;

namespace ERPService.DataModel.DTO
{
    public class EmbPostPayment : BaseEntity
    {

        [PDFExport(headerText = "TRANSNO", order = 1, width = 75, cellAlign = 2)]
        [ExcelExport(headerText = "TRANSNO",  order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long TransNo { get; set; }
        public Guid EmbassyId { get; set; }
        public Guid OrgId { get; set; }
        public string FinYear { get; set; }

        [PDFExport(headerText = "PAYMENTDATE", dateFormat = "dd-MM-yyyy", order = 3, width = 75, cellAlign = 2)]
        [ExcelExport(headerText = "PAYMENTDATE", dateFormat = "dd-MM-yyyy", order = 3)]
        [AuditableAttribute("EMB_PAYMENT_PAYDATE", ERPConstants.DATE_FORMAT)]
        public DateTime PaymentDate { get; set; }


        [PDFExport(headerText = "BOOKNO", order = 4, width = 75, cellAlign = 2)]
        [ExcelExport(headerText = "BOOKNO", order = 4)]
        [AuditableAttribute("EMB_PAYMENT_BOOK_NO")]
        public string BookNo { get; set; }

        [AuditableAttribute("CASHMGMT_LEDGER")]
        public Int32 LedgerCode { get; set; }

        [PDFExport(headerText = "AMOUNT", order = 4, currencyFormat = "#,###", width = 75, cellAlign = 2)]
        [ExcelExport(headerText = "AMOUNT", currencyFormat = "#,###", order = 4)]
        [AuditableAttribute("EMB_PAYMENT_AMOUNT", ERPConstants.CURRENCY_FORMAT)]
        public decimal Amount { get; set; }

        
        [PDFExport(headerText = "CURRENCYRATE", currencyFormat = "#,###.00", order = 6, width = 75, cellAlign = 2)]
        [ExcelExport(headerText = "CURRENCYRATE", currencyFormat = "#,###.00",  order = 6)]
        [AuditableAttribute("EMB_PAYMENT_CURCY_RATE", ERPConstants.CURRENCY_WITH_DECIMAL_FORMAT)]
        public decimal CurrencyRate { get; set; }

        [PDFExport(headerText = "CURRENCYAMOUNT", currencyFormat = "#,###.00", order = 7, width = 75, cellAlign = 2)]
        [ExcelExport(headerText = "CURRENCYAMOUNT", currencyFormat = "#,###.00", order = 7)]
        [AuditableAttribute("EMB_PAYMENT_CURCY_AMOUNT", ERPConstants.CURRENCY_WITH_DECIMAL_FORMAT)]
        public decimal CurrencyAmount { get; set; }


        [PDFExport(headerText = "CURRENCY", order = 8, width = 75, cellAlign = 2)]
        [ExcelExport(headerText = "CURRENCY", order = 8)]
        [AuditableAttribute("EMB_PAYMENT_CURRENCY")]
        public string CurrencyCode { get; set; }
        [NotMapped]
        public string CurrencyDesc { get; set; }

        [AuditableAttribute("APP_STATUS")]
        public string Status { get; set; }
        public Guid? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        [AuditableAttribute("APPROVER_REMARKS")]
        public string ApproverRemarks { get; set; }
        [NotMapped]
        public List<EmbPostPaymentInvDet> EmbPostPaymentInvDet { get; set; }
        [NotMapped]
        public List<AppDocument> AppDocuments { get; set; }


        [PDFExport(headerText = "EMBASSY", order = 2, width = 75, cellAlign = 2)]
        [ExcelExport(headerText = "EMBASSY", order = 2)]
        [NotMapped]
        public string Embassy { get; set; }


        [PDFExport(headerText = "LEDGERCODE", order = 5, width = 75, cellAlign = 2)]
        [ExcelExport(headerText = "LEDGERCODE", order = 5)]
        [NotMapped]
        public string Ledger { get; set; }
    }
}
