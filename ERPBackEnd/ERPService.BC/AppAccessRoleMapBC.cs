using ERPService.Common;
using ERPService.Common.Interfaces;
using ERPService.Common.Shared;
using ERPService.Data;
using ERPService.DataModel.DTO;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ERPService.BC
{
    public class AppAccessRoleMapBC
    {
        private ILogger _logger;
        private IRepository _repository;
        public AppAccessRoleMapBC(ILogger logger, IRepository repository)
        {
            _logger = logger;
            _repository = repository;

        }

        public AppAccessRoleMapBC(ILogger logger)
        {
            _logger = logger;
        }

        public AppAccessRoleMapping SaveAppAccessRoleMap(AppAccessRoleMapping appAccessRoleMap)
        {
            AppAccessRoleMapping savedAppAccessRoleMap = null;
            if (appAccessRoleMap != null)
            {
                if (appAccessRoleMap.AppAccessId == Guid.Empty || appAccessRoleMap.UserRoleId == Guid.Empty)
                {
                    var validations = new AppResponse()
                    {
                        Messages = new List<string>(),
                        Status = APPMessageKey.MANDMISSING
                    };
                    validations.Messages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.MANDMISSING]);
                    savedAppAccessRoleMap = new AppAccessRoleMapping()
                    {
                        Validations = validations
                    };
                    return savedAppAccessRoleMap;
                }
                using (var connection = new SqlConnection(ERPSettings.ConnectionString))
                {
                    connection.Open();
                    try
                    {
                        using (var transaction = connection.BeginTransaction())
                        {
                            var appAccessRoleMapId = "";
                            var status = AppAccessRoleMapData.InsertUpdateAppAccessRoleMap(appAccessRoleMap, connection, transaction, out appAccessRoleMapId);
                            if (status == APPMessageKey.DATASAVESUCSS)
                            {
                                transaction.Commit();

                                savedAppAccessRoleMap = AppAccessRoleMapData.getAppAccessRoleMapById(appAccessRoleMapId.ToString(), connection);
                            }
                            else if (status == APPMessageKey.DUPLICATE)
                            {
                                var validations = new AppResponse()
                                {
                                    Messages = new List<string>(),
                                    Status = APPMessageKey.DUPLICATE
                                };
                                validations.Messages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.DUPLICATE]);
                                savedAppAccessRoleMap = new AppAccessRoleMapping()
                                {
                                    Validations = validations
                                };

                            }
                            else if (status == APPMessageKey.NOTLATEST)
                            {
                                var validations = new AppResponse()
                                {
                                    Messages = new List<string>(),
                                    Status = APPMessageKey.NOTLATEST
                                };
                                validations.Messages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.NOTLATEST]);
                                savedAppAccessRoleMap = new AppAccessRoleMapping()
                                {
                                    Validations = validations
                                };

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        var validations = new AppResponse()
                        {
                            Messages = new List<string>(),
                            Status = APPMessageKey.UNHDLDEX
                        };
                        validations.Messages.Add(String.Format(ERPExceptions.APP_MESSAGES[APPMessageKey.UNHDLDEX], ex.StackTrace));
                        savedAppAccessRoleMap = new AppAccessRoleMapping()
                        {
                            Validations = validations
                        };
                        _logger.Error(String.Format(ERPExceptions.APP_MESSAGES[APPMessageKey.SAVEAPPACCROLEMAP], ex.StackTrace));
                    }
                    connection.Close();
                }
            }
            return savedAppAccessRoleMap;
        }

        public AppAccessRoleMapping GetAppAcessRoleMapById(Guid appAccessRoleMapId)
        {
            AppAccessRoleMapping appAccessRoleMap = null;
            if (appAccessRoleMapId != null)
            {
                using (var connection = new SqlConnection(ERPSettings.ConnectionString))
                {
                    connection.Open();
                    try
                    {
                        appAccessRoleMap = AppAccessRoleMapData.getAppAccessRoleMapById(appAccessRoleMapId.ToString(), connection);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(String.Format(ERPExceptions.APP_MESSAGES[APPMessageKey.GETBYAPPACCROMAPBYID], ex.StackTrace));
                    }
                    connection.Close();
                }
            }
            return appAccessRoleMap;
        }
        public List<AppAccessRoleMapping> GetAppAccessRoleMapList(AppAccessRoleMapping appAccessRoleMap)
        {
            List<AppAccessRoleMapping> appAccessRoleMapList = null;
            if (appAccessRoleMap != null)
            {
                using (var connection = new SqlConnection(ERPSettings.ConnectionString))
                {
                    connection.Open();
                    try
                    {
                        appAccessRoleMapList = AppAccessRoleMapData.getAppAccessRoleMapByCriteria(appAccessRoleMap, connection);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(String.Format(ERPExceptions.APP_MESSAGES[APPMessageKey.GETAPPACCROLMAPLIST], ex.StackTrace));
                    }
                    connection.Close();
                }
            }
            return appAccessRoleMapList;
        }
        public AppAccessRoleMapping GetAppAcessRoleMapByIdEf(Guid appAccessRoleMapId)
        {
            return _repository.GetById<AppAccessRoleMapping>(appAccessRoleMapId);
        }
        public List<AppAccessRoleMapping> SaveUserRoleWithAppAccess(List<AppAccessRoleMapping> appAccessRoleMappings)
        {
            foreach (var item in appAccessRoleMappings)
            {
                if (item.Id != Guid.Empty)
                {
                    var obj = _repository.GetById<AppAccessRoleMapping>(item.Id);
                    obj.AllowDelete = item.AllowDelete;
                    obj.AllowAdd = item.AllowAdd;
                    obj.AllowEdit = item.AllowEdit;
                    obj.Active = item.Active;
                    obj.ModifiedDate = DateTime.Now;

                    _repository.Update<AppAccessRoleMapping>(obj);
                }
                else
                {
                    item.Id = Guid.NewGuid();
                    _repository.Add<AppAccessRoleMapping>(item);
                }
            }
            _repository.SaveChanges();
            return appAccessRoleMappings;
        }
    }
}
