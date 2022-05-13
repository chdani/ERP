using ERPService.Common.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPService.DataModel.CTO
{
    public class ProdInvIssueSearch
    {
        public string TransNo { get; set; }
        public DateTime FromTransDate { get; set; }
        public DateTime ToTransDate { get; set; }
        public Guid EmployeeId { get; set; }
        public string Status { get; set; }
        public string ExportType { get; set; }
        public string ExportHeaderText { get; set; }
    }
	
	    public class ProdInventorySearch
    {
        public DateTime FromTransDate { get; set; }
        public DateTime ToTransDate { get; set; }
        public Guid ProductMasterId { get; set; }
        public string TransactionType { get; set; }
        public Guid WareHouseLocationId { get; set; }
        public Guid ProdCategoryId { get; set; }
        public string ExportType { get; set; }
        public string ExportHeaderText { get; set; }
    }
}
