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
    public class PettyCashAccountController : ApiController
    {
        private ILogger _logger { get; }
        private IRepository _repository { get; }

        public PettyCashAccountController(ILogger logger, IRepository repository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository;
        }

        [HttpGet]
        [Route("getPettyCashAccounts")]
        [Authorize]
        public List<PettyCashAccount> GetPettyCashAccounts()
        {
            var pettyCashAccountBC = new PettyCashAccoutBC(_logger, _repository);
            return pettyCashAccountBC.GetPettyCashAccountList();
        }

        [HttpGet]
        [Route("getPettyCashAccountById/{pettyAccountId}")]
        [Authorize]
        public PettyCashAccount GetPettyCashAccountById(Guid pettyAccountId)
        {
            var pettycashAccoutBC = new PettyCashAccoutBC(_logger, _repository);
            return pettycashAccoutBC.GetPettyCashAccountById(pettyAccountId);
        }

        [HttpPost]
        [Route("savePettyCashAccount")]
        [Authorize]
        public AppResponse SavePettyCashAccount(PettyCashAccount pettyCashAccount)
        {
            var pettyCashAccoutBC = new PettyCashAccoutBC(_logger, _repository);
            return pettyCashAccoutBC.SavePettyAccount(pettyCashAccount);
        }

        //[HttpGet]
        //[Route("removeFromHeadAccount/{pettyAccountId}")]
        //[Authorize]
        //public AppResponse removeFromHeadAccount(Guid pettyAccountId)
        //{
        //    var pettyCashAccoutBC = new PettyCashAccoutBC(_logger, _repository);
        //    return pettyCashAccoutBC.removeHeadAccount(pettyAccountId);
        //}

        //[HttpGet]
        //[Route("markAsHeadAccount/{pettyAccountId}")]
        //[Authorize]
        //public AppResponse MarkAsHeadAccount(Guid pettyAccountId)
        //{
        //    var pettyCashAccoutBC = new PettyCashAccoutBC(_logger, _repository);
        //    return pettyCashAccoutBC.markAsHeadAccount(pettyAccountId);
        //}

        [HttpGet]
        [Route("markPettyAccountInactive/{pettyAccountId}")]
        [Authorize]
        public AppResponse MarkPettyAccountInactive(Guid pettyAccountId)
        {
            var pettyCashAccoutBC = new PettyCashAccoutBC(_logger, _repository);
            return pettyCashAccoutBC.markPettyAccountInactive(pettyAccountId);
        }

        [HttpPost]
        [Route("markPettyAccountActive/{pettyAccountId}")]
        [Authorize]
        public AppResponse MarkPettyAccountActive(Guid pettyAccountId)
        {
            var pettyCashAccoutBC = new PettyCashAccoutBC(_logger, _repository);
            return pettyCashAccoutBC.markPettyAccountActive(pettyAccountId);
        }
    }
}