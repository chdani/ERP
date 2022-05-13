using ERPService.Common.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPService.DataModel.DTO
{
    public class ProdInvIssueComment : BaseEntity
    {
        public Guid ProdInvIssueId { get; set; }
        public string Comments { get; set; }
        [NotMapped]
        public string UserName { get; set; }

        [NotMapped]
        public List<AppDocument> AppDocuments { get; set; }
    }
}
