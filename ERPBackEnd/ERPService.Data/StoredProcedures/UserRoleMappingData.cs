using ERPService.DataModel.CTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using ERPService.DataModel.DTO;
using ERPService.Common;

namespace ERPService.Data
{
    public static class UserRoleMappingData
    {
        public static string InsertUpdateUserRoleMap (UserRoleMapping userRoleMap, SqlConnection connection, SqlTransaction transaction, out string userRoleMapId)
        {
            userRoleMapId = "";
            var sqlCommand = new SqlCommand("p_insert_upd_usrRoleMap_001", connection);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Transaction = transaction;

            if(userRoleMap.Id != Guid.Empty)
                sqlCommand.Parameters.AddWithValue("@Id", userRoleMap.Id);
            sqlCommand.Parameters.AddWithValue("@UserId", userRoleMap.UserId);
            sqlCommand.Parameters.AddWithValue("@UserRoleId", userRoleMap.UserRoleId);
            sqlCommand.Parameters.AddWithValue("@Active", userRoleMap.Active);
            sqlCommand.Parameters.AddWithValue("@CreatedBy", userRoleMap.CreatedBy);
            sqlCommand.Parameters.AddWithValue("@ModifiedBy", userRoleMap.ModifiedBy);
            if(userRoleMap.ModifiedDate > DateTime.MinValue)
                sqlCommand.Parameters.AddWithValue("@ModifiedOn", userRoleMap.ModifiedDate);

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
            if (status == APPMessageKey.DATASAVESUCSS) userRoleMapId = sqlCommand.Parameters["@recordId"].Value.ToString();

            return status;
        }

        public static List<UserRoleMapping> getuserRoleMapByCriteria(UserRoleMapping search, SqlConnection connection)
        {
            List<UserRoleMapping> userRoleMaps = new List<UserRoleMapping>();

            var sqlCommand = new SqlCommand("p_get_userRoleMaps_001", connection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            if (search.Id != Guid.Empty)
                sqlCommand.Parameters.AddWithValue("@Id", search.Id);

            if (search.UserRoleId != Guid.Empty)
                sqlCommand.Parameters.AddWithValue("@UserRoleId", search.UserRoleId);
            if (search.UserId != Guid.Empty)
                sqlCommand.Parameters.AddWithValue("@UserId", search.UserId);
            if (!string.IsNullOrEmpty(search.Active))
                sqlCommand.Parameters.AddWithValue("@Active", search.Active);

            var reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                var userRoleMapIdOrd = reader.GetOrdinal("id");
                var userIDOrd = reader.GetOrdinal("UserID");
                var userRoleIdOrd = reader.GetOrdinal("UserRoleId");
                var roleCodeOrd = reader.GetOrdinal("RoleCode");
                var roleNameOrd = reader.GetOrdinal("RoleName");
                var userNameOrd = reader.GetOrdinal("UserName");
                var activeOrd = reader.GetOrdinal("Active");
                var createdByOrd = reader.GetOrdinal("CreatedBy");
                var createdOnOrd = reader.GetOrdinal("CreatedDate");
                var modifiedByOrd = reader.GetOrdinal("ModifiedBy");
                var modifiedOnOrd = reader.GetOrdinal("ModifiedDate");

                while (reader.Read())
                {
                    var userRoleMap = new UserRoleMapping()
                    {
                        Id = reader.GetGuid(userRoleMapIdOrd),
                        UserId = reader.GetGuid(userIDOrd),
                        UserRoleId = reader.GetGuid(userRoleIdOrd),
                        RoleCode = reader.GetString(roleCodeOrd),
                        RoleName = reader.GetString(roleNameOrd),
                        UserName = reader.GetString(userNameOrd),
                        Active = reader.GetString(activeOrd),
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


        public static UserRoleMapping getUserRoleMapById(string userRoleMapId, SqlConnection connection)
        {
            UserRoleMapping userRoleMap = null;

            var sqlCommand = new SqlCommand("p_get_userRoleMapById_001", connection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            sqlCommand.Parameters.AddWithValue("@Id", userRoleMapId);

            var reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                var userRoleMapIdOrd = reader.GetOrdinal("id");
                var userIDOrd = reader.GetOrdinal("UserID");
                var userRoleIdOrd = reader.GetOrdinal("UserRoleId");
                var roleCodeOrd = reader.GetOrdinal("RoleCode");
                var roleNameOrd = reader.GetOrdinal("RoleName");
                var userNameOrd = reader.GetOrdinal("UserName");
                var activeOrd = reader.GetOrdinal("Active");
                var createdByOrd = reader.GetOrdinal("CreatedBy");
                var createdOnOrd = reader.GetOrdinal("CreatedDate");
                var modifiedByOrd = reader.GetOrdinal("ModifiedBy");
                var modifiedOnOrd = reader.GetOrdinal("ModifiedDate");

                while (reader.Read())
                {
                    userRoleMap = new UserRoleMapping()
                    {
                        Id = reader.GetGuid(userRoleMapIdOrd),
                        UserId = reader.GetGuid(userIDOrd),
                        UserRoleId = reader.GetGuid(userRoleIdOrd),
                        RoleCode = reader.GetString(roleCodeOrd),
                        RoleName = reader.GetString(roleNameOrd),
                        UserName = reader.GetString(userNameOrd),
                        Active = reader.GetString(activeOrd),
                        CreatedBy = reader.GetGuid(createdByOrd),
                        CreatedDate = reader.GetDateTime(createdOnOrd),
                        ModifiedBy = reader.IsDBNull(modifiedByOrd) ? Guid.Empty : reader.GetGuid(modifiedByOrd),
                        ModifiedDate = reader.IsDBNull(modifiedOnOrd) ? DateTime.MinValue : reader.GetDateTime(modifiedOnOrd),
                    };
                     
                }
            }

            return userRoleMap;
        }

    }
}
