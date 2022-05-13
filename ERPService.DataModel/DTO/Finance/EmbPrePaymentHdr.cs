using ERPService.Common;
using ERPService.Common.Shared;
using ERPService.DataModel.CTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPService.DataModel.DTO
{
    public class EmbPrePaymentHdr : BaseEntity
    {
        [AuditableAttribute("CASH_TRANSFER_ORG")]
        public Guid OrgId { get; set; }


        [PDFExport(headerText = "TRANSNO", order = 1, width = 75, cellAlign = 2)]
        [ExcelExport(headerText = "TRANSNO", order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long TransNo { get; set; }
        [NotMapped]
        [PDFExport(headerText = "EMBASSY", order = 2, width = 75, cellAlign = 2)]
        [ExcelExport(headerText = "EMBASSY", order = 2)]
        public string Embassy { get; set; }
        [PDFExport(headerText = "CURRENCY", order = 3, width = 50, cellAlign = 2)]
        [ExcelExport(headerText = "CURRENCY", order = 3)]
        [AuditableAttribute("EMB_PAYMENT_CURRENCY")]
        public string CurrencyCode { get; set; }
        [PDFExport(headerText = "CURRENCYRATE", order = 4, width = 50, cellAlign = 2)]
        [ExcelExport(headerText = "CURRENCYRATE", order = 4)]
        [AuditableAttribute("EMB_PAYMENT_CURCY_RATE")]
        public decimal CurrencyRate { get; set; }

        [AuditableAttribute("BUDGT_FIN_YEAR")]
        public string FinYear { get; set; }

        [PDFExport(headerText = "BOOKDATE", order = 6, dateFormat = "dd-MM-yyyy", width = 75, cellAlign = 2)]
        [ExcelExport(headerText = "BOOKDATE", dateFormat = "dd-MM-yyyy", order = 6)]
        [AuditableAttribute("EMB_PAYMENT_BOOK_DATE", ERPConstants.DATE_FORMAT)]
        public DateTime BookDate { get; set; }


        [PDFExport(headerText = "BOOKNO", order = 5, width = 75, cellAlign = 2)]
        [ExcelExport(headerText = "BOOKNO", order = 5)]
        [AuditableAttribute("EMB_PAYMENT_BOOK_NO")]
        public string BookNo { get; set; }

        [AuditableAttribute("APP_STATUS")]
        public string Status { get; set; }

        [AuditableAttribute("EMB_PAYMENT_REMARKS")]
        public string Remarks { get; set; }

        public Guid? ApprovedBy { get; set; }

        public DateTime? ApprovedDate { get; set; }

        [AuditableAttribute("APPROVER_REMARKS")]
        public string ApproverRemarks { get; set; }
        [NotMapped]
        public ICollection<EmbPrePaymentEmbDet> EmbPrePaymentEmbDet { get; set; }
        [NotMapped]
        public bool UserConsent { get; set; }
        [NotMapped]
        public List<AppDocument> AppDocuments { get; set; }


        [PDFExport(subHeaderText = "CLEARENCEORDERNO", order = 1, width = 50, cellAlign = 2)]
        [ExcelExport(subHeaderText = "CLEARENCEORDERNO", order = 1)]
        [NotMapped]
        public string ClearanceOrdNo { get; set; }
        [PDFExport(subHeaderText = "CLEARENCEORDERDATE", order = 2, dateFormat = "dd-MM-yyyy", width = 50, cellAlign = 2)]
        [ExcelExport(subHeaderText = "CLEARENCEORDERDATE", dateFormat = "dd-MM-yyyy", order = 2)]
        [NotMapped]
        public DateTime ClearanceOrdDate { get; set; }

        [PDFExport(subHeaderText = "AMOUNT", order = 3, currencyFormat = "#,###", width = 50, cellAlign = 2)]
        [ExcelExport(subHeaderText = "AMOUNT", currencyFormat = "#,###", order = 3)]
        [NotMapped]
        public decimal Amount { get; set; }

        [PDFExport(subHeaderText = "REMARKS", order = 4, width = 50, cellAlign = 2)]
        [ExcelExport(subHeaderText = "REMARKS", order = 4)]
        [NotMapped]
        public string DetRemarks { get; set; }
        [PDFExport(secondSubHeaderText = "LEDGERCODE", order = 1, width = 50, cellAlign = 1)]
        [ExcelExport(secondSubHeaderText = "LEDGERCODE", order = 1)]
        [NotMapped]
        public string ledgerDecs { get; set; }

        [PDFExport(secondSubHeaderText = "INVOICENO", order = 2, width = 50, cellAlign = 1)]
        [ExcelExport(secondSubHeaderText = "INVOICENO", order = 2)]
        [NotMapped]
        public string InvNo { get; set; }

        [PDFExport(secondSubHeaderText = "TELEXREF", order = 3, width = 50, cellAlign = 1)]
        [ExcelExport(secondSubHeaderText = "TELEXREF", order = 3)]
        [NotMapped]
        public string TelexRef { get; set; }

        [PDFExport(secondSubHeaderText = "AMOUNT", order = 4, currencyFormat = "#,###", width = 50, cellAlign = 1)]
        [ExcelExport(secondSubHeaderText = "AMOUNT", currencyFormat = "#,###", order = 4)]
        [NotMapped]
        public decimal InvAmount { get; set; }

        [PDFExport(secondSubHeaderText = "CURRENCYAMOUNT", order = 5, currencyFormat = "#,###", width = 50, cellAlign = 1)]
        [ExcelExport(secondSubHeaderText = "CURRENCYAMOUNT", currencyFormat = "#,###", order = 5)]
        [NotMapped]
        public decimal CurrencyAmount { get; set; }




        [PDFExport(secondSubHeaderText = "REMARKS", order = 6, width = 50, cellAlign = 1)]
        [ExcelExport(secondSubHeaderText = "REMARKS", order = 6)]
        [NotMapped]
        public string InvRemarks { get; set; }
        [AuditableAttribute("EMB_PAYMENT_EMB_NAME")]
        public Guid EmbassyId { get; set; }

        [NotMapped]

        public string CurrencyDesc { get; set; }
    }
}
