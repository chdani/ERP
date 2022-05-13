using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static ERPService.Common.Shared.AuditUtility;

namespace ERPService.Common.Shared
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }

        public Guid CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
        [ConcurrencyCheck]
        public DateTime? ModifiedDate { get; set; }
        [AuditableAttribute("Active")]
        public string Active { get; set; }
        [NotMapped]
        public char Action { get; set; } //N - New, M- Modified, D-Deleted
        [NotMapped]
        public AppResponse Validations { get; set; }
    }
}
