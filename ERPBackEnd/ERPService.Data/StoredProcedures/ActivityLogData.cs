using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERPService.DataModel.DTO;
using ERPService.DataModel.CTO;

namespace ERPService.Data.StoredProcedures
{
    public static class ActivityLogData
    {
        public static ActivityLogRes GetActivityLog(ActivityLogVM activityLog, SqlConnection connection)
        {
            List<ActivityLog> activityLogList = null;
            ActivityLogRes activityLogRes = new ActivityLogRes();

            DateTime From = DateTime.MinValue, To = DateTime.MinValue;

            if (!string.IsNullOrEmpty(activityLog.FromDate))
                From = DateTime.ParseExact(activityLog.FromDate, "yyyy-MM-dd",
                                       System.Globalization.CultureInfo.InvariantCulture);

            if (!string.IsNullOrEmpty(activityLog.ToDate))
                To = DateTime.ParseExact(activityLog.ToDate, "yyyy-MM-dd",
                                       System.Globalization.CultureInfo.InvariantCulture);

            var sqlCommand = new SqlCommand("p_get_ActivityLog_001", connection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            if (!string.IsNullOrEmpty(activityLog.LogType))
                sqlCommand.Parameters.AddWithValue("@LogType", activityLog.LogType);
            if (From != DateTime.MinValue)
                sqlCommand.Parameters.AddWithValue("@FromDate", From == null ? DateTime.Now : From);
            if (To != DateTime.MinValue)
                sqlCommand.Parameters.AddWithValue("@ToDate", To == null ? DateTime.Now : To);
            //Page From and To
            sqlCommand.Parameters.AddWithValue("@PageFrom", activityLog.PageFrom);
            sqlCommand.Parameters.AddWithValue("@PageTo", activityLog.PageTo);

            sqlCommand.Parameters.Add("@RowCount", SqlDbType.Int);
            sqlCommand.Parameters["@RowCount"].Direction = ParameterDirection.Output;

            var reader = sqlCommand.ExecuteReader();

            if (reader.HasRows)
            {
                var UserId = reader.GetOrdinal("UserId");
                var Host = reader.GetOrdinal("Host");
                var Headers = reader.GetOrdinal("Headers");
                var RequestBody = reader.GetOrdinal("RequestBody");
                var RequestMethod = reader.GetOrdinal("RequestMethod");
                var UserHostAddress = reader.GetOrdinal("UserHostAddress");
                var UserAgent = reader.GetOrdinal("UserAgent");
                var AbsoluteURI = reader.GetOrdinal("AbsoluteURI");
                var RequestedOn = reader.GetOrdinal("RequestedOn");
                var FirstName = reader.GetOrdinal("FirstName");
                var LastName = reader.GetOrdinal("LastName");
                activityLogList = new List<ActivityLog>();

                while (reader.Read())
                {
                    var actLog = new ActivityLog()
                    {
                        UserId = reader.IsDBNull(UserId) ? Guid.Empty : reader.GetGuid(UserId),
                        Host = reader.IsDBNull(Host) ? "" : reader.GetString(Host),
                        Headers = reader.IsDBNull(Headers) ? "" : reader.GetString(Headers),
                        RequestBody = reader.IsDBNull(RequestBody) ? "" : reader.GetString(RequestBody),
                        RequestMethod = reader.IsDBNull(RequestMethod) ? "" : reader.GetString(RequestMethod),
                        UserHostAddress = reader.IsDBNull(UserHostAddress) ? "" : reader.GetString(UserHostAddress),
                        AbsoluteURI = reader.IsDBNull(AbsoluteURI) ? "" : reader.GetString(AbsoluteURI),
                        MethodName  = reader.IsDBNull(AbsoluteURI) ? "" : Common.Utilities.GetMethodNameFromURL(reader.GetString(AbsoluteURI)),
                        RequestedOn = reader.IsDBNull(RequestedOn) ? DateTime.MinValue : reader.GetDateTime(RequestedOn),
                        FirstName = reader.IsDBNull(FirstName) ? "" : reader.GetString(FirstName),
                        LastName = reader.IsDBNull(LastName) ? "" : reader.GetString(LastName),
                    };
                    activityLogList.Add(actLog);
                }
            }

            reader.Close();
            int rowCount = 0;
            if (sqlCommand.Parameters["@RowCount"].Value != null)
                rowCount = Convert.ToInt32(sqlCommand.Parameters["@RowCount"].Value);

            activityLogRes.Activity = activityLogList;
            activityLogRes.RowCount = rowCount;

            return activityLogRes;
        }

        public static ExceptionLogRes GetExceptionLog(ActivityLogVM activityLog, SqlConnection connection)
        {
            List<ExceptionLog> exceptionLogList = null;
            ExceptionLogRes exceptionLogRes = new ExceptionLogRes();

            DateTime From = DateTime.MinValue, To = DateTime.MinValue;
            if (!string.IsNullOrEmpty(activityLog.FromDate))
                From = DateTime.ParseExact(activityLog.FromDate, "yyyy-MM-dd",
                                       System.Globalization.CultureInfo.InvariantCulture);

            if (!string.IsNullOrEmpty(activityLog.ToDate))
                To = DateTime.ParseExact(activityLog.ToDate, "yyyy-MM-dd",
                                       System.Globalization.CultureInfo.InvariantCulture);

            var sqlCommand = new SqlCommand("p_get_ActivityLog_001", connection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            //if (activityLog.@ActivityType != Guid.Empty)
            //    sqlCommand.Parameters.AddWithValue("@ActivityType", activityLog.ActivityType);

            if (!string.IsNullOrEmpty(activityLog.LogType))
                sqlCommand.Parameters.AddWithValue("@LogType", activityLog.LogType);
            if (From != DateTime.MinValue)
                sqlCommand.Parameters.AddWithValue("@FromDate", From == null ? DateTime.Now : From);
            if (To != DateTime.MinValue)
                sqlCommand.Parameters.AddWithValue("@ToDate", To == null ? DateTime.Now : To);
            //Page From and To
            sqlCommand.Parameters.AddWithValue("@PageFrom", activityLog.PageFrom);
            sqlCommand.Parameters.AddWithValue("@PageTo", activityLog.PageTo);

            sqlCommand.Parameters.Add("@RowCount", SqlDbType.Int);
            sqlCommand.Parameters["@RowCount"].Direction = ParameterDirection.Output;

            var reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                var id = reader.GetOrdinal("Id");
                var ExceptionMessage = reader.GetOrdinal("ExceptionMessage");
                var InnerException = reader.GetOrdinal("InnerException");
                var StackTrace = reader.GetOrdinal("StackTrace");
                var ExceptionOccurredAt = reader.GetOrdinal("ExceptionOccurredAt");
                exceptionLogList = new List<ExceptionLog>();

                while (reader.Read())
                {
                    var exLog = new ExceptionLog()
                    {
                        Id = reader.GetGuid(id),
                        ExceptionMessage = reader.IsDBNull(ExceptionMessage) ? "" : reader.GetString(ExceptionMessage),
                        InnerException = reader.IsDBNull(InnerException) ? "" : reader.GetString(InnerException),
                        StackTrace = reader.IsDBNull(StackTrace) ? "" : reader.GetString(StackTrace),
                        ExceptionOccurredAt = reader.IsDBNull(ExceptionOccurredAt) ? DateTime.MinValue : reader.GetDateTime(ExceptionOccurredAt),
                    };
                    exceptionLogList.Add(exLog);
                }
            }

            reader.Close();

            int rowCount = 0;
            if (sqlCommand.Parameters["@RowCount"].Value != null)
                rowCount = Convert.ToInt32(sqlCommand.Parameters["@RowCount"].Value);

            exceptionLogRes.Exception = exceptionLogList;
            exceptionLogRes.RowCount = rowCount;

            return exceptionLogRes;
        }
    }
}

