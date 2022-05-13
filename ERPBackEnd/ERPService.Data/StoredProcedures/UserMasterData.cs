using ERPService.Common;
using ERPService.Common.Shared;
using ERPService.DataModel.CTO;
using ERPService.DataModel.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ERPService.Data
{
    public static class UserMasterData
    {
        public static UserContext ValidateUserLogin(UserLogin login,  SqlConnection connection)
        {
            UserContext userContext = null;

            var sqlCommand = new SqlCommand("p_valdidate_userLogin_001", connection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            sqlCommand.Parameters.AddWithValue("@UserName", login.UserName);

            var reader = sqlCommand.ExecuteReader();
            if(reader.HasRows)
            {
                var userIdOrd = reader.GetOrdinal("id");
                var userNameOrd = reader.GetOrdinal("username");
                var firstNameOrd = reader.GetOrdinal("FirstName");
                var lastNameOrd = reader.GetOrdinal("LastName");
                var emailIdOrd = reader.GetOrdinal("EmailId");
                var userTypeOrd = reader.GetOrdinal("UserType");
                var employeeIdOrd = reader.GetOrdinal("EmployeeId");

                while (reader.Read())
                {
                    userContext = new UserContext()
                    {
                        Id = reader.GetGuid(userIdOrd),
                        EmailId = reader.GetString(emailIdOrd),
                        FirstName = reader.GetString(firstNameOrd),
                        LastName = reader.GetString(lastNameOrd),
                        UserName = reader.GetString(userNameOrd),
                        UserType = reader.GetString(userTypeOrd),
                        EmployeeId = reader.GetGuid(employeeIdOrd)
                    };
                }
            }
            return userContext;
        }

        public static string InsertUpdateUserInfo(UserMaster userInfo, SqlConnection connection, SqlTransaction transaction, out string userRoleMapId)
        {
            userRoleMapId = "";
            var sqlCommand = new SqlCommand("p_insert_update_user_001", connection);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Transaction = transaction;

            if(userInfo.Id != Guid.Empty)
                sqlCommand.Parameters.AddWithValue("@Id", userInfo.Id);
            sqlCommand.Parameters.AddWithValue("@UserName", userInfo.UserName);
            sqlCommand.Parameters.AddWithValue("@FirstName", userInfo.FirstName);
            sqlCommand.Parameters.AddWithValue("@LastName", userInfo.LastName);
            sqlCommand.Parameters.AddWithValue("@EmailId", userInfo.EmailId);
            sqlCommand.Parameters.AddWithValue("@Active", userInfo.Active);
            sqlCommand.Parameters.AddWithValue("@UserType", userInfo.UserType);
            sqlCommand.Parameters.AddWithValue("@CreatedBy", userInfo.CreatedBy);
            sqlCommand.Parameters.AddWithValue("@ModifiedBy", userInfo.ModifiedBy);
            if (userInfo.ModifiedDate > DateTime.MinValue)
                sqlCommand.Parameters.AddWithValue("@ModifiedOn", userInfo.ModifiedDate);

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

        public static List<UserMaster> getUserInfoByCriteria(UserMaster search, SqlConnection connection)
        {
            List<UserMaster> users = new List<UserMaster>();

            var sqlCommand = new SqlCommand("p_get_userInfo_001", connection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            if (search.Id != Guid.Empty)
                sqlCommand.Parameters.AddWithValue("@UserId", search.Id);

            if (!string.IsNullOrEmpty(search.UserName))
                sqlCommand.Parameters.AddWithValue("@UserName", search.UserName);
            if (!string.IsNullOrEmpty(search.FirstName))
                sqlCommand.Parameters.AddWithValue("@FirstName", search.FirstName);
            if (!string.IsNullOrEmpty(search.LastName))
                sqlCommand.Parameters.AddWithValue("@LastName", search.LastName);
            if (!string.IsNullOrEmpty(search.Active))
                sqlCommand.Parameters.AddWithValue("@Active", search.Active);

            var reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                var userIdOrd = reader.GetOrdinal("id");
                var userNameOrd = reader.GetOrdinal("username");
                var firstNameOrd = reader.GetOrdinal("FirstName");
                var lastNameOrd = reader.GetOrdinal("LastName");
                var emailIdOrd = reader.GetOrdinal("EmailId");
                var userTypeOrd = reader.GetOrdinal("UserType");
                var activeOrd = reader.GetOrdinal("Active");
                var createdByOrd = reader.GetOrdinal("CreatedBy");
                var createdOnOrd = reader.GetOrdinal("CreatedOn");
                var modifiedByOrd = reader.GetOrdinal("ModifiedBy");
                var modifiedOnOrd = reader.GetOrdinal("ModifiedOn");

                while (reader.Read())
                {
                    var user = new UserMaster()
                    {
                        Id = reader.GetGuid(userIdOrd),
                        EmailId = reader.GetString(emailIdOrd),
                        FirstName = reader.GetString(firstNameOrd),
                        LastName = reader.GetString(lastNameOrd),
                        UserName = reader.GetString(userNameOrd),
                        UserType = reader.GetString(userTypeOrd),
                        Active = reader.GetString(activeOrd),
                        CreatedBy = reader.GetGuid(createdByOrd),
                        CreatedDate = reader.GetDateTime(createdOnOrd),
                        ModifiedBy = reader.IsDBNull(modifiedByOrd) ? Guid.Empty : reader.GetGuid(modifiedByOrd),
                        ModifiedDate = reader.IsDBNull(modifiedOnOrd) ? DateTime.MinValue : reader.GetDateTime(modifiedOnOrd),
                    };
                    users.Add(user);
                }
            }
            return users;
        }


        public static UserMaster getUserInfoById(string userId, SqlConnection connection)
        {
            UserMaster user = null;

            var sqlCommand = new SqlCommand("p_get_userInfo_byId_001", connection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            sqlCommand.Parameters.AddWithValue("@UserId", userId);

            var reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                var userIdOrd = reader.GetOrdinal("id");
                var userNameOrd = reader.GetOrdinal("username");
                var firstNameOrd = reader.GetOrdinal("FirstName");
                var lastNameOrd = reader.GetOrdinal("LastName");
                var emailIdOrd = reader.GetOrdinal("EmailId");
                var userTypeOrd = reader.GetOrdinal("UserType");
                var activeOrd = reader.GetOrdinal("Active");
                var createdByOrd = reader.GetOrdinal("CreatedBy");
                var createdOnOrd = reader.GetOrdinal("CreatedOn");
                var modifiedByOrd = reader.GetOrdinal("ModifiedBy");
                var modifiedOnOrd = reader.GetOrdinal("ModifiedOn");

                while (reader.Read())
                {
                     user = new UserMaster()
                    {
                        Id = reader.GetGuid(userIdOrd),
                        EmailId = reader.GetString(emailIdOrd),
                        FirstName = reader.GetString(firstNameOrd),
                        LastName = reader.GetString(lastNameOrd),
                        UserName = reader.GetString(userNameOrd),
                        UserType = reader.GetString(userTypeOrd),
                        Active = reader.GetString(activeOrd),
                        CreatedBy = reader.GetGuid(createdByOrd),
                        CreatedDate = reader.GetDateTime(createdOnOrd),
                        ModifiedBy = reader.IsDBNull(modifiedOnOrd) ? Guid.Empty : reader.GetGuid(modifiedByOrd),
                        ModifiedDate = reader.IsDBNull(modifiedOnOrd) ? DateTime.MinValue : reader.GetDateTime(modifiedOnOrd),
                    };
                }
            }
            return user;
        }

    }
}
