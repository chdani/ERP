using ERPService.BC;
using ERPService.Common.Interfaces;
using ERPService.Common.Shared;
using ERPService.DataModel.CTO;
using ERPService.DataModel.DTO;
using Serilog;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace ERPService.WebApi
{
    [Route("api/[controller]")]
    [Authorize]
    public class CostCenterController : ApiController
    {
        private ILogger _logger { get; }
        private IRepository _repository { get; }

        public CostCenterController(ILogger logger, IRepository repository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository;
        }
        [HttpGet]
        [Route("getCostCenterById/{id}")]
        [Authorize]
        public CostCenter GetCostCenterById(Guid id)
        {
            var costCenterBC = new CostCenterBC(_logger,_repository);
            return costCenterBC.GetCostCenterById(id);
        }
        [HttpPost]
        [Route("getCostCenters")]
        [Authorize]
        public List<CostCenter> GetCostCenters(CostCenter costCenter)
        {
            var costCenterBC = new CostCenterBC(_logger,_repository);
            return costCenterBC.GetCostCenterList(costCenter);
        }
        [HttpPost]
        [Route("saveCostCenters")]
        [Authorize]
        public AppResponse SaveCostCenters(List<CostCenter> costCenters)
        {
            var costCenterBC = new CostCenterBC(_logger, _repository);
            return costCenterBC.SaveCostCenters(costCenters);
        }

        [HttpPost]
        [Route("fetchAllCostCentersLangBased")]
        [Authorize]
        public List<CostCenter> FetchAllCostCentersLangBased(UserLanguage input)
        {
            var langMasterBC = new LangMasterBC(_logger, _repository);
            return langMasterBC.GetLangBasedDataForCostCenters(input.Language);
        }

    }
}