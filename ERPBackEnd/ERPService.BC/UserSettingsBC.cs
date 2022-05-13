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

namespace ERPService.BC
{
    public class UserSettingsBC
    {
        private ILogger _logger;
        private IRepository _repository;
        public UserSettingsBC(ILogger logger, IRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public AppResponse SaveUserSettings(List<UserSetting> userSettings, Guid userId)
        {
            AppResponse appResponse = new AppResponse();

            if (userId == Guid.Empty)
            {
                appResponse.Status = "NOUSERINFO";
                appResponse.Messages = new List<string>();
                appResponse.Messages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.PROUSERIDORLOGINSYS]);
                return appResponse;
            }
            else
            {
                var usrInfo = _repository.GetById<UserMaster>(userId);
                if (usrInfo == null)
                {
                    appResponse.Status = "NOUSERINFO";
                    appResponse.Messages = new List<string>();
                    appResponse.Messages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.PROUSERIDORLOGINSYS]);
                    return appResponse;
                }
            }

            var duplicate = userSettings.Select(a => a.ConfigKey).Distinct();
            if (duplicate.Count() != userSettings.Count())
            {
                appResponse.Status = APPMessageKey.DUPLICATE;
                appResponse.Messages = new List<string>();
                appResponse.Messages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.DUPLICATE]);
                return appResponse;
            }

            var userSettDB = _repository.GetQuery<UserSetting>().Where(a => a.UserMasterId == userId && a.ConfigKey != "PASSWORD");
            foreach (var user in userSettDB)
                _repository.Delete<UserSetting>(user, false);
            _repository.SaveChanges();

            foreach (var setting in userSettings)
            {
                setting.Id = Guid.NewGuid();
                setting.UserMasterId = userId;
                _repository.Add(setting, false);
            }
            _repository.SaveChanges();

            appResponse.Status = APPMessageKey.DATASAVESUCSS;
            return appResponse;
        }

        public List<UserSetting> GetUserSettingsByUserId(Guid userId)
        {
            return _repository.GetQuery<UserSetting>().Where(a => a.UserMasterId == userId).ToList();
        }
        public UserSetting GetUserSettingsByKey(UserSetting userSetting)
        {
            return _repository.Get<UserSetting>(a => a.UserMasterId == userSetting.UserMasterId && a.ConfigKey == userSetting.ConfigKey);
        }
    }

}