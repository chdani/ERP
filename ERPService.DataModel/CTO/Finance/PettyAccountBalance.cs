using ERPService.Common.Shared;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPService.DataModel.CTO
{
    public class PettyCashBalanceResponse
    {
        public string FinYear { get; set; }
        public Guid TellerId { get; set; }
        public Guid AccountId { get; set; }
        public Guid TellerUserId { get; set; }
        public string AccountName { get; set; }
        public string TellerName { get; set; }
        public string UserName { get; set; }
        public DateTime BalanceDate { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal Credit { get; set; }
        public decimal Debit { get; set; }
        public decimal ClosingBalance { get; set; }
        public bool IsHeadAccount { get; set; }
        public bool IsHeadTeller { get; set; }
    }

    public class PettyCashBalanceSearch
    {
        public string FinYear { get; set; }
        public Guid TellerId { get; set; }
        public Guid AccountId { get; set; }
        public Guid UserId { get; set; }
        public Guid OrgId { get; set; }

        public DateTime BalanceDate
        {
            get { return _BalanceDate; }
            set { _BalanceDate = value.ToLocalTime(); }
        }

        public DateTime _BalanceDate { get; set; }

        [NotMapped]
        public DateTime FromTransDate { get; set; }
        [NotMapped]
        public DateTime ToTransDate { get; set; }
    }

    public class PettyCashTransferSearch {
        public string FinYear { get; set; }
        public Guid FromOrg { get; set; }
        public Guid ToOrg { get; set; }
        public Guid FromAccount { get; set; }
        public Guid ToAccount { get; set; }
        public Guid FromTeller { get; set; }
        public Guid ToTeller { get; set; }
        public decimal Amount { get; set; }
        public string Remarks { get; set; }
    }

    public class DayClosureRequest
    {
        public Guid FromTellerId { get; set; }

        public Guid FromOrgId { get; set; }

        public DateTime TransDate { get; set; }

        public string FinYear { get; set; }
    }
}
