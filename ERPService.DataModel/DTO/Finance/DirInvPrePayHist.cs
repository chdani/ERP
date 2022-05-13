using ERPService.Common.Shared;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace ERPService.DataModel.DTO
{
    public class DirInvPrePayHist : BaseEntity
    {
        public Guid DirectInvPrePaymentId { get; set; }
        public string FieldName { get; set; }
        public string PrevValue { get; set; }
        public string CurrentValue { get; set; }
        [NotMapped]
        public string UserName { get; set; }

    }
}
