using ERPService.Common.Shared;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPService.DataModel.DTO
{
    public class PettyCashTeller : BaseEntity
    {
        public string TellerCode { get; set; }
        public string TellerName { get; set; }
        public bool IsHeadTeller { get; set; }
        public Guid UserId { get; set; }
        [NotMapped]
        public string UserName { get; set; }
    }
}
