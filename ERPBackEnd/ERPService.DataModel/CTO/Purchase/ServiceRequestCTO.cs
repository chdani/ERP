using ERPService.Common.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPService.DataModel.CTO
{
    public class ServiceRequestSearch
    {
        public string TransNo { get; set; }
        public DateTime FromTransDate { get; set; }
        public DateTime ToTransDate { get; set; }
        public Guid ProductId { get; set; }
        public Guid EmployeeId { get; set; }
        public string Status { get; set; }
        public string ExportType { get; set; }
        public string ExportHeaderText { get; set; }
    }
 
}
