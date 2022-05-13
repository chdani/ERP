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
using System.Linq.Expressions;
using System.Runtime.Caching;

namespace ERPService.BC
{
    public class SystemSettingsBC
    {
        private ILogger _logger;
        private IRepository _repository;
        public SystemSettingsBC(ILogger logger, IRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public AppResponse SaveSystemSettings(List<SystemSetting> SystemSettings)
        {
            AppResponse appResponse = new AppResponse();

            var systemSettDB = _repository.GetQuery<SystemSetting>().ToList();
            var validation = true;

            foreach (var setting in SystemSettings)
            {
                if (systemSettDB.Count(a => a.Id != setting.Id && a.ConfigKey == setting.ConfigKey) > 0)
                {
                    validation = false;
                    appResponse.Status = APPMessageKey.DUPLICATE;
                    appResponse.Messages = new List<string>();
                    appResponse.Messages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.DUPLICATE]);
                }

                if (string.IsNullOrEmpty(setting.ConfigKey) || string.IsNullOrEmpty(setting.ConfigKey))
                {
                    validation = false;
                    appResponse.Status = APPMessageKey.MANDMISSING;
                    appResponse.Messages = new List<string>();
                    appResponse.Messages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.MANDMISSING]);
                }
                if (string.IsNullOrEmpty(setting.Type))
                    setting.Type = "U";
            }

            if (validation)
            {
                foreach (var setting in SystemSettings)
                {
                    if (setting.Id != Guid.Empty)
                        _repository.Update(setting, false);
                    else
                    {
                        setting.Id = Guid.NewGuid();
                        _repository.Add(setting, false);
                    }
                }
                _repository.SaveChanges();
                appResponse.Status = APPMessageKey.DATASAVESUCSS;
                AppGeneralMethods.RemoveCache(ERPCacheKey.SYSTEMSETTING, _repository);
            }
            else
                appResponse.Status = APPMessageKey.ONEORMOREERR;
            return appResponse;
        }

        public List<SystemSetting> GetSystemSettings()
        {
            ObjectCache cache = MemoryCache.Default;
            var cacheKey = ERPCacheKey.SYSTEMSETTING;
            var systemSettings = new List<SystemSetting>();
            if (cache.Contains(cacheKey))
                systemSettings = (List<SystemSetting>)cache.Get(cacheKey);
            else
            {
                systemSettings = _repository.List<SystemSetting>(q => q.Active == "Y");
                cache.Set(cacheKey, systemSettings, DateTime.Now.AddDays(2));
            }
            return systemSettings;
        }

        public SystemSetting GetSystemSettingsByKey(string key)
        {
            return _repository.GetQuery<SystemSetting>().FirstOrDefault(a => a.ConfigKey == key && a.Active == "Y");
        }
    }

}