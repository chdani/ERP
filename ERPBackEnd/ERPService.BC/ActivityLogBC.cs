using ERPService.Common;
using ERPService.Common.Interfaces;
using ERPService.Common.Shared;
using ERPService.Data;
using ERPService.Data.StoredProcedures;
using ERPService.DataModel.CTO;
using ERPService.DataModel.DTO;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;

namespace ERPService.BC
{
    public class ActivityLogBC
    {
        //private IRepository _repository;
        public ActivityLogBC()
        {
            //_repository = repository;
        }

        public AppResponse InsertActvityLog(ActivityLog activityLog)
        {
            AppResponse appResponse = new AppResponse();
            try
            {
                //_repository.Add(activityLog, false);
                //_repository.SaveChanges();

                using (var connection = new SqlConnection(ERPSettings.ConnectionString))
                {
                    connection.Open();
                    try
                    {
                        var result = Insert_Log.InsertActivityLog(activityLog, connection);
                        if (result)
                            appResponse.Status = APPMessageKey.DATASAVESUCSS;
                        else
                            appResponse.Status = APPMessageKey.ONEORMOREERR;
                    }
                    catch (Exception ex)
                    {
                        //Ignored
                    }
                    finally
                    {
                        connection.Close();
                    }

                }
            }
            catch (Exception ex)
            {
                appResponse.Status = APPMessageKey.ONEORMOREERR;
                appResponse.Messages.Add(ex.Message);
                throw;
            }
            return appResponse;
        }

        public AppResponse InsertExceptionLog(ExceptionLog exceptionLog)
        {
            AppResponse appResponse = new AppResponse();
            try
            {
                //_repository.Add(activityLog, false);
                //_repository.SaveChanges();

                using (var connection = new SqlConnection(ERPSettings.ConnectionString))
                {
                    connection.Open();
                    try
                    {
                        var result = Insert_Log.InsertExceptionLog(exceptionLog, connection);
                        if (result)
                            appResponse.Status = APPMessageKey.DATASAVESUCSS;
                        else
                            appResponse.Status = APPMessageKey.ONEORMOREERR;
                    }
                    catch (Exception ex)
                    {
                        //Ignored
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                appResponse.Status = APPMessageKey.ONEORMOREERR;
                appResponse.Messages.Add(ex.Message);
            }
            return appResponse;
        }

        public ActivityLogRes GetActvityLog(ActivityLogVM activityLog)
        {
            ActivityLogRes actLog = new ActivityLogRes();
            using (var connection = new SqlConnection(ERPSettings.ConnectionString))
            {
                connection.Open();
                try
                {
                    actLog = ActivityLogData.GetActivityLog(activityLog, connection);
                }
                catch (Exception ex)
                {
                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
            return actLog;
        }

        public ExceptionLogRes GetExceptionLog(ActivityLogVM exceptionLog)
        {
            ExceptionLogRes exLog = new ExceptionLogRes();
            using (var connection = new SqlConnection(ERPSettings.ConnectionString))
            {
                connection.Open();
                try
                {
                    exLog = ActivityLogData.GetExceptionLog(exceptionLog, connection);
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
            return exLog;
        }
    }
}