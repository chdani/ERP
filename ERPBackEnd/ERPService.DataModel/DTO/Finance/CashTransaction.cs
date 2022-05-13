using ERPService.Common.Shared;
using ERPService.DataModel.CTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPService.DataModel.DTO
{
    public class CashTransaction : BaseEntity
    {
        public Guid TellerId { get; set; }
        public Guid TellerUserId { get; set; }
        public Guid AccountId { get; set; }
        public Guid OrgId { get; set; }
        public string ProcessType { get; set; }
        [PDFExport(headerText = "PEOCESS", order = 1, width = 50, cellAlign = 1)]
        [ExcelExport(headerText = "PEOCESS", order = 1)]
        [AuditableAttribute("CASHMGMT_PROCESS")]
        [NotMapped]
        public string ProcessName { get; set; }
        public string TransType { get; set; } //Expense - E, Receipt - R
        [PDFExport(headerText = "DATE", order = 2, width = 78, dateFormat = "dd-MM-yyyy", cellAlign = 1)]
        [ExcelExport(headerText = "DATE", order = 2, dateFormat = "dd-MM-yyyy")]
        [AuditableAttribute("CASHMGMT_TRANSDATE")]
        public DateTime TransDate { get; set; }
        public int LedgerCode { get; set; }
        [PDFExport(headerText = "LEDGER", order = 4, width = 100, cellAlign = 1)]
        [ExcelExport(headerText = "LEDGER", order = 4)]
        [AuditableAttribute("CASHMGMT_LEDGER")]
        [NotMapped]
        public string LedgerCodeWithDesc { get; set; }
        [PDFExport(headerText = "RECEPIENT", order = 6, width = 100, cellAlign = 1)]
        [ExcelExport(headerText = "RECEPIENT", order = 6)]
        [AuditableAttribute("CASHMGMT_RECIPIENT")]
        public string Recipient { get; set; }
        public string CostCenter { get; set; }
        [PDFExport(headerText = "COSTCENTER", order = 5, width = 100, cellAlign = 1)]
        [ExcelExport(headerText = "COSTCENTER", order = 5)]
        [AuditableAttribute("CASHMGMT_COSTCENTER")]
        [NotMapped]
        public string CostCenterWithDesc { get; set; }
        public string FinYear { get; set; }

        public decimal Credit { get; set; }
        public decimal Debit { get; set; }
        [PDFExport(headerText = "REFERENCE", order = 7, width = 100, cellAlign = 1)]
        [ExcelExport(headerText = "REFERENCE", order = 7)]
        [AuditableAttribute("CASHMGMT_REFERENCE")]
        public string ReferenceNo { get; set; }
        [PDFExport(headerText = "REMARKS", order = 8, width = 100, cellAlign = 1)]
        [ExcelExport(headerText = "Remarks", order = 8)]
        [AuditableAttribute("CASHMGMT_REMARKS")]
        public string Remarks { get; set; }

        [NotMapped]
        public DateTime FromTransDate { get; set; }

        [NotMapped]
        public List<string> SelectProcessType { get; set; } = new List<string>();

        [NotMapped]
        public List<int> SelectLedger { get; set; } = new List<int>();

        [NotMapped]
        public List<string> SelectCostCenter { get; set; } = new List<string>();
        [NotMapped]
        public DateTime ToTransDate { get; set; }
        [NotMapped]
        public bool IsDayClosed { get; set; }

        [NotMapped]
        public string ExportType { get; set; }

        [NotMapped]
        public string ExportHeaderText { get; set; }

        [NotMapped]
        [PDFExport(headerText = "AMOUNT", order = 3, width = 78, cellAlign = 2)]
        [ExcelExport(headerText = "AMOUNT", order = 3)]
        [AuditableAttribute("CASHMGMT_AMOUNT")]
        public decimal FormattedAmount
        {
            get { return this.TransType == "E" ? this.Debit : this.Credit; }

        }

    }
}
