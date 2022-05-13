using ERPService.Common;
using ERPService.DataModel.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ERPService.Data
{
    public static class AppAccessRoleMapData
    {
        public static string InsertUpdateAppAccessRoleMap(AppAccessRoleMapping appAccessRoleMap, SqlConnection connection, SqlTransaction transaction, out string appAccessRoleMapId)
        {
            
            appAccessRoleMapId = "";
            var sqlCommand = new SqlCommand("p_insert_upd_AppAcccessRoleMap_001", connection);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Transaction = transaction;

            if (appAccessRoleMap.Id != Guid.Empty)
                sqlCommand.Parameters.AddWithValue("@Id", appAccessRoleMap.Id);
            sqlCommand.Parameters.AddWithValue("@AppAccessId", appAccessRoleMap.AppAccessId);
            sqlCommand.Parameters.AddWithValue("@UserRoleId", appAccessRoleMap.UserRoleId);
            sqlCommand.Parameters.AddWithValue("@AllowDelete", appAccessRoleMap.AllowDelete);
            sqlCommand.Parameters.AddWithValue("@AllowWrite", appAccessRoleMap.AllowAdd);
            sqlCommand.Parameters.AddWithValue("@AllowWrite", appAccessRoleMap.AllowEdit);
            sqlCommand.Parameters.AddWithValue("@Active", appAccessRoleMap.Active);
            sqlCommand.Parameters.AddWithValue("@CreatedBy", appAccessRoleMap.CreatedBy);
            sqlCommand.Parameters.AddWithValue("@ModifiedBy", appAccessRoleMap.ModifiedBy);
            if (appAccessRoleMap.ModifiedDate > DateTime.MinValue)
                sqlCommand.Parameters.AddWithValue("@ModifiedOn", appAccessRoleMap.ModifiedDate);

            var sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@RecordId";
            sqlParam.SqlDbType = SqlDbType.UniqueIdentifier;
            sqlParam.SqlValue = null;
            sqlParam.Direction = ParameterDirection.Output;
            sqlCommand.Parameters.Add(sqlParam);

            sqlParam = new SqlParameter();
            sqlParam.ParameterName = "@Result";
            sqlParam.SqlDbType = SqlDbType.NVarChar;
            sqlParam.SqlValue = null;
            sqlParam.Size = 20;
            sqlParam.Direction = ParameterDirection.Output;
            sqlCommand.Parameters.Add(sqlParam);

            sqlCommand.ExecuteNonQuery();
            var status = sqlCommand.Parameters["@Result"].Value.ToString();
            if (status == APPMessageKey.DATASAVESUCSS)
                appAccessRoleMapId = sqlCommand.Parameters["@recordId"].Value.ToString();
            
            return status;
        }

        public static List<AppAccessRoleMapping> getAppAccessRoleMapByCriteria(AppAccessRoleMapping search, SqlConnection connection)
        {
            List<AppAccessRoleMapping> userRoleMaps = new List<AppAccessRoleMapping>();

            var sqlCommand = new SqlCommand("p_get_appAccessRoleMaps_001", connection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            if (search.Id != Guid.Empty)
                sqlCommand.Parameters.AddWithValue("@Id", search.Id);

            if (search.UserRoleId != Guid.Empty)
                sqlCommand.Parameters.AddWithValue("@UserRoleId", search.UserRoleId);
            if (search.AppAccessId != Guid.Empty)
                sqlCommand.Parameters.AddWithValue("@AppAccessId", search.AppAccessId);
            if (!string.IsNullOrEmpty(search.Active))
                sqlCommand.Parameters.AddWithValue("@Active", search.Active);

            var reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                var userRoleMapIdOrd = reader.GetOrdinal("id");
                var userRoleIdOrd = reader.GetOrdinal("UserRoleId");
                var appAccessIdOrd = reader.GetOrdinal("AppAccessId");
                var roleCodeOrd = reader.GetOrdinal("RoleCode");
                var roleNameOrd = reader.GetOrdinal("RoleName");
                var accessCodeOrd = reader.GetOrdinal("AccessCode");
                var accessNameOrd = reader.GetOrdinal("AccessName");
                var screenUrlOrd = reader.GetOrdinal("ScreenUrl");
                var accessTypeOrd = reader.GetOrdinal("AccessType");
                var activeOrd = reader.GetOrdinal("Active");
                var createdByOrd = reader.GetOrdinal("CreatedBy");
                var createdOnOrd = reader.GetOrdinal("CreatedDate");
                var allowAdd = reader.GetOrdinal("AllowAdd");
                var allowEdit = reader.GetOrdinal("AllowEdit");
                var allowDelete = reader.GetOrdinal("AllowDelete");
                var modifiedByOrd = reader.GetOrdinal("ModifiedBy");
                var modifiedOnOrd = reader.GetOrdinal("ModifiedDate");

                while (reader.Read())
                {
                    var userRoleMap = new AppAccessRoleMapping()
                    {
                        Id = reader.GetGuid(userRoleMapIdOrd),
                        UserRoleId = reader.GetGuid(userRoleIdOrd),
                        AppAccessId = reader.GetGuid(appAccessIdOrd),
                        RoleCode = reader.GetString(roleCodeOrd),
                        RoleName = reader.GetString(roleNameOrd),
                        AccessCode = reader.GetString(accessCodeOrd),
                        AccessType = reader.GetString(accessTypeOrd),
                        AccessName = reader.GetString(accessNameOrd),
                        ScreenUrl = reader.IsDBNull(screenUrlOrd) ? "" : reader.GetString(screenUrlOrd),
                        Active = reader.GetString(activeOrd),
                        AllowAdd = reader.GetString(allowAdd),
                        AllowEdit = reader.GetString(allowEdit),
                        AllowDelete = reader.GetString(allowEdit),
                        CreatedBy = reader.GetGuid(createdByOrd),
                        CreatedDate = reader.GetDateTime(createdOnOrd),
                        ModifiedBy = reader.IsDBNull(modifiedByOrd) ? Guid.Empty : reader.GetGuid(modifiedByOrd),
                        ModifiedDate = reader.IsDBNull(modifiedOnOrd) ? DateTime.MinValue : reader.GetDateTime(modifiedOnOrd),
                    };
                    userRoleMaps.Add(userRoleMap);
                }
            }
            return userRoleMaps;
        }


        public static AppAccessRoleMapping getAppAccessRoleMapById(string appAccessRoleMapId, SqlConnection connection)
        {
            AppAccessRoleMapping appAccessRoleMap = null;

            var sqlCommand = new SqlCommand("p_get_AppAccessMapById_001", connection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            sqlCommand.Parameters.AddWithValue("@Id", appAccessRoleMapId);

            var reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                var userRoleMapIdOrd = reader.GetOrdinal("id");
                var userRoleIdOrd = reader.GetOrdinal("UserRoleId");
                var appAccessIdOrd = reader.GetOrdinal("AppAccessId");
                var roleCodeOrd = reader.GetOrdinal("RoleCode");
                var roleNameOrd = reader.GetOrdinal("RoleName");
                var accessCodeOrd = reader.GetOrdinal("AccessCode");
                var accessNameOrd = reader.GetOrdinal("AccessName");
                var screenUrlOrd = reader.GetOrdinal("ScreenUrl");
                var accessTypeOrd = reader.GetOrdinal("AccessType");
                var activeOrd = reader.GetOrdinal("Active");
                var createdByOrd = reader.GetOrdinal("CreatedBy");
                var createdOnOrd = reader.GetOrdinal("CreatedDate");
                var modifiedByOrd = reader.GetOrdinal("ModifiedBy");
                var modifiedOnOrd = reader.GetOrdinal("ModifiedDate");

                while (reader.Read())
                {
                    appAccessRoleMap = new AppAccessRoleMapping()
                    {
                        Id = reader.GetGuid(userRoleMapIdOrd),
                        UserRoleId = reader.GetGuid(userRoleIdOrd),
                        AppAccessId = reader.GetGuid(appAccessIdOrd),
                        RoleCode = reader.GetString(roleCodeOrd),
                        RoleName = reader.GetString(roleNameOrd),
                        AccessCode = reader.GetString(accessCodeOrd),
                        AccessType = reader.GetString(accessTypeOrd),
                        AccessName = reader.GetString(accessNameOrd),
                        ScreenUrl = reader.IsDBNull(screenUrlOrd) ? "" : reader.GetString(screenUrlOrd),
                        Active = reader.GetString(activeOrd),
                        CreatedBy = reader.GetGuid(createdByOrd),
                        CreatedDate = reader.GetDateTime(createdOnOrd),
                        ModifiedBy = reader.IsDBNull(modifiedOnOrd) ? Guid.Empty : reader.GetGuid(modifiedByOrd),
                        ModifiedDate = reader.IsDBNull(modifiedOnOrd) ? DateTime.MinValue : reader.GetDateTime(modifiedOnOrd),
                    };

                }
            }

            return appAccessRoleMap;
        }

    }
}
