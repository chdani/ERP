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
    public class UserRoleMapBC
    {
        private ILogger _logger;
        private IRepository _repository;
        public UserRoleMapBC(ILogger logger, IRepository repository)
        {
            _logger = logger;
            _repository = repository;

        }

        public UserRoleMapBC(ILogger logger)
        {
            _logger = logger;
        }

        public UserRoleMapping SaveUserRoleMap(UserRoleMapping userRoleMap)
        {
            UserRoleMapping savedUserRoleMap = null;
            if (userRoleMap != null)
            {
                if (userRoleMap.UserId == Guid.Empty || userRoleMap.UserRoleId == Guid.Empty)
                {
                    var validations = new AppResponse()
                    {
                        Messages = new List<string>(),
                        Status = APPMessageKey.MANDMISSING
                    };
                    validations.Messages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.MANDMISSING]);
                    savedUserRoleMap = new UserRoleMapping()
                    {
                        Validations = validations
                    };
                    return savedUserRoleMap;
                }
                using (var connection = new SqlConnection(ERPSettings.ConnectionString))
                {
                    connection.Open();
                    try
                    {
                        using (var transaction = connection.BeginTransaction())
                        {
                            var userRoleMapId = "";
                            var status = UserRoleMappingData.InsertUpdateUserRoleMap(userRoleMap, connection, transaction, out userRoleMapId);

                            if (status == APPMessageKey.DATASAVESUCSS)
                            {
                                transaction.Commit();
                                savedUserRoleMap = UserRoleMappingData.getUserRoleMapById(userRoleMapId, connection);
                            }
                            else if (status == APPMessageKey.DUPLICATE)
                            {
                                var validations = new AppResponse()
                                {
                                    Messages = new List<string>(),
                                    Status = APPMessageKey.DUPLICATE
                                };
                                validations.Messages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.DUPLICATE]);
                                savedUserRoleMap = new UserRoleMapping()
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
                                savedUserRoleMap = new UserRoleMapping()
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
                        savedUserRoleMap = new UserRoleMapping()
                        {
                            Validations = validations
                        };
                        _logger.Error(String.Format(ERPExceptions.APP_MESSAGES[APPMessageKey.SAVEUSERROLEMAPERR], ex.StackTrace));
                    }
                    connection.Close();
                }
            }
            return savedUserRoleMap;
        }

        public UserRoleMapping GetUserRoleMapById(Guid userRoleId)
        {
            UserRoleMapping userRoleMap = null;
            if (userRoleId != null)
            {
                using (var connection = new SqlConnection(ERPSettings.ConnectionString))
                {
                    connection.Open();
                    try
                    {
                        userRoleMap = UserRoleMappingData.getUserRoleMapById(userRoleId.ToString(), connection);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(String.Format(ERPExceptions.APP_MESSAGES[APPMessageKey.GETUSERROLEMAPIDERR], ex.StackTrace));
                    }
                    connection.Close();
                }
            }
            return userRoleMap;
        }
        public List<UserRoleMapping> GetUserRoleMapList(UserRoleMapping userRoleMap)
        {
            List<UserRoleMapping> userRoleMapList = null;
            if (userRoleMap != null)
            {
                using (var connection = new SqlConnection(ERPSettings.ConnectionString))
                {
                    connection.Open();
                    try
                    {
                        userRoleMapList = UserRoleMappingData.getuserRoleMapByCriteria(userRoleMap, connection);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(String.Format(ERPExceptions.APP_MESSAGES[APPMessageKey.GETUSERROLEMAPLISERR], ex.StackTrace));
                    }
                    connection.Close();
                }
            }
            return userRoleMapList;
        }
        public UserRoleMapping GetUserRoleMapByIdEf(Guid userRoleMapId)
        {
            return _repository.GetById<UserRoleMapping>(userRoleMapId);
        }
    }
}
