using ERPService.BC;
using ERPService.Common.Interfaces;
using ERPService.Common.Shared;
using ERPService.DataModel.DTO;
using Serilog;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;

namespace ERPService.WebApi
{
    [Route("api/[controller]")]
    [Authorize]
    public class AppAccessRoleMapController : ApiController
    {
        private ILogger _logger { get; }
        private IRepository _repository { get; }

        private UserContext _userContext;

        public AppAccessRoleMapController(ILogger logger, IRepository repository, UserContext userContext)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository;
            _userContext = userContext;
        }
        [HttpGet]
        [Route("getAppAccessRoleMapById/{appAcessRoleMapId}")]
        [Authorize]
        public AppAccessRoleMapping GetAppAccessRoleMapById(Guid appAcessRoleMapId)
        {
            AppAccessRoleMapBC appAccessRoleMapBC = new AppAccessRoleMapBC(_logger,_repository);
            return appAccessRoleMapBC.GetAppAcessRoleMapByIdEf(appAcessRoleMapId);
        }
        [HttpPost]
        [Route("getAppAccessRoleMapByCriteria")]
        [Authorize]
        public List<AppAccessRoleMapping> GetAppAccessRoleMapByCriteria(AppAccessRoleMapping appAccessRoleMap)
        {
            AppAccessRoleMapBC appAccessRoleMapBC = new AppAccessRoleMapBC(_logger);
            return appAccessRoleMapBC.GetAppAccessRoleMapList(appAccessRoleMap);
        }
        [HttpPost]
        [Route("saveAppAccessRoleMap")]
        [Authorize]
        public AppAccessRoleMapping SaveAppAccessRoleMap(AppAccessRoleMapping appAccessRoleMap)
        {
            AppAccessRoleMapBC appAccessRoleMapBC = new AppAccessRoleMapBC(_logger);
            return appAccessRoleMapBC.SaveAppAccessRoleMap(appAccessRoleMap);
        }

        [HttpPost]
        [Authorize]
        [Route("saveUserRoleWithAppAccess")]
        public List<AppAccessRoleMapping> saveUserRoleWithAppAccess(List<AppAccessRoleMapping> appAccessRoleMappings)
        {
            AppAccessRoleMapBC appAccessRoleMapBC = new AppAccessRoleMapBC(_logger, _repository);
            return appAccessRoleMapBC.SaveUserRoleWithAppAccess(appAccessRoleMappings);
        }
    }
}