using ERPService.BC;
using ERPService.Common.Interfaces;
using ERPService.Common.Shared;
using ERPService.DataModel.CTO;
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
    public class UseRoleController : AppApiBaseController
    {
        public UseRoleController(ILogger logger, IRepository repository, IOwinContext context) : base(logger, repository, context)
        {
        }
        [HttpGet]
        [Route("getUserRoleById/{userRoleId}")]
        [Authorize]
        public UserRole GetUserRoleById(Guid userRoleId)
        {
            UserRoleBC userRoleBC = new UserRoleBC(_logger, _repository);
            return userRoleBC.GetUserRoleByIdEf(userRoleId);
        }

        [HttpGet]
        [Route("getAppAccessRoleMapByRoleId/{userRoleId}")]
        [Authorize]
        public List<AppAccessMapping> GetAppAccessRoleMapByRoleId(Guid userRoleId)
        {
            UserRoleBC userRoleBC = new UserRoleBC(_logger, _repository);
            return userRoleBC.GetAppAccessRoleMappingByRoleId(userRoleId);
        }
        [HttpGet]
        [Route("GetUserRoleByUserId/{userMasterId}")]
        [Authorize]
        public List<UserRoleMapping> GetUserRoleByUserId(Guid userMasterId)
        {
            UserRoleBC userRoleBC = new UserRoleBC(_logger, _repository);
            return userRoleBC.GetUserRoleMasterEFById(userMasterId);
        }

        [HttpGet]
        [Route("getUserRoleList")]
        [Authorize]
        public List<UserRole> GetUserRoleList()
        {
            UserRoleBC userRoleBC = new UserRoleBC(_logger, _repository);
            return userRoleBC.GetUserRoleList();
        }


        [HttpPost]
        [Route("saveUserRole")]
        [Authorize]
        public AppResponse SaveUserRole(UserRole userRole)
        {
            UserRoleBC userRoleBC = new UserRoleBC(_logger, _repository);
            return userRoleBC.SaveUserRoleEF(userRole);
        }

        [HttpGet]
        [Route("markRoleInactive/{userRoleId}")]
        [Authorize]
        public AppResponse MarkRoleInactive(Guid userRoleId)
        {
            UserRoleBC userRoleBC = new UserRoleBC(_logger, _repository);
            return userRoleBC.MarkRoleInactive(userRoleId);
        }
    }
}