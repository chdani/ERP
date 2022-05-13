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
    public static class Insert_Log
    {
        public static bool InsertActivityLog(ActivityLog activityLog, SqlConnection connection)
        {
            var sqlCommand = new SqlCommand("p_Insert_ActivityLog_001", connection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            //if (activityLog.@ActivityType != Guid.Empty)
            //    sqlCommand.Parameters.AddWithValue("@ActivityType", activityLog.ActivityType);

            //if (activityLog.UserId != Guid.Empty)
            sqlCommand.Parameters.AddWithValue("@UserId", activityLog.UserId);
            if (!string.IsNullOrEmpty(activityLog.Host))
                sqlCommand.Parameters.AddWithValue("@Host", activityLog.Host);
            if (!string.IsNullOrEmpty(activityLog.Headers))
                sqlCommand.Parameters.AddWithValue("@Headers", activityLog.Headers);
            if (!string.IsNullOrEmpty(activityLog.RequestBody))
                sqlCommand.Parameters.AddWithValue("@RequestBody", activityLog.RequestBody);
            if (!string.IsNullOrEmpty(activityLog.RequestMethod))
                sqlCommand.Parameters.AddWithValue("@RequestMethod", activityLog.RequestMethod);
            if (!string.IsNullOrEmpty(activityLog.UserHostAddress))
                sqlCommand.Parameters.AddWithValue("@UserHostAddress", activityLog.UserHostAddress);
            if (!string.IsNullOrEmpty(activityLog.UserAgent))
                sqlCommand.Parameters.AddWithValue("@UserAgent", activityLog.UserAgent);

            if (!string.IsNullOrEmpty(activityLog.AbsoluteURI))
                sqlCommand.Parameters.AddWithValue("@AbsoluteURI", activityLog.AbsoluteURI);
            if (activityLog.RequestedOn != DateTime.MinValue)
                sqlCommand.Parameters.AddWithValue("@RequestedOn", activityLog.RequestedOn);
            else
                sqlCommand.Parameters.AddWithValue("@RequestedOn", DateTime.Now);

            var result = sqlCommand.ExecuteNonQuery();
            return result > 0;
        }

        public static bool InsertExceptionLog(ExceptionLog exceptionLog, SqlConnection connection)
        {
            var sqlCommand = new SqlCommand("p_Insert_ExceptionLog_001", connection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            //if (exceptionLog.Id != Guid.Empty)
            //    sqlCommand.Parameters.AddWithValue("@Id", exceptionLog.Id);
            if (!string.IsNullOrEmpty(exceptionLog.ExceptionMessage))
                sqlCommand.Parameters.AddWithValue("@ExceptionMessage", exceptionLog.ExceptionMessage);
            if (!string.IsNullOrEmpty(exceptionLog.InnerException))
                sqlCommand.Parameters.AddWithValue("@InnerException", exceptionLog.InnerException);
            if (!string.IsNullOrEmpty(exceptionLog.StackTrace))
                sqlCommand.Parameters.AddWithValue("@StackTrace", exceptionLog.StackTrace);
            if (exceptionLog.ExceptionOccurredAt != DateTime.MinValue)
                sqlCommand.Parameters.AddWithValue("@ExceptionOccurredAt", exceptionLog.ExceptionOccurredAt);
            else
                sqlCommand.Parameters.AddWithValue("@ExceptionOccurredAt", DateTime.Now);

            var result = sqlCommand.ExecuteNonQuery();
            return result > 0;
        }
    }

}