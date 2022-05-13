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
    public class CashJournalController: ApiController
    {
        private ILogger _logger { get; }
        private IRepository _repository { get; }

        public CashJournalController(ILogger logger, IRepository repository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository;
        }
        [HttpPost]
        [Route("getCashJournals")]
        [Authorize]
        public List<CashJournalSummary> GetCashJournals(CashJournalSearch journalSearch)
        {
            var cashJournalBC = new CashJournalBC(_logger,_repository);
            return cashJournalBC.GetCashJournals(journalSearch);
        }
    }
}