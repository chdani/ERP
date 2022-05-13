using ERPService.Common.Shared;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPService.DataModel.DTO
{
    public class UserRoleMapping : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid UserRoleId { get; set; }
        //public virtual UserMaster UserMaster { get; set; }
        //public virtual UserRole UserRole { get; set; }

        [NotMapped]
        public string RoleCode { get; set; }
        [NotMapped]
        public string RoleName { get; set; }
        [NotMapped]
        public string UserName { get; set; }
        [NotMapped]
        public Guid Code { get; set; }
        [NotMapped]
        public string Name { get; set; }
    }
}
