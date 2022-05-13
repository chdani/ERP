using ERPService.Common.Shared;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPService.DataModel.DTO
{
    public class LedgerBalanceDraft : BaseEntity
    {
        public Guid TransactionId { get; set; }
        public string TransactionType { get; set; }
        public DateTime LedgerDate { get; set; }
        public int LedgerCode { get; set; }
        public Guid OrgId { get; set; }
        public string FinYear { get; set; }
        public decimal Amount { get; set; }
    }
}
