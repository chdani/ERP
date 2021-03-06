using ERPService.Common.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPService.DataModel.DTO
{
    public class BudgAllocHdrComment : BaseEntity
    {
        public Guid BudgAllocHdrId { get; set; }
        public string Comments { get; set; }
        [NotMapped]
        public string UserName { get; set; }

        [NotMapped]
        public List<AppDocument> AppDocuments { get; set; }
    }
}
