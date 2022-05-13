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
    public static class AppMenuMasterData
    {
        public static List<AppMenuMaster> GetAppMenuMasterList(AppMenuMaster search,  SqlConnection connection)
        {
            List<AppMenuMaster> appMenuMasterList = null;

            var sqlCommand = new SqlCommand("p_get_menuMasters_001", connection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            if (search.Id != Guid.Empty)
                sqlCommand.Parameters.AddWithValue("@Id", search.Id);

            if (!string.IsNullOrEmpty(search.MainMenuCode))
                sqlCommand.Parameters.AddWithValue("@MainMenuCode", search.MainMenuCode);
            if (!string.IsNullOrEmpty(search.MainMenuName))
                sqlCommand.Parameters.AddWithValue("@MainMenuName", search.MainMenuName);
            if (!string.IsNullOrEmpty(search.SubMenuName))
                sqlCommand.Parameters.AddWithValue("@SubMenuName", search.SubMenuName);
            if (!string.IsNullOrEmpty(search.SubMenuCode))
                sqlCommand.Parameters.AddWithValue("@SubMenuCode", search.SubMenuCode);
            if (search.AppAccessId != Guid.Empty)
                sqlCommand.Parameters.AddWithValue("@AppAccessId", search.AppAccessId);
            if (!string.IsNullOrEmpty(search.Active))
                sqlCommand.Parameters.AddWithValue("@Active", search.Active);

            var reader = sqlCommand.ExecuteReader();
            if(reader.HasRows)
            {
                var idOrd = reader.GetOrdinal("id");
                var mainMenuCodeOrd = reader.GetOrdinal("MainMenuCode");
                var mainMenuNameOrd = reader.GetOrdinal("MainMenuName");
                var mainMenuIconOrd = reader.GetOrdinal("MainMenuIcon");
                var mainMenuDispOrdOrd = reader.GetOrdinal("MainMenuDispOrd");
                var subMenuNameOrd = reader.GetOrdinal("SubMenuName");
                var subMenuCodeOrd = reader.GetOrdinal("SubMenuCode");
                var subMenuIconeOrd = reader.GetOrdinal("SubMenuIcon");
                var dispOrderOrd = reader.GetOrdinal("DispOrder");
                var appAccessIdOrd = reader.GetOrdinal("AppAccessId");
                var activeOrd = reader.GetOrdinal("Active");
                var createdByOrd = reader.GetOrdinal("CreatedBy");
                var createdOnOrd = reader.GetOrdinal("CreatedDate");
                var modifiedByOrd = reader.GetOrdinal("ModifiedBy");
                var modifiedOnOrd = reader.GetOrdinal("ModifiedDate");
                appMenuMasterList = new List<AppMenuMaster>();

                while (reader.Read())
                {
                    var appMenuMaster = new AppMenuMaster()
                    {
                        Id = reader.GetGuid(idOrd),
                        MainMenuCode= reader.GetString(mainMenuCodeOrd),
                        MainMenuName = reader.GetString(mainMenuNameOrd),
                        MainMenuDispOrd = reader.IsDBNull(mainMenuDispOrdOrd) ? 1 : reader.GetInt32(mainMenuDispOrdOrd),
                        MainMenuIcon = reader.IsDBNull(mainMenuIconOrd) ? "" : reader.GetString(mainMenuIconOrd),
                        SubMenuIcon = reader.IsDBNull(subMenuIconeOrd) ? "" :  reader.GetString(subMenuIconeOrd),
                        SubMenuCode = reader.GetString(subMenuCodeOrd),
                        SubMenuName = reader.GetString(subMenuNameOrd),
                        DispOrder = reader.IsDBNull(dispOrderOrd) ? 1 : reader.GetInt32(dispOrderOrd),
                        AppAccessId = reader.IsDBNull(appAccessIdOrd) ?  Guid.Empty : reader.GetGuid(appAccessIdOrd),
                        Active = reader.GetString(activeOrd),
                        CreatedBy = reader.GetGuid(createdByOrd),
                        CreatedDate = reader.GetDateTime(createdOnOrd),
                        ModifiedBy = reader.IsDBNull(modifiedByOrd) ? Guid.Empty : reader.GetGuid(modifiedByOrd),
                        ModifiedDate = reader.IsDBNull(modifiedOnOrd) ? DateTime.MinValue : reader.GetDateTime(modifiedOnOrd),
                    };

                    appMenuMasterList.Add(appMenuMaster);
                }
            }
            return appMenuMasterList;
        }

        public static AppMenuMaster GetAppMenuMasterByid(string appMenuMasterId, SqlConnection connection)
        {
            AppMenuMaster appMenuMaster = null;

            var sqlCommand = new SqlCommand("p_get_menuMaster_byId_001", connection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            sqlCommand.Parameters.AddWithValue("@Id", appMenuMasterId);

            var reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                var idOrd = reader.GetOrdinal("id");
                var mainMenuCodeOrd = reader.GetOrdinal("MainMenuCode");
                var mainMenuNameOrd = reader.GetOrdinal("MainMenuName");
                var mainMenuIconOrd = reader.GetOrdinal("MainMenuIcon");
                var mainMenuDispOrdOrd = reader.GetOrdinal("MainMenuDispOrd");
                var subMenuNameOrd = reader.GetOrdinal("SubMenuName");
                var subMenuCodeOrd = reader.GetOrdinal("SubMenuCode");
                var subMenuIconeOrd = reader.GetOrdinal("SubMenuIcon");
                var dispOrderOrd = reader.GetOrdinal("DispOrder");
                var appAccessIdOrd = reader.GetOrdinal("AppAccessId");
                var activeOrd = reader.GetOrdinal("Active");
                var createdByOrd = reader.GetOrdinal("CreatedBy");
                var createdOnOrd = reader.GetOrdinal("CreatedDate");
                var modifiedByOrd = reader.GetOrdinal("ModifiedBy");
                var modifiedOnOrd = reader.GetOrdinal("ModifiedDate");


                while (reader.Read())
                {
                    appMenuMaster = new AppMenuMaster()
                    {
                        Id = reader.GetGuid(idOrd),
                        MainMenuCode = reader.GetString(mainMenuCodeOrd),
                        MainMenuName = reader.GetString(mainMenuNameOrd),
                        MainMenuDispOrd = reader.IsDBNull(mainMenuDispOrdOrd) ? 1 : reader.GetInt32(mainMenuDispOrdOrd),
                        MainMenuIcon = reader.IsDBNull(mainMenuIconOrd) ? "" : reader.GetString(mainMenuIconOrd),
                        SubMenuIcon = reader.IsDBNull(subMenuIconeOrd) ? "" : reader.GetString(subMenuIconeOrd),
                        SubMenuCode = reader.GetString(subMenuCodeOrd),
                        SubMenuName = reader.GetString(subMenuNameOrd),
                        DispOrder = reader.IsDBNull(dispOrderOrd) ? 1 : reader.GetInt32(dispOrderOrd),
                        AppAccessId = reader.IsDBNull(appAccessIdOrd) ? Guid.Empty : reader.GetGuid(appAccessIdOrd),
                        Active = reader.GetString(activeOrd),
                        CreatedBy = reader.GetGuid(createdByOrd),
                        CreatedDate = reader.GetDateTime(createdOnOrd),
                        ModifiedBy = reader.IsDBNull(modifiedOnOrd) ? Guid.Empty : reader.GetGuid(modifiedByOrd),
                        ModifiedDate = reader.IsDBNull(modifiedOnOrd) ? DateTime.MinValue : reader.GetDateTime(modifiedOnOrd),
                    };
                }
            }
            return appMenuMaster;
        }


        public static string InsertUpdateAppMenuMaster(AppMenuMaster menuMaster, SqlConnection connection, SqlTransaction transaction, out string menuMasterId)
        {
            menuMasterId = "";
            var sqlCommand = new SqlCommand("p_insert_upd_menuMaster_001", connection);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Transaction = transaction;

            if (menuMaster.Id != Guid.Empty)
                sqlCommand.Parameters.AddWithValue("@Id", menuMaster.Id);
            sqlCommand.Parameters.AddWithValue("@AppAccessId", menuMaster.AppAccessId);
            sqlCommand.Parameters.AddWithValue("@DispOrder", menuMaster.DispOrder);
            sqlCommand.Parameters.AddWithValue("@MainMenuCode", menuMaster.MainMenuCode);
            sqlCommand.Parameters.AddWithValue("@MainMenuDispOrd", menuMaster.MainMenuDispOrd);
            sqlCommand.Parameters.AddWithValue("@MainMenuIcon", menuMaster.MainMenuIcon);
            sqlCommand.Parameters.AddWithValue("@MainMenuName", menuMaster.MainMenuName);
            sqlCommand.Parameters.AddWithValue("@SubMenuIcon", menuMaster.SubMenuIcon);
            sqlCommand.Parameters.AddWithValue("@SubMenuName", menuMaster.SubMenuName);
            sqlCommand.Parameters.AddWithValue("@SubMenuCode", menuMaster.SubMenuCode);
            sqlCommand.Parameters.AddWithValue("@Active", menuMaster.Active);
            sqlCommand.Parameters.AddWithValue("@CreatedBy", menuMaster.CreatedBy);
            sqlCommand.Parameters.AddWithValue("@ModifiedBy", menuMaster.ModifiedBy);
            if (menuMaster.ModifiedDate > DateTime.MinValue)
                sqlCommand.Parameters.AddWithValue("@ModifiedOn", menuMaster.ModifiedDate);

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
            if (status == APPMessageKey.DATASAVESUCSS) menuMasterId = sqlCommand.Parameters["@recordId"].Value.ToString();
          
            return status;
        }

    }
}
