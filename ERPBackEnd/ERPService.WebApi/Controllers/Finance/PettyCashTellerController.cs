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
    [Route("api/pettyCashTeller")]
    [Authorize]
    public class PettyCashTellerController : ApiController
    {
        private ILogger _logger { get; }
        private IRepository _repository { get; }

        public PettyCashTellerController(ILogger logger, IRepository repository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository;
        }

        [HttpGet]
        [Route("getPettyCashTellers")]
        [Authorize]
        public List<PettyCashTeller> GetPettyCashTellers()
        {
            var pettyCashAccountBC = new PettyCashTellerBC(_logger, _repository);
            return pettyCashAccountBC.GetPettyCashTellerList();
        }

        [HttpGet]
        [Route("getPettyCashTellerById/{pettyAccountId}")]
        [Authorize]
        public PettyCashTeller GetPettyCashTellerById(Guid pettyAccountId)
        {
            var pettyCashTellerBC = new PettyCashTellerBC(_logger, _repository);
            return pettyCashTellerBC.GetPettyCashTellerById(pettyAccountId);
        }

        [HttpPost]
        [Route("savePettyCashTeller")]
        [Authorize]
        public AppResponse SavePettyCashTeller(PettyCashTeller pettyCashTeller)
        {
            var pettyCashTellerBC = new PettyCashTellerBC(_logger, _repository);
            return pettyCashTellerBC.SavePettyCashTeller(pettyCashTeller);
        }
    }
}