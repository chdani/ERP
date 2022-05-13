using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPService.DataModel.CTO
{
    public class UserRoleAccessInfo
    {
        public string RoleCode { get; set; }
        public string RoleName { get; set; }
        public string AllowDelete { get; set; }
        public string AllowAdd { get; set; }
        public string AllowEdit { get; set; }
        public string AllowApprove { get; set; }
        public string AccessCode { get; set; }
        public string AccessName { get; set; }
        public string AccessType { get; set; }
        public string ScreenUrl { get; set; }
        public string ModuleCode { get; set; }
        public string ModuleName { get; set; }
        public string MainMenuCode { get; set; }
        public string MainMenuName { get; set; }
        public string SubMenuName { get; set; }
        public string SubMmenuCode { get; set; }
        public string ModuleIcon { get; set; }
        public string MainMenuIcon { get; set; }
        public string SubMenuIcon { get; set; }
        public int DispOrder { get; set; }
        public int MainMenuDispOrder { get; set; }
        public bool ShowFinYear { get; set; }
        public bool ShowOrg { get; set; }
        public int ModuleDispOrder { get; set; }
    }

    public class AppAccessMapping
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public bool add { get; set; }
        public bool edit { get; set; }
        public bool delete { get; set; }
        public bool approve { get; set; }
        public bool view { get; set; }
    }

    [NotMapped]
    public class AppAccessCode {
        public Guid Code { get; set; }
        public string Name { get; set; }
    }
}
