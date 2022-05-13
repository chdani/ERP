using ERPService.Common.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPService.DataModel.DTO
{
    public class GoodsRecNoteStatusHist : BaseEntity
    {
        public Guid GoodsReceiptNoteId { get; set; }
        public string Status { get; set; }
        public string Comments { get; set; }
        [NotMapped]
        public string UserName { get; set; }
    }
}
