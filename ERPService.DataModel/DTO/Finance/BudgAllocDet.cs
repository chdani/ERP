using ERPService.Common.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPService.DataModel.DTO
{
    public class BudgAllocDet : BaseEntity
    {
        public Guid BudgAllocHdrId { get; set; }
        public Guid OrgId { get; set; }
        public int LedgerCode { get; set; }
        public decimal BudgetAmount { get; set; }
        [NotMapped]
        public decimal BalanceAmount { get; set; }
        public string Remarks { get; set; }
        public int? ToLedgerCode { get; set; }
        [NotMapped]
        public string LedgerDesc { get; set; }
        [NotMapped]
        public string ToLedgerDesc { get; set; }
        
        [NotMapped]
        public ICollection<LedgerBalance> Ledger { get; set; }
        [NotMapped]
        public List<AppDocument> AppDocuments { get; set; }
        
    }
}
