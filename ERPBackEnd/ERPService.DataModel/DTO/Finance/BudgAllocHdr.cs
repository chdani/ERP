using ERPService.Common;
using ERPService.Common.Shared;
using ERPService.DataModel.CTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPService.DataModel.DTO
{
    public class BudgAllocHdr : BaseEntity
    {
        [PDFExport(headerText = "TRANSNO", order = 1, width = 50, cellAlign = 1)]
        [ExcelExport(headerText = "TRANSNO", order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public Int64 TransNo { get; set; }


        [AuditableAttribute("BUDGT_TYPE")]
        [PDFExport(headerText = "BUDGETTYPE", order = 2, width = 50, cellAlign = 1)]
        [ExcelExport(headerText = "BUDGETTYPE", order = 2)]
        public string BudgetType { get; set; }
        [NotMapped]
        public List<string> SelectBudgetType { get; set; } = new List<string>();

        [AuditableAttribute("CASH_TRANSFER_ORG")]
        public Guid OrgId { get; set; }


        [AuditableAttribute("BUDGT_FIN_YEAR")]
        public string FinYear { get; set; }

        [AuditableAttribute("BUDGT_BUDGT_DATE", ERPConstants.DATE_FORMAT)]
        [PDFExport(headerText = "BUDGETDATE", order = 3, width = 50, dateFormat = "dd-MM-yyyy", cellAlign = 1)]
        [ExcelExport(headerText = "BUDGETDATE", dateFormat = "dd-MM-yyyy", order = 3)]
        public DateTime BudgetDate { get; set; }

        [AuditableAttribute("BUDGT_BUDGT_AMT", ERPConstants.CURRENCY_FORMAT)]
        [PDFExport(headerText = "BUDGETAMOUNT", order = 4, width = 50, currencyFormat = "#,###", cellAlign = 1)]
        [ExcelExport(headerText = "BUDGETAMOUNT", currencyFormat = "#,###", order = 4)]
        public decimal BudgetAmount { get; set; }

        [AuditableAttribute("APP_STATUS")]
        public string Status { get; set; }
        [NotMapped]
        public List<string> SelectedStatus { get; set; } = new List<string>();
        public Guid? ApprovedBy { get; set; }

        public DateTime? ApprovedDate { get; set; }

        [AuditableAttribute("APPROVER_REMARKS")]
        public string ApproverRemarks { get; set; }

        [NotMapped]
        public DateTime FromDate { get; set; }
        [NotMapped]
        public DateTime ToDate { get; set; }

        [NotMapped]
        public ICollection<BudgAllocDet> BudgAllocDet { get; set; }
        [NotMapped]
        public string AccountName { get; set; }

        [NotMapped]
        public List<AppDocument> AppDocuments { get; set; }

        [PDFExport(subHeaderText = "GLACCOUNT", order = 1, width = 50, cellAlign = 1)]
        [ExcelExport(subHeaderText = "GLACCOUNT", order = 1)]
        [NotMapped]
        public string LedgerDesc { get; set; }

        [PDFExport(subHeaderText = "TOGLACCOUNT", order = 3, width = 50, cellAlign = 1)]
        [ExcelExport(subHeaderText = "TOGLACCOUNT", order = 3)]
        [NotMapped]
        public string ToLedgerDesc { get; set; }

        [PDFExport(subHeaderText = "BUDGETAMOUNT", order = 2, width = 50, currencyFormat = "#,###", cellAlign = 1)]
        [ExcelExport(subHeaderText = "BUDGETAMOUNT", currencyFormat = "#,###", order = 2)]
        [NotMapped]
        public decimal BudgetDetailAmount { get; set; }

        [PDFExport(subHeaderText = "REMARKS", order = 4, width = 50, cellAlign = 1)]
        [ExcelExport(subHeaderText = "REMARKS", order = 4)]
        [NotMapped]
        public string Remarks { get; set; }


        [NotMapped]
        public string ExportType { get; set; }

        [NotMapped]
        public string ExportHeaderText { get; set; }

    }
}
