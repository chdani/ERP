using ERPService.Common.Shared;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPService.DataModel.DTO
{
    public class PettyCashAccount : BaseEntity
    {
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public bool IsHeadAccount { get; set; }
    }
}
