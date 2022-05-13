using ERPService.Common.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPService.DataModel.DTO
{
    public class AppAccess : BaseEntity
    {
        public string AccessName { get; set; }
        public string AccessCode { get; set; }
        public string AccessType { get; set; }
        public string ScreenUrl { get; set; }

        public ICollection<AppAccessRoleMapping> AppAccessRoleMap {get;set;}
    }
}
