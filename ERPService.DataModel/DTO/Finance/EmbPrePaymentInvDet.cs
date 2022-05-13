using ERPService.Common;
using ERPService.Common.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPService.DataModel.DTO
{
    public class EmbPrePaymentInvDet : BaseEntity
    {
        public Guid EmbPrePaymentEmbDetId { get; set; }
        [AuditableAttribute("EMB_PAYMENT_LEDGER")]
        public int LedgerCode { get; set; }

        [AuditableAttribute("CASH_TRANSFER_ORG")]
        public Guid OrgId { get; set; }

        [AuditableAttribute("BUDGT_FIN_YEAR")]
        public string FinYear { get; set; }

        [AuditableAttribute("EMB_PAYMENT_INVDATE", ERPConstants.DATE_FORMAT)]
        public DateTime InvDate { get; set; }

        [AuditableAttribute("EMB_PAYMENT_INVNO")]
        public string InvNo { get; set; }

        [AuditableAttribute("EMB_PAYMENT_TELEX_REFNO")]
        public string TelexRef { get; set; }

        [AuditableAttribute("EMB_PAYMENT_AMOUNT", ERPConstants.CURRENCY_FORMAT)]
        public decimal Amount { get; set; }
        public decimal DueAmount { get; set; }



        [AuditableAttribute("EMB_PAYMENT_CURCY_AMOUNT", ERPConstants.CURRENCY_WITH_DECIMAL_FORMAT)]
        public decimal CurrencyAmount { get; set; }

        [AuditableAttribute("EMB_PAYMENT_REMARKS")]
        public string Remarks { get; set; }
        [NotMapped]
        public List<AppDocument> AppDocuments { get; set; }

        [NotMapped]
        public string ledgerDecs { get; set; }
    }
}
