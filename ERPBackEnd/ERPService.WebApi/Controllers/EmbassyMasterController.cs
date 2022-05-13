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
    [Route("api/embassyMaster")]
    [Authorize]
    public class EmbassyMasterController : ApiController
    {
        private ILogger _logger { get; }
        private IRepository _repository { get; }

        public EmbassyMasterController(ILogger logger, IRepository repository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository;
        }

        [HttpGet]
        [Route("getEmbassyMasters")]
        [Authorize]
        public List<EmbassyMaster> GetEmbassyMasters()
        {
            var embassyMasterBC = new EmbassyMasterBC(_logger, _repository);
            return embassyMasterBC.GetEmbassyMasterList();
        }

        [HttpGet]
        [Route("getEmbassyMasterById/{id}")]
        [Authorize]
        public EmbassyMaster GetEmbassyMasterById(Guid id)
        {
            var embassyMasterBC = new EmbassyMasterBC(_logger, _repository);
            return embassyMasterBC.GetEmbassyMasterById(id);
        }

        [HttpPost]
        [Route("saveEmbassyMaster")]
        [Authorize]
        public AppResponse SaveEmbassyMaster(EmbassyMaster embassyMaster)
        {
            var embassyMasterBC = new EmbassyMasterBC(_logger, _repository);
            return embassyMasterBC.SaveEmbassyMaster(embassyMaster);
        }
    }
}