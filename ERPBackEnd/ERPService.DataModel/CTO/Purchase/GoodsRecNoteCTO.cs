using ERPService.Common.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPService.DataModel.CTO
{
    public class GoodsRecNoteSearch
    {
        public string TransNo { get; set; }
        public DateTime FromTransDate { get; set; }
        public DateTime ToTransDate { get; set; }
        public DateTime FromInvDate { get; set; }
        public DateTime ToInvDate { get; set; }
        public string InvNo { get; set; }
        public Guid VendorMasterId { get; set; }
        public string Status { get; set; }
        public string ExportType { get; set; }
        public string ExportHeaderText { get; set; }
    }
}
