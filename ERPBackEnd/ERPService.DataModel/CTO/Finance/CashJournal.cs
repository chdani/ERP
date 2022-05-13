using ERPService.Common.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPService.DataModel.CTO
{
    public class CashJournalSearch
    {
        public List<Guid> TellerId { get; set; }
        public List<Guid> AccountId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public Guid OrgId { get; set; }
        public string FinYear { get; set; }
    }

    public class CashJournalSummary
    {
        public string TellerName { get; set; }
        public string AccountName { get; set; }
        public string OrganizationName { get; set; }
        public DateTime BalanceDate { get; set; }
        public decimal Opening { get; set; }
        public decimal Credit { get; set; }
        public decimal Debit { get; set; }
        public decimal Closing { get; set; }
    }
}
