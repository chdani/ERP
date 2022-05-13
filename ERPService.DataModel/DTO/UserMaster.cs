using ERPService.Common.Shared;
using ERPService.DataModel.CTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPService.DataModel.DTO
{
    public class UserMaster : BaseEntity
    {
        public UserMaster()
        {
            userRole = new List<AppAccessCode>();
            UserRoleMap = new List<UserRoleMapping>();
        }
        public string UserName { get; set; }
        public string EmailId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserType { get; set; }
        public Guid? EmployeeId { get; set; }
        [NotMapped]
        public string Password { get; set; }
        [NotMapped]
        public Boolean Resetpwd { get; set; }
        [NotMapped]
        public List<UserRoleMapping> UserRoleMap { get; set; }
        [NotMapped]
        public List<UserLedgerAccnt> LedgerAccnts { get; set; }
        [NotMapped]
        public List<UserOrganizationMap> Organizations { get; set; }
        public List<AppAccessCode> userRole { get; set; }
    }
}
