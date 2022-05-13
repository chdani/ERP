using ERPService.Common;
using ERPService.Common.Interfaces;
using ERPService.Common.Shared;
using ERPService.DataModel.CTO;
using ERPService.DataModel.DTO;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;


namespace ERPService.BC
{
    public class CashJournalBC
    {
        private ILogger _logger;
        private IRepository _repository;
        public CashJournalBC(ILogger logger, IRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public List<CashJournalSummary> GetCashJournals(CashJournalSearch search)
        {
            search.FromDate = search.FromDate.ToLocalTime().Date;
            search.ToDate = search.ToDate.ToLocalTime().Date;

            var journalSummary = new List<CashJournalSummary>();
            var tellerBC = new PettyCashTellerBC(_logger, _repository);
            var tellers = tellerBC.GetPettyCashTellerList();

            var pettyCashAccBC = new PettyCashAccoutBC(_logger, _repository);
            var pettyCashAccs = pettyCashAccBC.GetPettyCashAccountList();

            var orgBC = new OrganizationBC(_logger, _repository);
            var orgs = orgBC.GetOrganizationList();


            var journals = _repository.GetQuery<PettyCashBalance>().Where(a =>
                a.FinYear == search.FinYear
                && a.OrgId == search.OrgId
                && a.Active == "Y"
                && (search.FromDate <= DateTime.MinValue || a.BalanceDate >= search.FromDate)
                && (search.ToDate <= DateTime.MinValue || a.BalanceDate <= search.ToDate)
                ).OrderBy(a => a.BalanceDate).ToList();
            if (search.TellerId != null  &&  search.TellerId.Count > 0 )
            {
                var filterTeller = journals.Where(a => search.TellerId.Contains(a.TellerId)).ToList();
                journals = filterTeller;
            }
            if (search.AccountId != null && search.AccountId.Count > 0  )
            {
                var filterAccount = journals.Where(a => search.AccountId.Contains(a.AccountId)).ToList();
                journals = filterAccount;
            }
            foreach (var journal in journals)
            {
                var org = orgs.FirstOrDefault(a => a.Id == journal.OrgId);
                var teller = tellers.FirstOrDefault(a => a.Id == journal.TellerId);
                var account = pettyCashAccs.FirstOrDefault(a => a.Id == journal.AccountId);
                journalSummary.Add(new CashJournalSummary()
                {
                    AccountName = account?.AccountName,
                    OrganizationName = org?.OrgName,
                    TellerName = teller?.TellerName,
                    Closing = journal.ClosingBalance,
                    Opening = journal.OpeningBalance,
                    Credit = journal.Credit,
                    Debit = journal.Debit,
                    BalanceDate = journal.BalanceDate
                });
            }
            return journalSummary;
        }
    }

}