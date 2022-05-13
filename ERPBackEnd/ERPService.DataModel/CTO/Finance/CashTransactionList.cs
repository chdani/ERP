using ERPService.Common.Shared;
using ERPService.DataModel.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;


namespace ERPService.DataModel.CTO
{
    public class CashTransactionList
    {
        public CashTransaction ReceiptDetails { get; set; }
        public CashTransaction ExpensesDetails { get; set; }
        public List<PettyCashTransfer> TransactionDetails { get; set; }

    }
}
