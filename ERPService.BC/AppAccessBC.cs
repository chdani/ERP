using ERPService.BC.Utility;
using ERPService.Common;
using ERPService.Common.Interfaces;
using ERPService.Common.Shared;
using ERPService.Data;
using ERPService.DataModel;
using ERPService.DataModel.DTO;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Caching;

namespace ERPService.BC
{
    public class AppAccessBC
    {
        private ILogger _logger;
        private IRepository _repository;
        public AppAccessBC(ILogger logger, IRepository repository)
        {
            _logger = logger;
            _repository = repository;

        }

        public AppAccessBC(ILogger logger)
        {
            _logger = logger;
        }

        public AppAccess SaveAppAccess(AppAccess AppAccess)
        {
            AppAccess savedAppAccess = null;
            if (AppAccess != null)
            {
                if (string.IsNullOrEmpty(AppAccess.AccessCode) || string.IsNullOrEmpty(AppAccess.AccessName) || string.IsNullOrEmpty(AppAccess.AccessType))
                {
                    var validations = new AppResponse()
                    {
                        Messages = new List<string>(),
                        Status = APPMessageKey.MANDMISSING
                    };
                    validations.Messages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.MANDMISSING]);
                    savedAppAccess = new AppAccess()
                    {
                        Validations = validations
                    };
                    return savedAppAccess;
                }
                using (var connection = new SqlConnection(ERPSettings.ConnectionString))
                {
                    connection.Open();
                    try
                    {
                        using (var transaction = connection.BeginTransaction())
                        {
                            var appAccessId = "";
                            var status = AppAccessData.InsertUpdateAppAccess(AppAccess, connection, transaction, out appAccessId);
                            if (status == APPMessageKey.DATASAVESUCSS)
                            {
                                transaction.Commit();

                                savedAppAccess = AppAccessData.GetAppAccessByid(appAccessId, connection);
                            }
                            else if (status == APPMessageKey.DUPLICATE)
                            {
                                var validations = new AppResponse()
                                {
                                    Messages = new List<string>(),
                                    Status = APPMessageKey.DUPLICATE
                                };
                                validations.Messages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.DUPLICATE]);
                                savedAppAccess = new AppAccess()
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
                                savedAppAccess = new AppAccess()
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
                        savedAppAccess = new AppAccess()
                        {
                            Validations = validations
                        };
                        _logger.Error(String.Format(ERPExceptions.APP_MESSAGES[APPMessageKey.SAVEAPPACCERR], ex.StackTrace));
                    }
                    connection.Close();
                }
            }

