using ERPService.Common.Shared;
using ERPService.DataModel.CTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPService.DataModel.DTO
{
    public class UserRole : BaseEntity
    {
        public UserRole()
        {
            UserScreenAccess = new List<AppAccessMapping>();
        }
        public string RoleCode { get; set; }
        public string RoleName { get; set; }
        public virtual ICollection<UserRoleMapping> UserRoleMappings { get; set; }
        public List<AppAccessMapping> UserScreenAccess { get; set; }
    }
}
