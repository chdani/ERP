using ERPService.Common.Shared;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPService.DataModel.DTO
{
    public class AppAccessRoleMapping : BaseEntity
    {
        public Guid AppAccessId { get; set; }
        public Guid UserRoleId { get; set; }
        public string AllowAdd { get; set; }
        public string AllowEdit { get; set; }
        public string AllowDelete { get; set; }
        public string AllowApprove { get; set; }
        [NotMapped]
        public string RoleCode { get; set; }
        [NotMapped]
        public string RoleName { get; set; }
        [NotMapped]
        public string AccessType { get; set; }
        [NotMapped]
        public string AccessCode { get; set; }
        [NotMapped]
        public string AccessName { get; set; }
        [NotMapped]
        public string ScreenUrl { get; set; }

    }
}
