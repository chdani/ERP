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
    public class WareHouseController : AppApiBaseController
    {
        public WareHouseController(ILogger logger, IRepository repository, IOwinContext context) : base(logger, repository, context)
        {
        }

        [HttpGet]
        [Route("getWareHouseById/{wareHouseId}")]
        [Authorize]
        public WareHouse GetWareHouseById(Guid wareHouseId)
        {
            WareHouseBC wareHouseBCBC = new WareHouseBC(_logger, _repository);
            return wareHouseBCBC.GetWareHouseById(wareHouseId);
        }

        [HttpGet]
        [Route("getWareHouseList")]
        [Authorize]
        public List<WareHouse> GetWareHouseList()
        {
            WareHouseBC wareHouseBCBC = new WareHouseBC(_logger, _repository);
            return wareHouseBCBC.GetWareHouseList();
        }

        [HttpGet]
        [Route("getWareHouseLocationList")]
        [Authorize]
        public List<WareHouseLocation> GetWareHouseLocationList()
        {
            WareHouseBC wareHouseBCBC = new WareHouseBC(_logger, _repository);
            return wareHouseBCBC.GetWareHouseLocationList();
        }
        [HttpPost]
        [Route("getWareHouseListSearch")]
        [Authorize]
        public List<WareHouse> GetWareHouseListSearch(WareHouse wareHouse)
        {
            WareHouseBC wareHouseBC = new WareHouseBC(_logger, _repository);
            return wareHouseBC.GetWareHouseListSearch(wareHouse);
        }
        [HttpPost]
        [Route("saveWareHouse")]
        [Authorize]
        public AppResponse SaveWareHouse(WareHouse wareHouse)
        {
            WareHouseBC wareHouseBC = new WareHouseBC(_logger, _repository);
            return wareHouseBC.SaveWareHouse(wareHouse);
        }
    }
}