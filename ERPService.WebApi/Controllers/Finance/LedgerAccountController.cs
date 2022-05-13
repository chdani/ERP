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
    public class LedgerAccountController : ApiController
    {
        private ILogger _logger { get; }
        private IRepository _repository { get; }

        public LedgerAccountController(ILogger logger, IRepository repository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository;
        }
        [HttpGet]
        [Route("getLedgerAccountById/{ledgerAccid}")]
        [Authorize]
        public LedgerAccount GetLedgerAccountById(Guid ledgerAccid)
        {
            LedgerAccountBC ledgerAccBC = new LedgerAccountBC(_logger,_repository);
            return ledgerAccBC.GetLedgerAccountById(ledgerAccid);
        }
        [HttpGet]
        [Route("getLedgerAccountGroupById/{ledgerAccGrpId}")]
        [Authorize]
        public LedgerAccountGrp GetLedgerAccountGroupById(Guid ledgerAccGrpId)
        {
            LedgerAccountBC ledgerAccBC = new LedgerAccountBC(_logger, _repository);
            return ledgerAccBC.GetLedgerAccountGroupById(ledgerAccGrpId);
        }
        [HttpPost]
        [Route("getLedgerAccountList")]
        [Authorize]
        public List<LedgerAccount> GetLedgerAccountList(LedgerAccountSearch ledgerAccount)
        {
            var ledgerAccBC = new LedgerAccountBC(_logger,_repository);
            return ledgerAccBC.GetLedgerAccountList(ledgerAccount);
        }
        [HttpGet]
        [Route("getLedgerAccountGroups")]
        [Authorize]
        public List<LedgerAccountGrp> GetLedgerAccountGroups()
        {
            var ledgerAccBC = new LedgerAccountBC(_logger, _repository);
            return ledgerAccBC.GetLedgerAccountGroups();
        }
        [HttpPost]
        [Route("saveLedgerAccountList")]
        [Authorize]
        public AppResponse SaveLedgerAccountList(List<LedgerAccount> ledgers)
        {
            var ledgerAccBC = new LedgerAccountBC(_logger, _repository);
            return ledgerAccBC.SaveLedgerAccountList(ledgers);
        }

        [HttpPost]
        [Route("saveLedgerAccount")]
        [Authorize]
        public AppResponse SaveLedgerAccount(LedgerAccount ledger)
        {
            var ledgerAccBC = new LedgerAccountBC(_logger, _repository);
            return ledgerAccBC.SaveLedgerAccount(ledger);
        }

        [HttpPost]
        [Route("saveLedgerAccountGroup")]
        [Authorize]
        public AppResponse SaveLedgerAccountGroup(LedgerAccountGrp ledgerGroup)
        {
            var ledgerAccBC = new LedgerAccountBC(_logger, _repository);
            return ledgerAccBC.SaveLedgerAccountGroup(ledgerGroup);
        }

        [HttpPost]
        [Route("fetchAllLedgerAccountsLangBased")]
        [Authorize]
        public List<LedgerAccount> FetchAllLedgerAccountsLangBased(UserLanguage input)
        {
            var langMasterBC = new LangMasterBC(_logger, _repository);
            return langMasterBC.GetLangBasedDataForLedgerAccounts(input.Language);
        }
    }
}