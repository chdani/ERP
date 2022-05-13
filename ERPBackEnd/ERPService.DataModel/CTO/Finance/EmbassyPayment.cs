using ERPService.Common.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPService.DataModel.CTO
{
    public class EmbPrePaymenteSearch
    {
        public string FinYear { get; set; }
        public Guid OrgId { get; set; }
        public string BookNo { get; set; }
        public DateTime FromBookDate { get; set; }
        public DateTime ToBookDate { get; set; }
        public List<string> Status { get; set; } = new List<string>();
        public List<Guid> EmbassyId { get; set; } = new List<Guid>();
        public string ExportType { get; set; }
        public string ExportHeaderText { get; set; }
    }

    public class EmbPostPaymenteSearch
    {
        public string FinYear { get; set; }
        public Guid OrgId { get; set; }
        public List<Guid> EmbassyId { get; set; } = new List<Guid>();
        public List<Int32> LedgerCode { get; set; } = new List<int>();
        public string BookNo { get; set; }
        public decimal Amount { get; set; }
        public decimal CurrencyAmount { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<string> Status { get; set; } = new List<string>();

        public string ExportType { get; set; }

        public string ExportHeaderText { get; set; }
    }

    public class EmbPrePaymentDueSearch
    {
        public string FinYear { get; set; }
        public Guid OrgId { get; set; }
        public Guid EmbassyId { get; set; }
    }

    public class EmbPrePaymentDue
    {
        public Guid Id { get; set; }
        public string FinYear { get; set; }
        public Guid OrgId { get; set; }
        public Guid EmbassyId { get; set; }
        public List<EmbPrePaymentDueDet> DueDetail { get; set; }
    }

    public class EmbPrePaymentDueDet
    {
        public Guid DetailId { get; set; }
        public decimal DueAmount { get; set; }
        public DateTime InvDate { get; set; }
        public string InvNo { get; set; }
        public string TelexRef { get; set; }
        public string Remarks { get; set; }

    }
}
