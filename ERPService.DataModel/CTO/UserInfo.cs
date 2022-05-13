using ERPService.Common.Shared;
using System.Collections.Generic;

namespace ERPService.DataModel.CTO
{
    public class UserInfo : BaseEntity
    {
        public UserContext UserContext { get; set; }
        public List<RoleInfo> UserRoles { get; set; }
        public List<AppModule> AppMenus { get; set; }
        public List<UserSubMenu> SubMenus { get; set; }
    }
   
    public class RoleInfo
    {
        public string RoleCode { get; set; }
        public string RoleName { get; set; }
        public string AccessCode { get; set; }
        public string AccessName { get; set; }
        public string AllowWrite { get; set; }
        public string AllowEdit { get; set; }
        public string AllowDelete { get; set; }
        public string AllowApprove { get; set; }
       
    }

    public class AppModule
    {
        public string ModuleCode { get; set; }
        public string ModuleName { get; set; }
        public int DisplayOrder { get; set; }
        public string ModuleIcon { get; set; }
        public List<UserMenu> UserMenus { get; set; }
    }
    public class UserMenu
    {
        public string MainMenuCode { get; set; }
        public string MainMenuName { get; set; }
        public string MainMenuIcon { get; set; }
        public int DisplayOrder { get; set; }
        public List<UserSubMenu> SubMenus { get; set; }
    }

    public class UserSubMenu
    {
        public string AllowDelete { get; set; }
        public string AllowAdd { get; set; }
        public string AllowEdit { get; set; }
        public string AllowApprove { get; set; }
        public string ScreenCode { get; set; }
        public string ScreenName { get; set; }
        public string ScreenUrl { get; set; }
        public string SubMenuIcon { get; set; }
        public int DisplayOrder { get; set; }
        public string SubMenuName { get; set; }
        public string SubMmenuCode { get; set; }
        public bool ShowFinYear { get; set; }
        public bool ShowOrg { get; set; }
    }

    public class UserLanguage
    {
        public string Language { get; set; }
    }

    public class SelectLabel
    {
        public string code { get; set; }
        public string name { get; set; }
    }
}
