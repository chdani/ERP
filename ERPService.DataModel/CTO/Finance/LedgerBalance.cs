using ERPService.Common.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPService.DataModel.CTO
{
    public class LedgerBalanceSearch
    {
        public Guid TransactionId { get; set; }
        public List<string> TransactionType { get; set; } = new List<string>();
        public List<string> BudgetType { get; set; } = new List<string>();
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<int> LedgerCodes { get; set; } = new List<int>();
        public int LedgerCode { get; set; }
        public Guid OrgId { get; set; }
        public string FinYear { get; set; }
        public decimal Credit { get; set; }
        public decimal Debit { get; set; }
        public string ExportType { get; set; }
        public string ExportHeaderText { get; set; }
        public Guid UserId { get; set; }
    }

    public class LedgerBalanceReport
    {
        [PDFExport(headerText = "TRANSACTION", order = 1, width = 50, cellAlign = 1)]
        [ExcelExport(headerText = "TRANSACTION", order = 1)]
        public string TransactionType { get; set; }
        [PDFExport(headerText = "DATE", order = 3, width = 50, dateFormat = "dd-MM-yyyy", cellAlign = 1)]
        [ExcelExport(headerText = "DATE", dateFormat = "dd-MM-yyyy", order = 3)]
        public DateTime TransDate { get; set; }
        [PDFExport(headerText = "LEDGERCODE", order = 2, width = 50, cellAlign = 1)]
        [ExcelExport(headerText = "LEDGERCODE", order = 2)]
        public string LedgerDesc { get; set; }

        public int LedgerCode { get; set; }

        public string FinYear { get; set; }
        [PDFExport(headerText = "CREDIT", order = 4, width = 50, currencyFormat = "#,###", cellAlign = 1)]
        [ExcelExport(headerText = "CREDIT", currencyFormat = "#,###", order = 4)]
        public decimal Credit { get; set; }
        [PDFExport(headerText = "DEBIT", order = 5, width = 50, currencyFormat = "#,###", cellAlign = 1)]
        [ExcelExport(headerText = "DEBIT", currencyFormat = "#,###", order = 5)]
        public decimal Debit { get; set; }
        [PDFExport(headerText = "REMARKS", order = 6, width = 50, cellAlign = 1)]
        [ExcelExport(headerText = "REMARKS", order = 6)]
        public string Remarks { get; set; }

        public Guid TransactionId { get; set; }



    }

    public class LedgerBalanceSummary
    {
        [PDFExport(headerText = "LEDGERCODE", order = 1, width = 50, cellAlign = 1)]
        [ExcelExport(headerText = "LEDGERCODE", order = 1)]
        public int LedgerCode { get; set; }
        [PDFExport(headerText = "AMOUNT", order = 2, width = 50, currencyFormat = "#,###", cellAlign = 1)]
        [ExcelExport(headerText = "AMOUNT", currencyFormat = "#,###", order = 2)]
        public decimal Balance { get; set; }
    }
}
