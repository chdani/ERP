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
    [Route("api/currencyMaster")]
    [Authorize]
    public class CurrencyMasterController : ApiController
    {
        private ILogger _logger { get; }
        private IRepository _repository { get; }

        public CurrencyMasterController(ILogger logger, IRepository repository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository;
        }

        [HttpGet]
        [Route("getCurrencyMasters")]
        [Authorize]
        public List<CurrencyMaster> GetCurrencyMasters()
        {
            var currencyMasterBC = new CurrencyMasterBC(_logger, _repository);
            return currencyMasterBC.GetCurrencyMasterList();
        }

        [HttpGet]
        [Route("getCurrencyMasterById/{id}")]
        [Authorize]
        public CurrencyMaster GetCurrencyMasterById(Guid id)
        {
            var currencyMasterBC = new CurrencyMasterBC(_logger, _repository);
            return currencyMasterBC.GetCurrencyMasterById(id);
        }

        [HttpPost]
        [Route("saveCurrencyMaster")]
        [Authorize]
        public AppResponse SaveCurrencyMaster(CurrencyMaster currencyMaster)
        {
            var currencyMasterBC = new CurrencyMasterBC(_logger, _repository);
            return currencyMasterBC.SaveCurrencyMaster(currencyMaster);
        }
    }
}