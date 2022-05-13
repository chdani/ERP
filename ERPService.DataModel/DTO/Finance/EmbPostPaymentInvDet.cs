using ERPService.Common.Shared;
using ERPService.DataModel.CTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using static ERPService.Common.Shared.AuditUtility;

namespace ERPService.DataModel.DTO
{
    public class EmbPostPaymentInvDet : BaseEntity
    {
        public Guid EmbPostPaymentId { get; set; }
        public Guid EmbPrePaymentInvDetId { get; set; }
        [NotMapped]
        public EmbPrePaymentDueDet EmbPrePaymentDueDet { get; set; }

    }
}
