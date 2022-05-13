using ERPService.DataModel.CTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ERPService.Data
{
    public static class UserAccessData
    {
        public static List<UserRoleAccessInfo> GetUserRoleAccess(Guid userId,  SqlConnection connection)
        {
            List<UserRoleAccessInfo> userAccessList = null;

            var sqlCommand = new SqlCommand("p_get_userbased_access_001", connection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            sqlCommand.Parameters.AddWithValue("@UserId", userId);

            var reader = sqlCommand.ExecuteReader();
            if(reader.HasRows)
            {
                var roleCodeOrd = reader.GetOrdinal("RoleCode");
                var roleNameOrd = reader.GetOrdinal("RoleName");
                var allowDeleteOrd = reader.GetOrdinal("AllowDelete");
                var allowAddOrd = reader.GetOrdinal("AllowAdd");
                var allowEditOrd = reader.GetOrdinal("AllowEdit");
                var allowApproveOrd = reader.GetOrdinal("AllowApprove");
                var accessCodeOrd = reader.GetOrdinal("AccessCode");
                var accessNameOrd = reader.GetOrdinal("AccessName");
                var accessTypeOrd = reader.GetOrdinal("AccessType");
                var screenUrlOrd = reader.GetOrdinal("ScreenUrl");
                var mainMenuCodeOrd = reader.GetOrdinal("MainMenuCode");
                var mainMenuNameOrd = reader.GetOrdinal("MainMenuName");
                var moduleCodeOrd = reader.GetOrdinal("ModuleCode");
                var moduleNameOrd = reader.GetOrdinal("ModuleName");
                var subMenuNameOrd = reader.GetOrdinal("SubMenuName");
                var subMenuCodeOrd = reader.GetOrdinal("SubMenuCode");
                var subMenuIconOrd = reader.GetOrdinal("SubMenuIcon");
                var mainMenuIconOrd = reader.GetOrdinal("MainMenuIcon");
                var moduleIconOrd = reader.GetOrdinal("ModuleIcon");
                var dispOrderOrd = reader.GetOrdinal("DispOrder");
                var mainMenuDispOrd = reader.GetOrdinal("MainMenuDispOrd");
                var showFinYear = reader.GetOrdinal("ShowFinYear");
                var showOrg = reader.GetOrdinal("ShowOrg");
                var moduleDispOrd = reader.GetOrdinal("ModuleDispOrder");
                userAccessList = new List<UserRoleAccessInfo>();

                while (reader.Read())
                {
                    var userAccess = new UserRoleAccessInfo()
                    {
                        RoleCode = reader.GetString(roleCodeOrd),
                        RoleName= reader.GetString(roleNameOrd),
                        AllowDelete = reader.GetString(allowDeleteOrd),
                        AllowAdd = reader.GetString(allowAddOrd),
                        AllowEdit = reader.GetString(allowEditOrd),
                        AllowApprove = reader.GetString(allowApproveOrd),
                        AccessCode = reader.GetString(accessCodeOrd),
                        AccessName = reader.GetString(accessNameOrd) ,
                        AccessType = reader.GetString(accessTypeOrd),
                        ScreenUrl = reader.IsDBNull(screenUrlOrd) ? null : reader.GetString(screenUrlOrd),
                        ModuleCode = reader.IsDBNull(moduleCodeOrd) ? null : reader.GetString(moduleCodeOrd),
                        ModuleName = reader.IsDBNull(moduleNameOrd) ? null : reader.GetString(moduleNameOrd),
                        MainMenuCode = reader.IsDBNull(mainMenuCodeOrd) ? null : reader.GetString(mainMenuCodeOrd),
                        MainMenuName = reader.IsDBNull(mainMenuNameOrd) ? null : reader.GetString(mainMenuNameOrd),
                        SubMenuName = reader.IsDBNull(subMenuNameOrd) ? null : reader.GetString(subMenuNameOrd),
                        SubMmenuCode = reader.IsDBNull(subMenuCodeOrd) ? null : reader.GetString(subMenuCodeOrd),
                        MainMenuIcon = reader.IsDBNull(mainMenuIconOrd) ? null : reader.GetString(mainMenuIconOrd),
                        ModuleIcon = reader.IsDBNull(moduleIconOrd) ? null : reader.GetString(moduleIconOrd),
                        DispOrder = reader.IsDBNull(dispOrderOrd) ? 0 : reader.GetInt32(dispOrderOrd),
                        MainMenuDispOrder = reader.IsDBNull(mainMenuDispOrd) ? 0 : reader.GetInt32(mainMenuDispOrd),
                        ShowFinYear = reader.IsDBNull(showFinYear) ?false: reader.GetBoolean(showFinYear),
                        ShowOrg = reader.IsDBNull(showOrg) ? false : reader.GetBoolean(showOrg),
                        ModuleDispOrder = reader.IsDBNull(moduleDispOrd) ? 0 : reader.GetInt32(moduleDispOrd),
                        SubMenuIcon = reader.IsDBNull(subMenuIconOrd) ? null :  reader.GetString(subMenuIconOrd)
                    };

                    userAccessList.Add(userAccess);
                }
            }
            return userAccessList;
        }

     }
}
