using ERPService.Common.Shared;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPService.DataModel.DTO
{
    public class LedgerAccountGrp : BaseEntity
    {
        public string AccountCode { get; set; }
        public string AccountDesc { get; set; }
        public int LedgerCodeFrom { get; set; }
        public int LedgerCodeTo { get; set; }
    }
}
