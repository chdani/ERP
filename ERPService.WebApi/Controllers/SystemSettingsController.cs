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
    public class SystemSettingsController : AppApiBaseController
    { 
        public SystemSettingsController(ILogger logger, IRepository repository, IOwinContext context) : base(logger, repository, context)
        {
        }

        [HttpGet]
        [Route("getSystemSettings")]
        [Authorize]
        public List<SystemSetting> GetSystemSettings()
        {
            var sysSettingBC = new SystemSettingsBC(_logger,_repository);
            return sysSettingBC.GetSystemSettings();
        }
        [HttpGet]
        [Route("getSystemSettingsByKey/{configKey}")]
        [Authorize]
        public SystemSetting GetSystemSettingsByKey(string configKey)
        {
            var sysSettingBC = new SystemSettingsBC(_logger, _repository);
            return sysSettingBC.GetSystemSettingsByKey(configKey);
        }
        [HttpPost]
        [Route("saveSystemSettings")]
        [Authorize]
        public AppResponse SaveSystemSettings(List<SystemSetting> sysSettings)
        {
            var sysSettingBC = new SystemSettingsBC(_logger, _repository);
            return sysSettingBC.SaveSystemSettings(sysSettings);
        }       
    }
}