using ERPService.Common;
using ERPService.DataModel.CTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using ERPService.Data;
using Serilog;
using ERPService.DataModel.DTO;
using ERPService.Common.Shared;
using ERPService.Common.Interfaces;
using System.Net;
using System.Net.Mail;
using System.Runtime.Caching;
using System.IO;
using ERPService.BC.Utility;

namespace ERPService.BC
{
    public class UserMasterBC
    {
        private ILogger _logger;
        private IRepository _repository;
        private ObjectCache cache = MemoryCache.Default;

        public UserMasterBC(ILogger logger, IRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public UserMasterBC(ILogger logger)
        {
            _logger = logger;
        }
        public UserMaster getResetPassword(string Key)
        {
            List<String> validationMessages = new List<string>();
            var userMaster = new UserMaster();
            var gid = cache.Get(Key);
            var userdata = (UserMaster)gid;
            if (userdata != null)
            {
                userMaster.Id = userdata.Id;
                userMaster.UserName = userdata.UserName;
                cache.Remove(Key);
            }
            else
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.SECEXP_REGENEMAIL]);

            }
            return userMaster;
        }
        public UserSetting SaveRePwd(UserMaster userInfo)
        {
            var userSetting = _repository.GetQuery<UserSetting>().Where(a => a.UserMasterId == userInfo.Id && a.ConfigKey == "PASSWORD" && a.Active == "Y").FirstOrDefault();
            if (userSetting == null)
            {
                userSetting = new UserSetting();
                userSetting.UserMasterId = userInfo.Id;
                userSetting.ConfigKey = "PASSWORD";
                userSetting.Active = "Y";
                userSetting.ConfigValue = PasswordHash.CreateHash(userInfo.Password);
            }
            else
                userSetting.ConfigValue = PasswordHash.CreateHash(userInfo.Password);

            InsertUpdateUsersetting(userSetting);
            return userSetting;
        }
        private Guid InsertUpdateUsersetting(UserSetting userSetting)
        {
            if (userSetting.Id == Guid.Empty)
            {
                userSetting.Id = Guid.NewGuid();
                _repository.Add(userSetting, false);

            }
            else
                _repository.Update(userSetting, true);

            _repository.SaveChanges();

            return userSetting.Id;
        }
        public UserInfo ValidateUserLogin(UserLogin login, bool isSSOLogin = false)
        {
            UserInfo userInfo = null;

            if (isSSOLogin)
            {
                var domainName = _repository.GetQuery<SystemSetting>().Where(a => a.ConfigKey == "DOMAIN_NAME").FirstOrDefault();
                var result = WindowsAuth.CheckIfUserActive(login.SSOUserName, domainName.ConfigValue);
                if (result)
                    login.UserName = login.SSOUserName;
                else
                {
                    var validations = new AppResponse()
                    {
                        Messages = new List<string>(),
                        Status = APPMessageKey.USERINACTIVESYS
                    };
                    validations.Messages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.USERINACTIVESYS]);
                    userInfo = new UserInfo()
                    {
                        Validations = validations
                    };
                    return userInfo;
                }
            }

            using (var connection = new SqlConnection(ERPSettings.ConnectionString))
            {
                connection.Open();
                try
                {
                    var userContext = UserMasterData.ValidateUserLogin(login, connection);

                    if (userContext != null)
                    {
                        bool isValidUser = false;
                        if (isSSOLogin)
                        {
                            //Mark valid user if Identity has been matched and data fetched for user
                            if (userContext != null)
                                isValidUser = true;
                        }
                        else if (userContext.UserType != "W" && !isSSOLogin) // if not windows authentication 
                        {
                            if (!string.IsNullOrEmpty(login.PassWord))
                            {
                                var userpwd = _repository.GetQuery<UserSetting>().Where(a => a.UserMasterId == userContext.Id && a.ConfigKey == "PASSWORD" && a.Active == "Y").FirstOrDefault();
                                var passwordCheck = PasswordHash.ValidatePassword(login.PassWord, userpwd.ConfigValue);
                                if (!passwordCheck)
                                {
                                    var validations = new AppResponse()
                                    {
                                        Messages = new List<string>(),
                                        Status = APPMessageKey.INVLOGIN
                                    };
                                    validations.Messages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.INVLOGIN]);
                                    userInfo = new UserInfo()
                                    {
                                        Validations = validations
                                    };
                                }
                                else
                                    isValidUser = true;
                            }
                            else
                            {
                                var validations = new AppResponse()
                                {
                                    Messages = new List<string>(),
                                    Status = APPMessageKey.PASSWORDREQ
                                };
                                validations.Messages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.PASSWORDREQ]);
                                userInfo = new UserInfo()
                                {
                                    Validations = validations
                                };
                            }
                        }
                        else if (userContext.UserType == "W" && !isSSOLogin) //Windows Authentication
                        {
                            var isAuthenticated = false;
                            if (userContext != null)
                            {
                                //Get Domain name from SystemSetting Tables
                                var domainName = _repository.GetQuery<SystemSetting>().Where(a => a.ConfigKey == "DOMAIN_NAME").FirstOrDefault();
                                isAuthenticated = WindowsAuth.ValidateCredentials(userContext.UserName, login.PassWord, domainName.ConfigValue);
                            }
                            if (isAuthenticated)
                                isValidUser = true;
                        }
                        else
                        {
                            var validations = new AppResponse()
                            {
                                Messages = new List<string>(),
                                Status = APPMessageKey.PASSWORDREQ
                            };
                            validations.Messages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.PASSWORDREQ]);
                            userInfo = new UserInfo()
                            {
                                Validations = validations
                            };
                        }

                        if (isValidUser)
                        {
                            var userSetting = _repository.GetQuery<UserSetting>().Where(a => a.UserMasterId == userContext.Id && a.ConfigKey == "USR_LANG" && a.Active == "Y").FirstOrDefault();

                            if (userSetting != null)
                                userContext.Language = userSetting.ConfigValue;
                            else
                                userContext.Language = "en";
                            userContext.Token = JWTToken.GenerateToken(userContext);

                            userInfo = GetUserAccessInfo(userContext.Id);
                            if (userInfo != null)
                                userInfo.UserContext = userContext;

                            else
                            {
                                userInfo = new UserInfo();
                                userInfo.UserContext = userContext;
                            }
                            userInfo.UserContext.LedgerAccounts = GetUserLedgerAccount(userContext.Id);
                            userInfo.UserContext.Organizations = GetUserOrganizationList(userContext.Id);
                        }

                    }
                    else
                    {
                        var validations = new AppResponse()
                        {
                            Messages = new List<string>(),
                            Status = APPMessageKey.PASSWORDREQ
                        };
                        validations.Messages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.INVLOGIN]);
                        userInfo = new UserInfo()
                        {
                            Validations = validations
                        };

                    }
                }
                catch (Exception ex)
                {
                    var validations = new AppResponse()
                    {
                        Messages = new List<string>(),
                        Status = APPMessageKey.UNHDLDEX
                    };
                    validations.Messages.Add(String.Format(ERPExceptions.APP_MESSAGES[APPMessageKey.UNHDLDEX], ex.Message));
                    userInfo = new UserInfo()
                    {
                        Validations = validations
                    };
                    _logger.Error(String.Format(ERPExceptions.APP_MESSAGES[APPMessageKey.BCVALIUSERLOGINERR], ex.StackTrace));
                }
                connection.Close();
            }
            return userInfo;
        }
        public List<UserOrganization> GetUserOrganizationList(Guid Id)
        {
            var userOrgList = _repository.GetQuery<UserOrganizationMap>().Where(a => a.UserID == Id).ToList();
            var OrgList = new List<UserOrganization>();
            var OrgBC = new OrganizationBC(_logger, _repository);
            var orgs = OrgBC.GetOrganizationList();

            foreach (var item in userOrgList)
            {
                var org = orgs.FirstOrDefault(a => a.Id == item.OrganizationId);
                if (org != null)
                {
                    OrgList.Add(new UserOrganization
                    {
                        Id = org.Id,
                        OrgCode = org.OrgCode,
                        OrgName = org.OrgName,
                        Location = org.Location,

                    });
                }
            }
            return OrgList;
        }
        public List<UserAccount> GetUserLedgerAccount(Guid id)
        {
            var userLedAccList = _repository.GetQuery<UserLedgerAccnt>().Where(a => a.UserID == id).ToList();
            var ledgers = new List<UserAccount>();
            var ledgerAccBC = new LedgerAccountBC(_logger, _repository);

            var ledgerAccs = ledgerAccBC.GetLedgerAccountList(new LedgerAccountSearch());
            foreach (var item in userLedAccList)
            {
                var ledgerAcc = ledgerAccs.FirstOrDefault(a => a.LedgerCode == item.AccountCode);
                if (ledgerAcc != null)
                {
                    ledgers.Add(new UserAccount
                    {
                        Id = ledgerAcc.Id,
                        LedgerCode = ledgerAcc.LedgerCode,
                        LedgerDesc = ledgerAcc.LedgerDesc,
                        UsedFor = ledgerAcc.UsedFor,
                        Remarks = ledgerAcc.Remarks
                    });
                }
            }
            return ledgers;
        }
        public UserInfo GetUserAccessInfo(Guid userId)
        {
            UserInfo userInfo = null;

            using (var connection = new SqlConnection(ERPSettings.ConnectionString))
            {
                connection.Open();
                var userRoleAccesses = UserAccessData.GetUserRoleAccess(userId, connection);
                if (userRoleAccesses != null)
                {
                    List<UserRoleAccessInfo> userRoleAccessInfos = new List<UserRoleAccessInfo>();
                    var ScreenCode = _repository.List<AppAccess>(a => a.Active == "Y");
                    ScreenCode.ForEach(q =>
                    {
                        var codes = userRoleAccesses.Where(a => a.AccessCode == q.AccessCode).ToList();
                        if (codes != null && codes.Count > 0)
                        {
                            var add = codes.FirstOrDefault(a => a.AllowAdd == "Y");
                            var edit = codes.FirstOrDefault(a => a.AllowEdit == "Y");
                            var delete = codes.FirstOrDefault(a => a.AllowDelete == "Y");
                            var approve = codes.FirstOrDefault(a => a.AllowApprove == "Y");
                            var userAcessScreen = codes.FirstOrDefault(a => a.AccessCode == q.AccessCode);
                            if (userAcessScreen != null)
                            {
                                if (add != null)
                                    userAcessScreen.AllowAdd = "Y";
                                else
                                    userAcessScreen.AllowAdd = "N";
                                if (edit != null)
                                    userAcessScreen.AllowEdit = "Y";
                                else
                                    userAcessScreen.AllowEdit = "N";
                                if (delete != null)
                                    userAcessScreen.AllowDelete = "Y";
                                else
                                    userAcessScreen.AllowDelete = "N";
                                if (approve != null)
                                    userAcessScreen.AllowApprove = "Y";
                                else
                                    userAcessScreen.AllowApprove = "N";
                                userRoleAccessInfos.Add(userAcessScreen);

                            }
                        }
                    });
                    userInfo = new UserInfo();
                    userInfo.UserRoles = userRoleAccessInfos.Where(a => a.AccessType == "C").Select(x => new { x.RoleCode, x.RoleName, x.AccessCode, x.AccessName, x.AllowAdd, x.AllowEdit, x.AllowDelete, x.AllowApprove }).Distinct().ToList().Select(x => new RoleInfo
                    {
                        RoleCode = x.RoleCode,
                        RoleName = x.RoleName,
                        AccessCode = x.AccessCode,
                        AccessName = x.AccessName,
                        AllowDelete = x.AllowDelete,
                        AllowEdit = x.AllowEdit,
                        AllowWrite = x.AllowAdd,
                        AllowApprove = x.AllowApprove
                    }).ToList();

                    userInfo.AppMenus = userRoleAccessInfos.Where(a => a.AccessType == "S").Select(x => new { x.ModuleCode, x.ModuleName, x.ModuleIcon, x.ModuleDispOrder }).Distinct().Select(a => new AppModule
                    {
                        ModuleCode = a.ModuleCode,
                        ModuleName = a.ModuleName,
                        ModuleIcon = a.ModuleIcon,
                        DisplayOrder = a.ModuleDispOrder
                    }).OrderBy(a => a.DisplayOrder).ToList();
                    foreach (var module in userInfo.AppMenus)
                    {
                        module.UserMenus = userRoleAccessInfos.Where(a => a.ModuleCode == module.ModuleCode).Select(x => new { x.MainMenuCode, x.MainMenuName, x.MainMenuIcon, x.MainMenuDispOrder }).Distinct().Select(a => new UserMenu
                        {
                            MainMenuCode = a.MainMenuCode,
                            MainMenuName = a.MainMenuName,
                            MainMenuIcon = a.MainMenuIcon,
                            DisplayOrder = a.MainMenuDispOrder
                        }).OrderBy(a => a.DisplayOrder).ToList();

                        foreach (var mainMenu in module.UserMenus)
                        {
                            mainMenu.SubMenus = userRoleAccessInfos.Where(a => a.MainMenuCode == mainMenu.MainMenuCode).OrderBy(a => a.DispOrder).Select(a =>
                                 new UserSubMenu
                                 {
                                     ScreenCode = a.AccessCode,
                                     ScreenName = a.AccessName,
                                     AllowDelete = a.AllowDelete,
                                     AllowAdd = a.AllowAdd,
                                     AllowEdit = a.AllowEdit,
                                     AllowApprove = a.AllowApprove,
                                     DisplayOrder = a.DispOrder,
                                     ScreenUrl = a.ScreenUrl,
                                     SubMenuIcon = a.SubMenuIcon,
                                     SubMenuName = a.SubMenuName,
                                     SubMmenuCode = a.SubMmenuCode,
                                     ShowFinYear = a.ShowFinYear,
                                     ShowOrg = a.ShowOrg
                                 }).ToList();
                        }
                    }

                    // userInfo.UserMenus=userInfo.UserMenus.Where(x=>x.a)
                }
            }
            return userInfo;
        }

        public AppResponse SaveUserInfo(UserMaster userInfo)
        {
            AppResponse appResponse = new AppResponse();
            List<String> validationMessages = new List<string>();

            bool validation = true;

            if (string.IsNullOrEmpty(userInfo.UserName) || string.IsNullOrEmpty(userInfo.EmailId)
                    || string.IsNullOrEmpty(userInfo.FirstName) || string.IsNullOrEmpty(userInfo.LastName)
                       || string.IsNullOrEmpty(userInfo.UserType))
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.MANDMISSING]);
                validation = false;
            }

            //if (!string.IsNullOrEmpty(userInfo.Password))
            //    userInfo.Password = PasswordHash.CreateHash(userInfo.Password);

            //Check if user Already Exist
            //if (userInfo.Id != null && userInfo.Id == Guid.Empty)
            //{
            var user = _repository.GetQuery<UserMaster>().Where(a => a.Active == "Y" && a.Id != userInfo.Id && (a.UserName == userInfo.UserName || a.EmailId == userInfo.EmailId)).FirstOrDefault();
            if (user != null)
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.DUPLIEMAILNAMEALRY]);
                validation = false;
            }
            //}

            if (userInfo != null && validation)
            {
                var userMaster = new UserMaster();
                //Setting Value for Update
                if (userInfo.Id != null && userInfo.Id != Guid.Empty)
                    userMaster = _repository.GetQuery<UserMaster>().Where(a => a.Id == userInfo.Id).FirstOrDefault();

                userMaster.FirstName = userInfo.FirstName;
                userMaster.LastName = userInfo.LastName;
                userMaster.EmailId = userInfo.EmailId;
                userMaster.EmployeeId = userInfo.EmployeeId;
                userMaster.UserName = userInfo.UserName;
                userMaster.UserType = userInfo.UserType;

                if (userInfo.Id == Guid.Empty)
                    userMaster.Active = "Y";

                //if (userInfo.Id != null && userInfo.Id != Guid.Empty)
                //    userMaster.ModifiedBy = new Guid("5158D424-B0AF-4F06-8302-69D767DB6C9D");
                //else
                //    userMaster.Active = "Y";

                var userId = InsertUpdateUser(userMaster);

                var newuserId = userMaster.Id;
                //If Update Delete previous RoleMapping
                var existingRole = new List<UserRoleMapping>();
                if (userInfo.Id != Guid.Empty)
                {
                    existingRole = _repository.GetQuery<UserRoleMapping>().Where(a => a.UserId == userInfo.Id).ToList();

                    foreach (var item in existingRole)
                    {
                        _repository.Delete<UserRoleMapping>(item);
                        _repository.SaveChanges();
                    }
                }

                //Insert AppAccess -- If null no role will be mapped
                if (userInfo.userRole != null)
                {
                    UserRoleMapping roleMap = new UserRoleMapping();
                    foreach (var item in userInfo.userRole)
                    {
                        roleMap = new UserRoleMapping
                        {
                            UserRoleId = item.Code,
                            UserId = newuserId,
                            Active = "Y"
                        };
                        InsertUpdateUserRoleMap(roleMap);
                    }
                }

                var userLedgerAcc = new List<UserLedgerAccnt>();
                if (userInfo.Id != Guid.Empty)
                {
                    userLedgerAcc = _repository.GetQuery<UserLedgerAccnt>().Where(a => a.UserID == userInfo.Id).ToList();

                    foreach (var item in userLedgerAcc)
                    {
                        _repository.Delete<UserLedgerAccnt>(item);
                        _repository.SaveChanges();
                    }
                }

                //Insert AppAccess -- If null no role will be mapped
                if (userInfo.LedgerAccnts != null)
                {
                    UserLedgerAccnt userLedgerAccnt = new UserLedgerAccnt();
                    foreach (var item in userInfo.LedgerAccnts)
                    {
                        userLedgerAccnt = new UserLedgerAccnt
                        {
                            UserID = newuserId,
                            AccountCode = item.AccountCode,
                            Active = item.Active
                        };
                        InsertUpdateUserLedgerAccnt(userLedgerAccnt);
                    }
                }

                var userOrganization = new List<UserOrganizationMap>();
                if (userInfo.Id != Guid.Empty)
                {
                    userOrganization = _repository.GetQuery<UserOrganizationMap>().Where(a => a.UserID == newuserId).ToList();

                    foreach (var item in userOrganization)
                    {
                        _repository.Delete<UserOrganizationMap>(item);
                        _repository.SaveChanges();
                    }
                }

                //Insert AppAccess -- If null no role will be mapped
                if (userInfo.Organizations != null)
                {
                    UserOrganizationMap userOrg = new UserOrganizationMap();
                    foreach (var item in userInfo.Organizations)
                    {
                        userOrg = new UserOrganizationMap
                        {
                            UserID = newuserId,
                            OrganizationId = item.Id,
                            Active = "Y"
                        };
                        InsertUpdateUserOrganization(userOrg);
                    }
                }

                appResponse.Status = APPMessageKey.DATASAVESUCSS;
                appResponse.ReferenceId = userId;
            }
            else
            {
                appResponse.Status = APPMessageKey.ONEORMOREERR;
                appResponse.Messages = validationMessages;
            }
            return appResponse;
        }
        public AppResponse SendPasswordResetMail(Guid userMasterId)
        {
            var userMaster = _repository.GetById<UserMaster>(userMasterId);
            if (userMaster != null)
            {
                if (userMaster.UserType != "W")
                {
                    Guid guid = Guid.NewGuid();
                    string key = guid.ToString();
                    UserMaster data = new UserMaster();
                    data.Id = userMaster.Id;
                    data.UserName = userMaster.UserName;
                    data.EmailId = userMaster.EmailId;
                    cache.Set(key, data, DateTime.Now.AddHours(24));
                    var semail = data.EmailId;
                    var femail = ERPSettings.APPSYSTEMSETTINGS[APPSystemsettingsKey.SENDEREMAILID];
                    var passwordGenerationLink = ERPSettings.PasswordResetLink;
                    var EmailTemlate = ERPSettings.EmailIdTemplate;
                    string body = string.Empty;
                    var root = AppDomain.CurrentDomain.BaseDirectory; using (var reader = new StreamReader(root + EmailTemlate))
                    {
                        string readFile = reader.ReadToEnd();
                        string StrContent = string.Empty;
                        StrContent = readFile;
                        StrContent = StrContent.Replace("#UserName#", userMaster.UserName);
                        StrContent = StrContent.Replace("#passwordGenerationLink#", passwordGenerationLink);
                        StrContent = StrContent.Replace("#key#", key);
                        body = StrContent.ToString();
                    }

                    MailMessage mailMessage = new MailMessage(femail, semail);
                    mailMessage.Subject = ERPSettings.EmailIdSubject;

                    mailMessage.Body = body;
                    mailMessage.IsBodyHtml = true;

                    return EmailUtility.SendEmail(mailMessage);

                }
                else
                    return new AppResponse() { Status = APPMessageKey.DATASAVESUCSS };
            }
            else
            {
                var repsonse = new AppResponse() { Status = APPMessageKey.ONEORMOREERR, Messages = new List<string>() };
                repsonse.Messages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.USERNOTFOUDMAIL]);
                return repsonse;
            }
        }
        private void InsertUpdateUserRoleMap(UserRoleMapping access)
        {
            if (access.Id == Guid.Empty)
            {
                access.Id = Guid.NewGuid();
                _repository.Add(access, false);
            }
            else
                _repository.Update(access, true);
            _repository.SaveChanges();
        }

        private void InsertUpdateUserLedgerAccnt(UserLedgerAccnt userLedgerAccnt)
        {
            if (userLedgerAccnt.Id == Guid.Empty)
            {
                userLedgerAccnt.Id = Guid.NewGuid();
                _repository.Add(userLedgerAccnt, false);
            }
            else
                _repository.Update(userLedgerAccnt, true);
            _repository.SaveChanges();
        }

        private void InsertUpdateUserOrganization(UserOrganizationMap userOrg)
        {
            if (userOrg.Id == Guid.Empty)
            {
                userOrg.Id = Guid.NewGuid();
                _repository.Add(userOrg, false);
            }
            else
                _repository.Update(userOrg, true);
            _repository.SaveChanges();
        }

        private Guid InsertUpdateUser(UserMaster userMaster)
        {
            if (userMaster.Id == Guid.Empty)
            {
                userMaster.Id = Guid.NewGuid();
                _repository.Add(userMaster, false);

            }
            else
                _repository.Update(userMaster, true);

            _repository.SaveChanges();

            return userMaster.Id;
        }

        public List<UserMaster> GetUserList(UserMaster user)
        {
            return _repository.GetQuery<UserMaster>().Where(a =>
               (string.IsNullOrEmpty(user.FirstName) || a.FirstName.Contains(user.FirstName))
              && (string.IsNullOrEmpty(user.LastName) || a.LastName.Contains(user.LastName))
              && (string.IsNullOrEmpty(user.UserName) || a.UserName.Contains(user.UserName))
              && (string.IsNullOrEmpty(user.EmailId) || a.EmailId == user.EmailId)
             && (string.IsNullOrEmpty(user.UserType) || a.UserType == user.UserType) &&
               a.Active == "Y").OrderByDescending(a => a.CreatedDate).ToList();
        }

        public List<SelectLabel> GetUserListFiltered(string query)
        {
            var searchString = query.ToLower();
            var selectLabel = new List<SelectLabel>();
            var result = _repository.GetQuery<UserMaster>().Where(a => a.Active == "Y" && (a.FirstName.ToLower().Contains(searchString)
            || a.LastName.ToLower().Contains(searchString)))
            .OrderByDescending(a => a.CreatedDate).ToList();

            foreach (var item in result)
            {
                selectLabel.Add(new SelectLabel
                {
                    name = item.FirstName + " " + item.LastName,
                    code = item.Id.ToString()
                });
            }
            return selectLabel;
        }

        public UserMaster GetUserMasterById(Guid userMasterId)
        {
            UserMaster userMaster = null;
            using (var connection = new SqlConnection(ERPSettings.ConnectionString))
            {
                connection.Open();
                try
                {
                    userMaster = UserMasterData.getUserInfoById(userMasterId.ToString(), connection);
                }
                catch (Exception ex)
                {
                    _logger.Error(String.Format(ERPExceptions.APP_MESSAGES[APPMessageKey.BCUSERMASBYIDERR], ex.StackTrace));
                }
                connection.Close();
            }
            return userMaster;
        }

        public UserMaster GetUserMasterEFById(Guid userMasterId)
        {
            var userMaster = _repository.GetById<UserMaster>(userMasterId);
            var userRole = _repository.GetQuery<UserRoleMapping>().Where(a => a.UserId == userMasterId && a.Active == "Y").ToList();
            var userLedgerAcc = _repository.GetQuery<UserLedgerAccnt>().Where(a => a.UserID == userMasterId && a.Active == "Y").ToList();
            var userOrganization = _repository.GetQuery<UserOrganizationMap>().Where(a => a.UserID == userMasterId && a.Active == "Y").ToList();

            userMaster.UserRoleMap = userRole;
            userMaster.LedgerAccnts = userLedgerAcc;
            userMaster.Organizations = userOrganization;

            return userMaster;
        }

        public AppResponse MarkUserInactive(Guid id)
        {
            AppResponse appResponse = new AppResponse();
            List<String> validationMessages = new List<string>();

            bool validation = true;
            var user = _repository.GetQuery<UserMaster>().Where(a => a.Id == id).FirstOrDefault();
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

                InsertUpdateUser(user);
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