            //remove cache values from cache.
            AppGeneralMethods.RemoveCache(ERPCacheKey.ACCESS, _repository);
            return savedAppAccess;
        }
        public AppResponse SaveAppAccessList(List<AppAccess> appAccessList)
        {
            AppResponse appResponse = new AppResponse();
            foreach (var appAccess in appAccessList)
            {
                InsertUpdateAppAccess(appAccess, false);
                appResponse.Status = APPMessageKey.DATASAVESUCSS;

            }
            if (appResponse.Status == APPMessageKey.DATASAVESUCSS)
            {
                try
                {
                    _repository.SaveChanges();
                    AppGeneralMethods.RemoveCache(ERPCacheKey.ACCESS, _repository);
                    appResponse.Status = APPMessageKey.DATASAVESUCSS;
                    appResponse.Messages = new List<string>();
                }
                catch (Exception ex)
                {
                    appResponse.Status = APPMessageKey.ONEORMOREERR;
                    appResponse.Messages = new List<string>();
                    appResponse.Messages.Add(ex.Message);
                }
            }
            return appResponse;
        }
        public AppResponse SaveAppMessageList(List<AppMessage> appMessageList)
        {
            AppResponse appResponse = new AppResponse();
            foreach (var appMessage in appMessageList)
            {
                InsertUpdateAppMessage(appMessage, false);
                appResponse.Status = APPMessageKey.DATASAVESUCSS;

            }
            if (appResponse.Status == APPMessageKey.DATASAVESUCSS)
            {
                try
                {
                    _repository.SaveChanges();
                    AppGeneralMethods.RemoveCache(ERPCacheKey.APPMESSAGE, _repository);
                    appResponse.Status = APPMessageKey.DATASAVESUCSS;
                    appResponse.Messages = new List<string>();
                }
                catch (Exception ex)
                {
                    appResponse.Status = APPMessageKey.ONEORMOREERR;
                    appResponse.Messages = new List<string>();
                    appResponse.Messages.Add(ex.Message);
                }
            }
            return appResponse;
        }
        private void InsertUpdateAppAccess(AppAccess appAccess, bool saveChanges)
        {

            if (appAccess.Id == Guid.Empty)
            {
                appAccess.Id = Guid.NewGuid();
                _repository.Add(appAccess);
            }
            else
                _repository.Update(appAccess);
            if (saveChanges)
                _repository.SaveChanges();

        }
        private void InsertUpdateAppMessage(AppMessage appMessage, bool saveChanges)
        {

            if (appMessage.Id == Guid.Empty)
            {
                appMessage.Id = Guid.NewGuid();
                _repository.Add(appMessage);
            }
            else
                _repository.Update(appMessage);
            if (saveChanges)
                _repository.SaveChanges();

        }
        public AppAccess GetAppAccessById(Guid appAccessId)
        {
            AppAccess appAccess = null;
            if (appAccessId != null)
            {
                using (var connection = new SqlConnection(ERPSettings.ConnectionString))
                {
                    connection.Open();
                    try
                    {
                        appAccess = AppAccessData.GetAppAccessByid(appAccessId.ToString(), connection);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(String.Format(ERPExceptions.APP_MESSAGES[APPMessageKey.GETAPPACCBYIDERR], ex.StackTrace));
                    }
                    connection.Close();
                }
            }
            return appAccess;
        }
        public List<AppAccess> GetAppAccessList(AppAccess appAccess)
        {
            List<AppAccess> appAccessList = null;
            if (appAccess != null)
            {
                using (var connection = new SqlConnection(ERPSettings.ConnectionString))
                {
                    connection.Open();
                    try
                    {
                        appAccessList = AppAccessData.GetAppAccessList(appAccess, connection);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(String.Format(ERPExceptions.APP_MESSAGES[APPMessageKey.GETAPPACCLISTERR], ex.StackTrace));
                    }
                    connection.Close();
                }
            }
            return appAccessList;
        }

        public AppAccess GetAppAccessEFById(Guid appAccessId)
        {
            var userMaster = _repository.GetById<AppAccess>(appAccessId);
            return userMaster;
        }

        public List<AppAccess> GetAppAccessList()
        {
            return _repository.GetQuery<AppAccess>().Where(a => a.Active == "Y").ToList();
        }


        public List<ApprovalAccessDetails> GetApprovalAccessDetail(List<Guid?> EmployeeIds, string  ScreenUrls ) {
            List<ApprovalAccessDetails> approvalAccessDetails = new List<ApprovalAccessDetails>();
            var appAccesses = _repository.GetQuery<AppAccess>();
            var appAccessRoleMap = _repository.GetQuery<AppAccessRoleMapping>();
            var userRoleMap = _repository.GetQuery<UserRoleMapping>();
            var userMaster = _repository.GetQuery<UserMaster>();
            var employee = _repository.GetQuery<Employee>();


             



            var accessDetailQuery =
                                  (
                                      from aa in appAccesses
                                      join arm in appAccessRoleMap on aa.Id equals arm.AppAccessId
                                      join urm in userRoleMap on arm.UserRoleId equals urm.UserRoleId
                                      join um in userMaster on urm.UserId equals um.Id
                                      join ems in employee on um.EmployeeId equals ems.Id

                                  
                                      where

                                                          aa.Active == "Y"
                                                           
                                                         && ems.Active == "Y"

                                                         && arm.Active == "Y"

                                                         && urm.Active == "Y"

                                                         && arm.AllowApprove == "Y"
                                                      
                                                         &&  EmployeeIds.Contains(      um.EmployeeId )
                                                         && aa.AccessCode == ScreenUrls


                                      select new
                                      {



                                          EmployeeId = ems.Id,
                                          UserName = um.UserName,
                                          AllowAdd = arm.AllowAdd,
                                          AllowApprove = arm.AllowApprove,
                                          AllowEdit = arm.AllowEdit,
                                          AllowDelete = arm.AllowDelete,
                                          Email = ems.Email,
                                          PhoneNumber = ems.PhoneNumber });







            var qryResult =  accessDetailQuery.ToList();

            foreach (var Result in qryResult)
            {

                ApprovalAccessDetails approvalsAccess = new ApprovalAccessDetails
                {

                    UserName = Result.UserName,
                    AllowAdd = Result.AllowAdd,
                    AllowApprove = Result.AllowApprove,
                    AllowEdit = Result.AllowEdit,
                    AllowDelete = Result.AllowDelete,
                    Email = Result.Email,
                };

                approvalAccessDetails.Add(approvalsAccess);





            }





            return approvalAccessDetails;








        }
    }
}
