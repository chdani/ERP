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
    public class UseRoleMapController : ApiController
    {
        private ILogger _logger { get; }
        private IRepository _repository { get; }

        public UseRoleMapController(ILogger logger, IRepository repository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository;
        }
        [HttpGet]
        [Route("getUserRoleMapById/{userRoleMapId}")]
        [Authorize]
        public UserRoleMapping GetUserRoleMapById(Guid userRoleMapId)
        {
            UserRoleMapBC userRoleMapBC = new UserRoleMapBC(_logger,_repository);
            return userRoleMapBC.GetUserRoleMapByIdEf(userRoleMapId);
        }
        [HttpPost]
        [Route("getUserRoleMapByCriteria")]
        [Authorize]
        public List<UserRoleMapping> GetUserRoleMapByCriteria(UserRoleMapping userRoleMap)
        {
            UserRoleMapBC userRoleMapBC = new UserRoleMapBC(_logger);
            return userRoleMapBC.GetUserRoleMapList(userRoleMap);
        }
        [HttpPost]
        [Route("saveUserRoleMap")]
        [Authorize]
        public UserRoleMapping SaveUserRoleMap(UserRoleMapping userRoleMap)
        {
            UserRoleMapBC userRoleMapBC = new UserRoleMapBC(_logger);
            return userRoleMapBC.SaveUserRoleMap(userRoleMap);
        }
    }
}