using ERPService.BC.Utility;
using ERPService.Common;
using ERPService.Common.Interfaces;
using ERPService.Common.Shared;
using ERPService.Data;
using ERPService.DataModel.DTO;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Caching;

namespace ERPService.BC
{
    public class AppMenuMasterBC
    {
        private ILogger _logger;
        private IRepository _repository;
        public AppMenuMasterBC(ILogger logger, IRepository repository)
        {
            _logger = logger;
            _repository = repository;

        }

        public AppMenuMasterBC(ILogger logger)
        {
            _logger = logger;
        }

        public AppMenuMaster SaveMenuMaster(AppMenuMaster menuMaster)
        {
            AppMenuMaster savedMenu = null;
            if (menuMaster != null)
            {
                if (string.IsNullOrEmpty(menuMaster.MainMenuCode) || string.IsNullOrEmpty(menuMaster.MainMenuName)
                       || string.IsNullOrEmpty(menuMaster.SubMenuName) || string.IsNullOrEmpty(menuMaster.SubMenuCode))
                {
                    var validations = new AppResponse()
                    {
                        Messages = new List<string>(),
                        Status = ERPExceptions.APP_MANDATORY_MISSING.Key
                    };
                    validations.Messages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.MANDMISSING]);
                    savedMenu = new AppMenuMaster()
                    {
                        Validations = validations
                    };
                    return savedMenu;
                }
                using (var connection = new SqlConnection(ERPSettings.ConnectionString))
                {
                    connection.Open();

                    try
                    {
                        using (var transaction = connection.BeginTransaction())
                        {
                            var menuMasterId = "";
                            var status = AppMenuMasterData.InsertUpdateAppMenuMaster(menuMaster, connection, transaction, out menuMasterId);

                            if (status == APPMessageKey.DATASAVESUCSS)
                            {
                                transaction.Commit();

                                savedMenu = AppMenuMasterData.GetAppMenuMasterByid(menuMasterId, connection);
                            }
                            else if (status == APPMessageKey.DUPLICATE)
                            {
                                var validations = new AppResponse()
                                {
                                    Messages = new List<string>(),
                                    Status = APPMessageKey.DUPLICATE
                                };
                                validations.Messages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.DUPLICATE]);
                                savedMenu = new AppMenuMaster()
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
                                savedMenu = new AppMenuMaster()
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
                        savedMenu = new AppMenuMaster()
                        {
                            Validations = validations
                        };
                        _logger.Error(String.Format(ERPExceptions.APP_MESSAGES[APPMessageKey.SAVEMENUMASTERERR], ex.StackTrace));
                    }
                    connection.Close();
                }
            }
            //remove cache values from cache.
            AppGeneralMethods.RemoveCache(ERPCacheKey.MENUS, _repository);
            return savedMenu;
        }
        public AppResponse SaveAppMenuMasterList(List<AppMenuMaster> appMenuMasterList)
        {
            AppResponse appResponse = new AppResponse();
            foreach (var appMenuMaster in appMenuMasterList)
            {
                InsertUpdateAppMenuMaster(appMenuMaster, false);
                appResponse.Status = APPMessageKey.DATASAVESUCSS;

            }
            if (appResponse.Status == APPMessageKey.DATASAVESUCSS)
            {
                try
                {
                    _repository.SaveChanges();
                    AppGeneralMethods.RemoveCache(ERPCacheKey.MENUS, _repository);
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
        private void InsertUpdateAppMenuMaster(AppMenuMaster appMenuMaster, bool saveChanges)
        {

            if (appMenuMaster.Id == Guid.Empty)
            {
                appMenuMaster.Id = Guid.NewGuid();
                _repository.Add(appMenuMaster, true);

            }
            else
            {
                _repository.Update(appMenuMaster, true);
            }
            if (saveChanges)
            {
                _repository.SaveChanges();
            }
        }
        public AppMenuMaster GetMenuMasterById(Guid menuMasterId)
        {
            AppMenuMaster menuMaster = null;
            if (menuMasterId != null)
            {
                using (var connection = new SqlConnection(ERPSettings.ConnectionString))
                {
                    connection.Open();
                    try
                    {
                        menuMaster = AppMenuMasterData.GetAppMenuMasterByid(menuMasterId.ToString(), connection);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(String.Format(ERPExceptions.APP_MESSAGES[APPMessageKey.GETMANUMASTERBYIDERR], ex.StackTrace));
                    }
                    connection.Close();
                }
            }
            return menuMaster;
        }
        public List<AppMenuMaster> GetMenuMasterList(AppMenuMaster menuMaster)
        {
            List<AppMenuMaster> menuMasterList = null;
            if (menuMaster != null)
            {
                using (var connection = new SqlConnection(ERPSettings.ConnectionString))
                {
                    connection.Open();
                    try
                    {
                        menuMasterList = AppMenuMasterData.GetAppMenuMasterList(menuMaster, connection);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(String.Format(ERPExceptions.APP_MESSAGES[APPMessageKey.GETMANUMASTERLISTERR], ex.StackTrace));
                    }
                    connection.Close();
                }
            }
            return menuMasterList;
        }

        public AppMenuMaster GetMenuMasterByIdEf(Guid menuMasterId)
        {
            return _repository.GetById<AppMenuMaster>(menuMasterId);
        }
    }
}
