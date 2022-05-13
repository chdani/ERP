using ERPService.Common.Shared;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPService.DataModel.DTO
{
    public class PettyCashBalance : BaseEntity
    {
        public string FinYear { get; set; }
        public Guid TellerId { get; set; }
        public Guid AccountId { get; set; }
        public Guid OrgId { get; set; }
        public DateTime BalanceDate { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal Credit { get; set; }
        public decimal Debit { get; set; }
        public decimal ClosingBalance { get; set; }
        public bool DayClosed { get; set; }
    }
}
