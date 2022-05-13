using ERPService.Common.Shared;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPService.DataModel.DTO
{
    public class LedgerBalance : BaseEntity
    {
        public Guid TransactionId { get; set; }
        public string TransactionType { get; set; }
        public DateTime LedgerDate { get; set; }
        public int LedgerCode { get; set; }
        public Guid OrgId { get; set; }
        public string FinYear { get; set; }
        public decimal Credit { get; set; }
        public decimal Debit { get; set; }
        public string Remarks { get; set; }
        public bool IsCommitted { get; set; } // Always true, PO creation will be set as false  

     

    }
}
