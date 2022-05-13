using ERPService.Common.Shared;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPService.DataModel.DTO
{
    public class LedgerAccountSearch : BaseEntity
    {
        public int LedgerCodeFrom { get; set; }
        public int LedgerCodeTo { get; set; }
        public string LedgerDesc { get; set; }
    }
}
