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
namespace ERPService.WebApi.Controllers.Purchase
{
    [Route("api/[controller]")]
    [Authorize]
    public class ProdUnitMasterController : AppApiBaseController
    {

        public ProdUnitMasterController(ILogger logger, IRepository repository, IOwinContext context) : base(logger, repository, context)
        {
        }
        [HttpGet]
        [Route("getallprodUnitMaster")]
        [Authorize]
        public List<ProdUnitMaster> GetAllProdUnitMaster()
        {
            ProdUnitMasterBC ProdUnitBC = new ProdUnitMasterBC(_logger, _repository);
            return ProdUnitBC.GetAllProdUnitMaster();
        }
        [HttpPost]
        [Route("saveProdUnitMaster")]
        [Authorize]
        public AppResponse SaveProdUnitMaster(ProdUnitMaster prodUnit)
        {
            ProdUnitMasterBC ProdUnitBC = new ProdUnitMasterBC(_logger, _repository);
            return ProdUnitBC.SaveProdUnitMaster(prodUnit);
        }

        [HttpGet]
        [Route("getProdUnitMasterById/{prodUnitId}")]
        [Authorize]
        public ProdUnitMaster GetProdUnitMasterById(Guid prodUnitId)
        {
            ProdUnitMasterBC ProdUnitBC = new ProdUnitMasterBC(_logger, _repository);
            return ProdUnitBC.GetProdUnitMasterById(prodUnitId);
        }
        [HttpPost]
        [Route("getSerachFilterProdUnitMaster")]
        [Authorize]
        public List<ProdUnitMaster> GetSerachFilterProdUnitMaster(ProdUnitMaster prodUnit)
        {
            ProdUnitMasterBC ProdUnitBC = new ProdUnitMasterBC(_logger, _repository);
            return ProdUnitBC.GetSerachFilterProdUnitMaster(prodUnit);
        }

        [HttpPost]
        [Route("fetchAllProdUnitMasterLangBased")]
        [Authorize]
        public List<ProdUnitMaster> FetchAllProdUnitMasterLangBased(UserLanguage input)
        {
            var langMasterBC = new LangMasterBC(_logger, _repository);
            return langMasterBC.GetLangBasedDataForProdUnitMaster(input.Language);
        }
    }

}