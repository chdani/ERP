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
    public class AppMenuMasterController : ApiController
    {
        private ILogger _logger { get; }
        private IRepository _repository { get; }

        public AppMenuMasterController(ILogger logger, IRepository repository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository;
        }
        [HttpGet]
        [Route("getAppMenuMasterById/{appMenuMasterId}")]
        [Authorize]
        public AppMenuMaster GetAppMenuMasterById(Guid appMenuMasterId)
        {
            AppMenuMasterBC menuMasterBC = new AppMenuMasterBC(_logger,_repository);
            return menuMasterBC.GetMenuMasterByIdEf(appMenuMasterId);
        }
        [HttpPost]
        [Route("getAppMenuMasterByCriteria")]
        [Authorize]
        public List<AppMenuMaster> GetAppMenuMasterByCriteria(AppMenuMaster menuMaster)
        {
            AppMenuMasterBC menuMasterBC = new AppMenuMasterBC(_logger);
            return menuMasterBC.GetMenuMasterList(menuMaster);
        }
        [HttpPost]
        [Route("saveAppMenuMaster")]
        [Authorize]
        public AppMenuMaster SaveAppMenuMaster(AppMenuMaster menuMaster)
        {
            AppMenuMasterBC menuMasterBC = new AppMenuMasterBC(_logger,_repository);
            return menuMasterBC.SaveMenuMaster(menuMaster);
        }
    }
}