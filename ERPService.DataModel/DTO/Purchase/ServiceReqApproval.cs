using ERPService.Common.Shared;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPService.DataModel.DTO
{
    public class ServiceReqApproval : BaseEntity
    {
        public Guid ServiceRequestId { get; set; }
        public int ApprovalLevel { get; set; }
        public Guid ApprovedBy { get; set; }
        public DateTime ApprovedDate { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        [NotMapped]
        public string UserName { get; set; }
        [NotMapped]
        public string ProdCategoryName { get; set; }
        [NotMapped]
        public string ProdSubCategoryName { get; set; }
        [NotMapped]
        public String ProductName { get; set; }
        [NotMapped]
        public String UnitName { get; set; }
        [NotMapped]
        public decimal Quantitys { get; set; }
    }
}
