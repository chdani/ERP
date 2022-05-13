using ERPService.Common.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPService.DataModel.DTO
{
    public class AppMenuMaster : BaseEntity
    {
        public Guid AppAccessId { get; set; }
        public string MainMenuName { get; set; }
        public string SubMenuName { get; set; }
        public string MainMenuCode { get; set; }
        public string SubMenuCode { get; set; }
        public string MainMenuIcon { get; set; }
        public int DispOrder { get; set; }
        public string SubMenuIcon { get; set; }
        public int MainMenuDispOrd { get; set; }
        public bool ShowFinYear { get; set; }
        public bool ShowOrg { get; set; }
        public string ModuleCode { get; set; }
        public string ModuleName { get; set; }
        public int ModuleDispOrder { get; set; }
        public string ModuleIcon { get; set; }

    }
}
