using ERPService.Common.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPService.DataModel.DTO
{
    public class EmbPrePaymentInvDetHist : BaseEntity
    {
        public Guid EmbPrePaymentInvDetId { get; set; }
        public string FieldName { get; set; }
        public string PrevValue { get; set; }
        public string CurrentValue { get; set; }

        [NotMapped]
        public string UserName { get; set; }
    }
}
