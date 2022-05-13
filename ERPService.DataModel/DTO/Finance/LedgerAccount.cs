using ERPService.Common.Shared;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPService.DataModel.DTO
{
    public class LedgerAccount : BaseEntity
    {
        public int  LedgerCode { get; set; }
        public string LedgerDesc { get; set; }
        public string Remarks { get; set; }
        public string UsedFor { get; set; }
    }
}
