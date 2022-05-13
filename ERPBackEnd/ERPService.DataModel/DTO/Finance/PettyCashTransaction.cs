using ERPService.Common.Shared;
using ERPService.DataModel.CTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPService.DataModel.DTO
{
    public class PettyCashTransfer : BaseEntity
    {
        public string FinYear { get; set; }
        public Guid FromTellerId { get; set; }
        public Guid FromAccountId { get; set; }
        public Guid ToTellerId { get; set; }
        public Guid ToAccountId { get; set; }
        public Guid FromOrgId { get; set; }
        public Guid ToOrgId { get; set; }

        [AuditableAttribute("CASH_TRANSFER_TRANS_DATE")]
        [PDFExport(headerText = "TRANSACTIONDATE", order = 4, width = 78, dateFormat = "dd-MM-yyyy", cellAlign = 1)]
        [ExcelExport(headerText = "TRANSACTIONDATE", order = 4, dateFormat = "dd-MM-yyyy")]
        public DateTime TransDate { get; set; }
        [AuditableAttribute("CASH_TRASNFER_AMOUNT")]

        [PDFExport(headerText = "AMOUNT", order = 5, width = 78, cellAlign = 2)]
        [ExcelExport(headerText = "AMOUNT", order = 5)]
        public decimal Amount { get; set; }
        [AuditableAttribute("CASH_TRANSFER_REMARK")]
        [PDFExport(headerText = "REMARKS", order = 6, width = 150)]
        [ExcelExport(headerText = "REMARKS", order = 6)]
        public string Remarks { get; set; }

        [NotMapped]
        public string FromTellerName { get; set; }

        [NotMapped]
        public string FromAccountName { get; set; }

        [NotMapped]
        public string ToTellerName { get; set; }

        [NotMapped]
        public string ToAccountName { get; set; }

        [NotMapped]
        public string FromOrgName { get; set; }

        [NotMapped]
        public string ToOrgName { get; set; }

        [NotMapped]
        public DateTime FromTransDate { get; set; }
        [NotMapped]
        public DateTime ToTransDate { get; set; }

        [NotMapped]
        public bool IsDayClosed { get; set; }

        [NotMapped]
        public string ExportType { get; set; }

        [NotMapped]
        public string ExportHeaderText { get; set; }

        [NotMapped]
        [AuditableAttribute("CASH_TRANSFER_ORG")]
        [PDFExport(headerText = "ORGANIZATION", order = 1, width = 100)]
        [ExcelExport(headerText = "ORGANIZATION", order = 1)]
        public string FormattedOrgName
        {
            get { return string.Format($"{this.FromOrgName} - {this.ToOrgName}"); }

        }
        [AuditableAttribute("CASH_TRANSFER_ACCOUNT")]
        [PDFExport(headerText = "ACCOUNT", order = 2, width = 100)]
        [ExcelExport(headerText = "ACCOUNT", order = 2)]
        public string FormattedAccount
        {
            get { return string.Format($"{this.FromAccountName} - {this.ToAccountName}"); }

        }
        [AuditableAttribute("CASH_TRANSFER_TELLER")]
        [PDFExport(headerText = "TELLER", order = 3, width = 100)]
        [ExcelExport(headerText = "TELLER", order = 3)]
        public string FormattedTeller
        {
            get { return string.Format($"{this.FromTellerName} - {this.ToTellerName}"); }

        }

        [NotMapped]
        public List<Guid> SelectFromTellerId { get; set; } = new List<Guid>();
        [NotMapped]
        public List<Guid> SelectFromAccountId { get; set; } = new List<Guid>();
        [NotMapped]
        public List<Guid> SelectToTellerId { get; set; } = new List<Guid>();
        [NotMapped]
        public List<Guid> SelectToAccountId { get; set; } = new List<Guid>();
        [NotMapped]
        public List<Guid> SelectFromOrgId { get; set; } = new List<Guid>();
        [NotMapped]
        public List<Guid> SelectToOrgId { get; set; } = new List<Guid>();
    }
}