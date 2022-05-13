using ERPService.Common;
using ERPService.Common.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPService.DataModel.DTO
{
    public class EmbPrePaymentEmbDet : BaseEntity
    {

        public Guid EmbPrePaymentHdrId { get; set; }

        [AuditableAttribute("EMB_PAYMENT_CLEARANCEORDNO")]
        public string ClearanceOrdNo { get; set; }

        [AuditableAttribute("CASH_TRANSFER_ORG")]
        public Guid OrgId { get; set; }

        [AuditableAttribute("BUDGT_FIN_YEAR")]
        public string FinYear { get; set; }

        [AuditableAttribute("EMB_PAYMENT_REMARKS")]
        public string Remarks { get; set; }

        [AuditableAttribute("EMB_PAYMENT_AMOUNT", ERPConstants.CURRENCY_FORMAT)]
        public decimal Amount { get; set; }

        [AuditableAttribute("EMB_PAYMENT_CLEARANCEORDDATE", ERPConstants.DATE_FORMAT)]
        public DateTime ClearanceOrdDate { get; set; }
        [NotMapped]
        public string CurrencyDesc { get; set; }
        [NotMapped]
        public ICollection<EmbPrePaymentInvDet> EmbPrePaymentInvDet { get; set; }
        [NotMapped]
        public List<AppDocument> AppDocuments { get; set; }



        [NotMapped]
        public string Embassy { get; set; }

    }
}
