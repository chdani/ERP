using ERPService.Common.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPService.DataModel.CTO
{
    public class DirectInvSearch
    {
        public Guid Id { get; set; }
        public List<Guid> VendorMasterId { get; set; } = new List<Guid>();
        public DateTime FromInvDate { get; set; }
        public DateTime ToInvDate { get; set; }
        public List<int> LedgerCode { get; set; } = new List<int>();
        public List<string> CostCenterCode { get; set; } = new List<string>();
        public string InvoiceNo { get; set; }
        public Guid OrgId { get; set; }
        public string FinYear { get; set; }
        public decimal Amount { get; set; }
        public decimal DueAmount { get; set; }
        public DateTime FromDocDate { get; set; }
        public DateTime ToDocDate { get; set; }
        public long DocumentNo { get; set; }
        [NotMapped]
        public List<string> Status { get; set; } = new List<string>();
        [NotMapped]
        public string ExportType { get; set; }
        [NotMapped]
        public string ExportHeaderText { get; set; }

    }

    public class DirectInvPrePaymentDue
    {
        public string FinYear { get; set; }
        public Guid OrgId { get; set; }
        public Guid VendorId { get; set; }
        public Guid DirInvPrePaymentId { get; set; }
        public decimal DueAmount { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string InvoiceNo { get; set; }
        public int LedgerCode { get; set; }

    }
    public class DirectInvPrePaydueSearch
    {
        public string FinYear { get; set; }
        public Guid OrgId { get; set; }
        public Guid VendorId { get; set; }
    }

}
