using ERPService.Common.Shared;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace ERPService.DataModel.DTO
{
    public class DirInvPrePayStatusHist : BaseEntity
    {
        public Guid DirectInvPrePaymentId { get; set; }
        public string Status { get; set; }
        public string Comments { get; set; }
        [NotMapped]
        public string UserName { get; set; }

    }
}
