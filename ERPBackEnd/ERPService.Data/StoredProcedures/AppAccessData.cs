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
    public static class AppAccessData
    {
        public static List<AppAccess> GetAppAccessList(AppAccess search,  SqlConnection connection)
        {
            List<AppAccess> appAccessList = null;

            var sqlCommand = new SqlCommand("p_get_appAccess_001", connection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            if (search.Id != Guid.Empty)
                sqlCommand.Parameters.AddWithValue("@AppAccessId", search.Id);

            if (!string.IsNullOrEmpty(search.AccessName))
                sqlCommand.Parameters.AddWithValue("@AccessName", search.AccessName);
            if (!string.IsNullOrEmpty(search.AccessCode))
                sqlCommand.Parameters.AddWithValue("@AccessCode", search.AccessCode);
            if (!string.IsNullOrEmpty(search.AccessType))
                sqlCommand.Parameters.AddWithValue("@AccessType", search.AccessType);
            if (!string.IsNullOrEmpty(search.ScreenUrl))
                sqlCommand.Parameters.AddWithValue("@ScreenUrl", search.ScreenUrl);
            if (!string.IsNullOrEmpty(search.Active))
                sqlCommand.Parameters.AddWithValue("@Active", search.Active);

            var reader = sqlCommand.ExecuteReader();
            if(reader.HasRows)
            {
                var appAccessOrd = reader.GetOrdinal("id");
                var accessCodeOrd = reader.GetOrdinal("AccessCode");
                var accessNameOrd = reader.GetOrdinal("AccessName");
                var accessTypeOrd = reader.GetOrdinal("AccessType");
                var screenURLOrd = reader.GetOrdinal("ScreenUrl");
                var activeOrd = reader.GetOrdinal("Active");
                var createdByOrd = reader.GetOrdinal("CreatedBy");
                var createdOnOrd = reader.GetOrdinal("CreatedDate");
                var modifiedByOrd = reader.GetOrdinal("ModifiedBy");
                var modifiedOnOrd = reader.GetOrdinal("ModifiedDate");
                appAccessList = new List<AppAccess>();

                while (reader.Read())
                {
                    var appAccess = new AppAccess()
                    {
                        Id = reader.GetGuid(appAccessOrd),
                        AccessCode = reader.GetString(accessCodeOrd),
                        AccessName = reader.GetString(accessNameOrd),
                        AccessType = reader.GetString(accessTypeOrd),
                        ScreenUrl = reader.IsDBNull(screenURLOrd) ? "" : reader.GetString(screenURLOrd),
                        Active = reader.GetString(activeOrd),
                        CreatedBy = reader.GetGuid(createdByOrd),
                        CreatedDate = reader.GetDateTime(createdOnOrd),
                        ModifiedBy = reader.IsDBNull(modifiedByOrd) ? Guid.Empty : reader.GetGuid(modifiedByOrd),
                        ModifiedDate = reader.IsDBNull(modifiedOnOrd) ? DateTime.MinValue : reader.GetDateTime(modifiedOnOrd),
                    };

                    appAccessList.Add(appAccess);
                }
            }
            return appAccessList;
        }

        public static AppAccess GetAppAccessByid(string appAccessId, SqlConnection connection)
        {
            AppAccess appAccess = null;

            var sqlCommand = new SqlCommand("p_get_appAccess_byId_001", connection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            sqlCommand.Parameters.AddWithValue("@AppAccessId", appAccessId);

            var reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                var appAccessOrd = reader.GetOrdinal("id");
                var accessCodeOrd = reader.GetOrdinal("AccessCode");
                var accessNameOrd = reader.GetOrdinal("AccessName");
                var accessTypeOrd = reader.GetOrdinal("AccessType");
                var screenURLOrd = reader.GetOrdinal("ScreenUrl");
                var activeOrd = reader.GetOrdinal("Active");
                var createdByOrd = reader.GetOrdinal("CreatedBy");
                var createdOnOrd = reader.GetOrdinal("CreatedDate");
                var modifiedByOrd = reader.GetOrdinal("ModifiedBy");
                var modifiedOnOrd = reader.GetOrdinal("ModifiedDate");

                while (reader.Read())
                {
                    appAccess = new AppAccess()
                    {
                        Id = reader.GetGuid(appAccessOrd),
                        AccessCode = reader.GetString(accessCodeOrd),
                        AccessName = reader.GetString(accessNameOrd),
                        AccessType = reader.GetString(accessTypeOrd),
                        ScreenUrl = reader.IsDBNull(screenURLOrd) ? "" : reader.GetString(screenURLOrd),
                        Active = reader.GetString(activeOrd),
                        CreatedBy = reader.GetGuid(createdByOrd),
                        CreatedDate = reader.GetDateTime(createdOnOrd),
                        ModifiedBy = reader.IsDBNull(modifiedByOrd) ? Guid.Empty : reader.GetGuid(modifiedByOrd),
                        ModifiedDate = reader.IsDBNull(modifiedOnOrd) ? DateTime.MinValue : reader.GetDateTime(modifiedOnOrd),
                    };
                }
            }
            return appAccess;
        }


        public static string InsertUpdateAppAccess(AppAccess appAccess, SqlConnection connection, SqlTransaction transaction, out string appAccessId)
        {
            appAccessId = "";
            var sqlCommand = new SqlCommand("p_insert_update_appaccess_001", connection);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Transaction = transaction;

            if (appAccess.Id != Guid.Empty)
                sqlCommand.Parameters.AddWithValue("@Id", appAccess.Id);
            sqlCommand.Parameters.AddWithValue("@AccessName", appAccess.AccessName);
            sqlCommand.Parameters.AddWithValue("@AccessCode", appAccess.AccessCode);
            sqlCommand.Parameters.AddWithValue("@AccessType", appAccess.AccessType);
            sqlCommand.Parameters.AddWithValue("@ScreenUrl", appAccess.ScreenUrl);
            sqlCommand.Parameters.AddWithValue("@CreatedBy", appAccess.CreatedBy);
            sqlCommand.Parameters.AddWithValue("@ModifiedBy", appAccess.ModifiedBy);
            if(appAccess.ModifiedDate > DateTime.MinValue)
                sqlCommand.Parameters.AddWithValue("@ModifiedOn", appAccess.ModifiedDate);
            sqlCommand.Parameters.AddWithValue("@Active", appAccess.Active);

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
                appAccessId = sqlCommand.Parameters["@recordId"].Value.ToString();
            return status;
        }

    }
}
