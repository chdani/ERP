using ERPService.BC;
using ERPService.Common.Interfaces;
using ERPService.Common.Shared;
using ERPService.DataModel.DTO;
using Serilog;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace ERPService.WebApi
{
    [Route("api/[controller]")]
    [Authorize]
    public class AppAccessController : ApiController
    {
        private ILogger _logger { get; }
        private IRepository _repository { get; }

        private UserContext _userContext;

        public AppAccessController(ILogger logger, IRepository repository, UserContext userContext)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository;
            _userContext = userContext;
        }

        [HttpGet]
        [Route("getAppAccessById/{appAccessId}")]
        [Authorize]
        public AppAccess GetAppAccessById(Guid appAccessId)
        {
            AppAccessBC appAccessBC = new AppAccessBC(_logger, _repository);
            return appAccessBC.GetAppAccessEFById(appAccessId);
        }

        [HttpGet]
        [Route("getAppAccessList")]
        [Authorize]
        public List<AppAccess> GetAppAccessList()
        {
            AppAccessBC appAccessBC = new AppAccessBC(_logger, _repository);
            return appAccessBC.GetAppAccessList();
        }

        [HttpPost]
        [Route("getAppAccessByCriteria")]
        [Authorize]
        public List<AppAccess> GetAppAccessByCriteria(AppAccess appAccess)
        {
            AppAccessBC appAccessBC = new AppAccessBC(_logger);
            return appAccessBC.GetAppAccessList(appAccess);
        }
        [HttpPost]
        [Route("saveAppAccess")]
        [Authorize]
        public AppAccess SaveAppAccess(AppAccess appAccess)
        {
            AppAccessBC appAccessBC = new AppAccessBC(_logger, _repository);
            return appAccessBC.SaveAppAccess(appAccess);
        }
    }
}