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
    public static class UserRoleData
    {
        public static string InsertUpdateUserRole (UserRole userRole, SqlConnection connection, SqlTransaction transaction, out string userRoleId)
        {
            userRoleId = "";

            var sqlCommand = new SqlCommand("p_insert_update_userrole_001", connection);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Transaction = transaction;

            if(userRole.Id != Guid.Empty)
                sqlCommand.Parameters.AddWithValue("@Id", userRole.Id);
            sqlCommand.Parameters.AddWithValue("@RoleCode", userRole.RoleCode);
            sqlCommand.Parameters.AddWithValue("@RoleName", userRole.RoleName);
            sqlCommand.Parameters.AddWithValue("@Active", userRole.Active);
            sqlCommand.Parameters.AddWithValue("@CreatedBy", userRole.CreatedBy);
            sqlCommand.Parameters.AddWithValue("@ModifiedBy", userRole.ModifiedBy);
            if (userRole.ModifiedDate > DateTime.MinValue)
                sqlCommand.Parameters.AddWithValue("@ModifiedOn", userRole.ModifiedDate);

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
            if(status == APPMessageKey.DATASAVESUCSS)
                userRoleId = sqlCommand.Parameters["@recordId"].Value.ToString();
             
            return status;
        }

        public static List<UserRole> getuserRoleByCriteria(UserRole search, SqlConnection connection)
        {
            List<UserRole> userRoles = new List<UserRole>();

            var sqlCommand = new SqlCommand("p_get_userRole_001", connection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            if (search.Id != Guid.Empty)
                sqlCommand.Parameters.AddWithValue("@UserRoleId", search.Id);

            if (!string.IsNullOrEmpty(search.RoleName))
                sqlCommand.Parameters.AddWithValue("@RoleName", search.RoleName);
            if (!string.IsNullOrEmpty(search.RoleCode))
                sqlCommand.Parameters.AddWithValue("@RoleCode", search.RoleCode);
            if (!string.IsNullOrEmpty(search.Active))
                sqlCommand.Parameters.AddWithValue("@Active", search.Active);

            var reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                var userRoleIdOrd = reader.GetOrdinal("id");
                var roleCodeOrd = reader.GetOrdinal("RoleCode");
                var roleNameOrd = reader.GetOrdinal("RoleName");
                var activeOrd = reader.GetOrdinal("Active");
                var createdByOrd = reader.GetOrdinal("CreatedBy");
                var createdOnOrd = reader.GetOrdinal("CreatedDate");
                var modifiedByOrd = reader.GetOrdinal("ModifiedBy");
                var modifiedOnOrd = reader.GetOrdinal("ModifiedDate");

                while (reader.Read())
                {
                    var userRole = new UserRole()
                    {
                        Id = reader.GetGuid(userRoleIdOrd),
                        RoleCode = reader.GetString(roleCodeOrd),
                        RoleName = reader.GetString(roleNameOrd),
                        Active = reader.GetString(activeOrd),
                        CreatedBy = reader.GetGuid(createdByOrd),
                        CreatedDate = reader.GetDateTime(createdOnOrd),
                        ModifiedBy = reader.IsDBNull(modifiedByOrd) ? Guid.Empty : reader.GetGuid(modifiedByOrd),
                        ModifiedDate = reader.IsDBNull(modifiedOnOrd) ? DateTime.MinValue : reader.GetDateTime(modifiedOnOrd),
                    };
                    userRoles.Add(userRole);
                }
            }
            return userRoles;
        }


        public static UserRole getUserRoleById(string userRoleId, SqlConnection connection)
        {
            UserRole userRole = null;

            var sqlCommand = new SqlCommand("p_get_userRole_byId_001", connection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            sqlCommand.Parameters.AddWithValue("@UserRoleId", userRoleId);

            var reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                var userRoleIdOrd = reader.GetOrdinal("id");
                var roleCodeOrd = reader.GetOrdinal("RoleCode");
                var roleNameOrd = reader.GetOrdinal("RoleName");
                var activeOrd = reader.GetOrdinal("Active");
                var createdByOrd = reader.GetOrdinal("CreatedBy");
                var createdOnOrd = reader.GetOrdinal("CreatedDate");
                var modifiedByOrd = reader.GetOrdinal("ModifiedBy");
                var modifiedOnOrd = reader.GetOrdinal("ModifiedDate");

                while (reader.Read())
                {
                    userRole = new UserRole()
                    {
                         Id = reader.GetGuid(userRoleIdOrd),
                         RoleCode = reader.GetString(roleCodeOrd),
                         RoleName = reader.GetString(roleNameOrd),
                         Active = reader.GetString(activeOrd),
                         CreatedBy = reader.GetGuid(createdByOrd),
                         CreatedDate = reader.GetDateTime(createdOnOrd),
                         ModifiedBy = reader.IsDBNull(modifiedOnOrd) ? Guid.Empty : reader.GetGuid(modifiedByOrd),
                         ModifiedDate = reader.IsDBNull(modifiedOnOrd) ? DateTime.MinValue : reader.GetDateTime(modifiedOnOrd),
                     };
                }
            }
            return userRole;
        }

    }
}
