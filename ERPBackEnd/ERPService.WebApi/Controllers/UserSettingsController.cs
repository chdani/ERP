using ERPService.BC;
using ERPService.Common.Interfaces;
using ERPService.Common.Shared;
using ERPService.DataModel.DTO;
using Microsoft.Owin;
using Serilog;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace ERPService.WebApi
{
    [Route("api/[controller]")]
    [Authorize]
    public class UserSettingsController : ApiController
    {
        private ILogger _logger { get; }
        private IRepository _repository { get; }
        private UserContext _userContext;

        public UserSettingsController(ILogger logger, IRepository repository, IOwinContext context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository;
            if (context.Environment.ContainsKey("USRCTX"))
                _userContext = (UserContext)context.Environment["USRCTX"];
            else
                _userContext = new UserContext();
        }
        [HttpGet]
        [Route("getUserSettingsByUserId/{userId}")]
        [Authorize]
        public List<UserSetting> GetUserSettingsByUserId(Guid userId)
        {
            var userSettingsBC = new UserSettingsBC(_logger,_repository);
            return userSettingsBC.GetUserSettingsByUserId(userId);
        }
        [HttpGet]
        [Route("getCurrentUserSettings")]
        [Authorize]
        public List<UserSetting> GetCurrentUserSettings()
        {
            var userSettingsBC = new UserSettingsBC(_logger,_repository);
            return userSettingsBC.GetUserSettingsByUserId(_userContext.Id);
        }
        [HttpPost]
        [Route("getUserSettingsByKey")]
        [Authorize]
        public UserSetting GetUserSettingsByKey(UserSetting userSetting)
        {
            var userSettingsBC = new UserSettingsBC(_logger, _repository);
            return userSettingsBC.GetUserSettingsByKey(userSetting);
        }
        [HttpPost]
        [Route("saveUserSettings")]
        [Authorize]
        public AppResponse SaveUserSettings(List<UserSetting> userSettings)
        {
            var userSettingsBC = new UserSettingsBC(_logger, _repository);
            return userSettingsBC.SaveUserSettings(userSettings, _userContext.Id);
        }
    }
}