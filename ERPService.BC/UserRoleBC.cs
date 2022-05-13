using ERPService.BC.Utility;
using ERPService.Common;
using ERPService.Common.Interfaces;
using ERPService.Common.Shared;
using ERPService.Data;
using ERPService.DataModel.CTO;
using ERPService.DataModel.DTO;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Caching;

namespace ERPService.BC
{
    public class UserRoleBC
    {
        private ILogger _logger;
        private IRepository _repository;
        public UserRoleBC(ILogger logger, IRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public UserRoleBC(ILogger logger)
        {
            _logger = logger;
        }

        public UserRole SaveUserRole(UserRole userRole)
        {
            UserRole userRoleSaved = null;
            if (userRole != null)
            {
                if (string.IsNullOrEmpty(userRole.RoleCode) || string.IsNullOrEmpty(userRole.RoleName))
                {
                    var validations = new AppResponse()
                    {
                        Messages = new List<string>(),
                        Status = ERPExceptions.APP_MANDATORY_MISSING.Key
                    };
                    validations.Messages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.CODEORNAMEMISSING]);
                    userRoleSaved = new UserRole()
                    {
                        Validations = validations
                    };
                    return userRoleSaved;
                }

                using (var connection = new SqlConnection(ERPSettings.ConnectionString))
                {
                    connection.Open();
                    try
                    {
                        using (var transaction = connection.BeginTransaction())
                        {
                            var userRoleId = "";
                            var status = UserRoleData.InsertUpdateUserRole(userRole, connection, transaction, out userRoleId);
                            if (status == APPMessageKey.DATASAVESUCSS)
                            {
                                transaction.Commit();
                                userRoleSaved = UserRoleData.getUserRoleById(userRoleId, connection);
                            }
                            else if (status == APPMessageKey.DUPLICATE)
                            {
                                var validations = new AppResponse()
                                {
                                    Messages = new List<string>(),
                                    Status = APPMessageKey.DUPLICATE
                                };
                                validations.Messages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.DUPLICATE]);
                                userRoleSaved = new UserRole()
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
                                userRoleSaved = new UserRole()
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
                            Status = ERPExceptions.EXCEPTION_UNHDLD.Key
                        };
                        validations.Messages.Add(String.Format(ERPExceptions.APP_MESSAGES[APPMessageKey.UNHDLDEX], ex.StackTrace));
                        userRoleSaved = new UserRole()
                        {
                            Validations = validations
                        };

                        _logger.Error(String.Format(ERPExceptions.APP_MESSAGES[APPMessageKey.SAVEUSERROLEERR], ex.StackTrace));
                    }
                    connection.Close();
                }
            }
            return userRoleSaved;
        }

        public UserRole GetUserRoleById(Guid userRoleId)
        {
            UserRole userRole = null;
            if (userRoleId != null)
            {
                using (var connection = new SqlConnection(ERPSettings.ConnectionString))
                {
                    connection.Open();
                    try
                    {
                        userRole = UserRoleData.getUserRoleById(userRoleId.ToString(), connection);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(String.Format(ERPExceptions.APP_MESSAGES[APPMessageKey.GETUSERROLEBYIDERR], ex.StackTrace));
                    }
                    connection.Close();
                }
            }
            return userRole;
        }
        //public List<UserRole> GetUserRoleList(UserRole userRole)
        //{
        //    List<UserRole> userRoleList = null;
        //    if (userRole != null)
        //    {
        //        using (var connection = new SqlConnection(ERPSettings.ConnectionString))
        //        {
        //            connection.Open();
        //            try
        //            {
        //                userRoleList = UserRoleData.getuserRoleByCriteria(userRole, connection);
        //            }
        //            catch (Exception ex)
        //            {
        //                _logger.Error("Method :BC.GetUserRoleList, Error : " + ex.StackTrace);
        //            }
        //            connection.Close();
        //        }
        //    }
        //    return userRoleList;
        //}

        public List<UserRole> GetUserRoleList()
        {
            return _repository.GetQuery<UserRole>().Where(a => a.Active == "Y").ToList();
        }

        public List<UserRoleMapping> GetUserRoleMasterEFById(Guid userMasterId)
        {
            List<UserRoleMapping> roleMapping = new List<UserRoleMapping>();
            //var userMaster = _repository.GetById<UserMaster>(userMasterId);
            var userRole = _repository.GetQuery<UserRoleMapping>().Where(a => a.UserId == userMasterId && a.Active == "Y").ToList();
            if (userRole != null)
            {
                foreach (var item in userRole)
                {
                    var name = _repository.GetQuery<UserRole>().Where(a => a.Id == item.UserRoleId).Select(a => a.RoleName).FirstOrDefault();
                    if (name != "")
                    {
                        roleMapping.Add(new UserRoleMapping
                        {
                            Id = item.Id,
                            UserId = item.UserId,
                            UserRoleId = item.UserRoleId,
                            Name = name
                        });
                    }
                }
            }
            return roleMapping;
        }

        public UserRole GetUserRoleByIdEf(Guid userRoleId)
        {
            var userRole = _repository.GetById<UserRole>(userRoleId);
            var roleMapping = _repository.GetQuery<AppAccessRoleMapping>().Where(a => a.UserRoleId == userRoleId && a.Active == "Y").ToList();
            var appAccess = _repository.GetQuery<AppAccess>();
            List<AppAccessMapping> map = new List<AppAccessMapping>();
            if (roleMapping != null)
            {
                foreach (var role in roleMapping)
                {
                    var access = appAccess.FirstOrDefault(a => a.Id == role.AppAccessId);
                    map.Add(new AppAccessMapping
                    {
                        id = role.AppAccessId,
                        name = access != null ? access.AccessName : "",
                        add = role.AllowAdd == "Y" ? true : false,
                        edit = role.AllowEdit == "Y" ? true : false,
                        delete = role.AllowDelete == "Y" ? true : false,
                        approve = role.AllowApprove == "Y" ? true : false,
                        view = true
                    });
                }
            }
            userRole.UserScreenAccess = map;
            return userRole;
        }

        public List<AppAccessMapping> GetAppAccessRoleMappingByRoleId(Guid userRoleId)
        {
            var roleMapping = _repository.GetQuery<AppAccessRoleMapping>().Where(a => a.UserRoleId == userRoleId && a.Active == "Y").ToList();
            var appAccess = _repository.GetQuery<AppAccess>();
            List<AppAccessMapping> mapping = new List<AppAccessMapping>();
            if (roleMapping != null)
            {
                foreach (var role in roleMapping)
                {
                    mapping.Add(new AppAccessMapping
                    {
                        id = role.AppAccessId,
                        name = appAccess.Where(a => a.Id == role.AppAccessId).FirstOrDefault().AccessName,
                        add = role.AllowAdd == "Y" ? true : false,
                        edit = role.AllowEdit == "Y" ? true : false,
                        delete = role.AllowDelete == "Y" ? true : false,
                        approve = role.AllowApprove == "Y" ? true : false,
                        view = role.Active == "Y" ? true : false,

                    });
                }
            }

            return mapping;
        }
        public AppResponse SaveUserRoleEF(UserRole userRole)
        {

            AppResponse appResponse = new AppResponse();
            List<String> validationMessages = new List<string>();

            bool validation = true;

            if (string.IsNullOrEmpty(userRole.RoleCode) || string.IsNullOrEmpty(userRole.RoleName))
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.MANDMISSING]);
                validation = false;
            }

            var user = _repository.GetQuery<UserRole>().Where(a => a.Id != userRole.Id && (a.RoleCode == userRole.RoleCode || a.RoleName == userRole.RoleName) && a.Active == "Y").FirstOrDefault();
            if (user != null)
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.DUPLICATE]);
                validation = false;
            }

            if (userRole != null && validation)
            {
                var userRoleObj = new UserRole();
                //Setting Value for Update
                if (userRole.Id != null && userRole.Id != Guid.Empty)
                    userRoleObj = _repository.GetQuery<UserRole>().Where(a => a.Id == userRole.Id).FirstOrDefault();

                userRoleObj.RoleCode = userRole.RoleCode;
                userRoleObj.RoleName = userRole.RoleName;

                if (userRole.Id == Guid.Empty)
                    userRoleObj.Active = "Y";

                //if (userRole.Id != null && userRole.Id != Guid.Empty)
                //    userRoleObj.ModifiedBy = new Guid("5158D424-B0AF-4F06-8302-69D767DB6C9D");
                //else
                //    userRoleObj.Active = "Y";

                InsertUpdateUser(userRoleObj, true);
                Guid newRoleId = userRoleObj.Id;


                //If Update Delete previous RoleMapping
                var existingRole = new List<AppAccessRoleMapping>();
                if (userRole.Id != Guid.Empty)
                {
                    existingRole = _repository.GetQuery<AppAccessRoleMapping>().Where(a => a.UserRoleId == userRole.Id).ToList();

                    foreach (var item in existingRole)
                    {
                        _repository.Delete<AppAccessRoleMapping>(item);
                        try
                        {
                            _repository.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            throw;
                        }
                    }
                }

                //Insert AppAccess -- If null no role will be mapped
                if (userRole.UserScreenAccess != null)
                {
                    AppAccessRoleMapping roleMap = new AppAccessRoleMapping();
                    foreach (var item in userRole.UserScreenAccess)
                    {
                        roleMap = new AppAccessRoleMapping
                        {
                            AppAccessId = item.id,
                            UserRoleId = newRoleId,
                            AllowAdd = item.add ? "Y" : "N",
                            AllowEdit = item.edit ? "Y" : "N",
                            AllowDelete = item.delete ? "Y" : "N",
                            AllowApprove = item.approve ? "Y" : "N",
                            Active = item.view ? "Y" : "N",
                        };
                        InsertUpdateAppAccessRoleMap(roleMap);
                    }
                }
                appResponse.Status = APPMessageKey.DATASAVESUCSS;
            }
            else
            {
                appResponse.Status = APPMessageKey.ONEORMOREERR;
                appResponse.Messages = validationMessages;
            }

            return appResponse;
        }
        public AppResponse SaveUserRoleList(List<UserRole> userRoleList)
        {
            AppResponse appResponse = new AppResponse();
            foreach (var userRole in userRoleList)
            {
                InsertUpdateUser(userRole, false);
                appResponse.Status = APPMessageKey.DATASAVESUCSS;

            }
            if (appResponse.Status == APPMessageKey.DATASAVESUCSS)
            {
                try
                {
                    _repository.SaveChanges();
                    AppGeneralMethods.RemoveCache(ERPCacheKey.ROLES, _repository);
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
        private void InsertUpdateAppAccessRoleMap(AppAccessRoleMapping access)
        {
            if (access.Id == Guid.Empty)
            {
                access.Id = Guid.NewGuid();
                _repository.Add(access, false);
            }
            else
                _repository.Update(access, true);
            try
            {
                _repository.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        private void InsertUpdateUser(UserRole userRole, bool saveChanges)
        {
            if (userRole.Id == Guid.Empty)
            {
                userRole.Id = Guid.NewGuid();
                _repository.Add(userRole, false);
            }
            else
                _repository.Update(userRole, true);
            if (saveChanges)
            {
                _repository.SaveChanges();
            }
        }
        public AppResponse MarkRoleInactive(Guid id)
        {
            AppResponse appResponse = new AppResponse();
            List<String> validationMessages = new List<string>();

            bool validation = true;
            var user = _repository.GetQuery<UserRole>().Where(a => a.Id == id).FirstOrDefault();
            if (user == null || user.Id == Guid.Empty)
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.RECNOTFOUND]);
                validation = false;
            }

            if (user != null && validation)
            {
                user.Active = "N";
                //if (user.Id != null && user.Id != Guid.Empty)
                //    user.ModifiedBy = new Guid("5158D424-B0AF-4F06-8302-69D767DB6C9D");

                InsertUpdateUser(user, true);
                appResponse.Status = APPMessageKey.DATASAVESUCSS;
            }
            else
            {
                appResponse.Status = APPMessageKey.ONEORMOREERR;
                appResponse.Messages = validationMessages;
            }

            return appResponse;
        }
    }
}
