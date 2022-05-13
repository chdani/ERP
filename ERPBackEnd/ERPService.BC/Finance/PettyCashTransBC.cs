using ERPService.Common;
using ERPService.Common.Interfaces;
using ERPService.Common.Shared;
using ERPService.Data;
using ERPService.DataModel.CTO;
using ERPService.DataModel.DTO;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Caching;

namespace ERPService.BC
{
    public class PettyCashTransBC
    {
        private ILogger _logger;
        private IRepository _repository;
        public PettyCashTransBC(ILogger logger, IRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public DataModel.DTO.PettyCashTransfer GetPettyCashTransactionById(Guid transId)
        {
            return _repository.Get<DataModel.DTO.PettyCashTransfer>(a => a.Id == transId);
        }

        public List<DataModel.DTO.PettyCashTransfer> GetPettyCashTransactions(DataModel.DTO.PettyCashTransfer search)
        {

            var orgBC = new OrganizationBC(_logger, _repository);
            var orgList = orgBC.GetOrganizationList();

            var bcPettyAccount = new PettyCashAccoutBC(_logger, _repository);
            var accountList = bcPettyAccount.GetPettyCashAccountList();

            var bcTeller = new PettyCashTellerBC(_logger, _repository);
            var tellerList = bcTeller.GetPettyCashTellerList();
            var pettyTransfer = _repository.GetQuery<PettyCashTransfer>().Where(a => a.FinYear == search.FinYear
            && a.Active == "Y"
            && (search.Id == Guid.Empty || a.Id == search.Id)
            && (search.Amount <= 0 || a.Amount == search.Amount)
            && ((search.FromTransDate <= DateTime.MinValue || search.ToTransDate <= DateTime.MinValue)
                        || (a.TransDate >= search.FromTransDate && a.TransDate <= search.ToTransDate))
            ).ToList();

            if (search.SelectFromAccountId.Count > 0 && search.SelectFromAccountId != null)
            {
                var filteraccount = pettyTransfer.Where(a => search.SelectFromAccountId.Contains(a.FromAccountId)).ToList();
                pettyTransfer = filteraccount;
            }
            if (search.SelectToAccountId.Count > 0 && search.SelectToAccountId != null)
            {
                var filteraccount = pettyTransfer.Where(a => search.SelectToAccountId.Contains(a.ToAccountId)).ToList();
                pettyTransfer = filteraccount;
            }
            if (search.SelectFromOrgId.Count > 0 && search.SelectFromOrgId != null)
            {
                var filterOrg = pettyTransfer.Where(a => search.SelectFromOrgId.Contains(a.FromOrgId)).ToList();
                pettyTransfer = filterOrg;
            }
            if (search.SelectToOrgId.Count > 0 && search.SelectToOrgId != null)
            {
                var filterOrg = pettyTransfer.Where(a => search.SelectToOrgId.Contains(a.ToOrgId)).ToList();
                pettyTransfer = filterOrg;
            }
            if (search.SelectFromTellerId.Count > 0 && search.SelectFromTellerId != null)
            {
                var filterTeller = pettyTransfer.Where(a => search.SelectFromTellerId.Contains(a.FromTellerId)).ToList();
                pettyTransfer = filterTeller;
            }
            if (search.SelectToTellerId.Count > 0 && search.SelectToTellerId != null)
            {
                var filterTeller = pettyTransfer.Where(a => search.SelectToTellerId.Contains(a.ToTellerId)).ToList();
                pettyTransfer = filterTeller;
            }

            var toTellerIds = pettyTransfer.Select(ct => ct.ToTellerId).ToList();
            var toOrgIds = pettyTransfer.Select(ct => ct.ToOrgId).ToList();

            var transDate = DateTime.Now.ToLocalTime();

            var dayClosedRecords = _repository.GetQuery<PettyCashBalance>().Where(a => a.FinYear == search.FinYear
            && a.Active == "Y"
            && toTellerIds.Contains(a.TellerId)
            && toOrgIds.Contains(a.OrgId)
            && a.BalanceDate <= transDate.Date
            && a.DayClosed).ToList();

            var userInfo = _repository.GetQuery<UserMaster>().Where(a => a.Active == "Y").ToList();

            if (pettyTransfer != null)
            {
                foreach (var item in pettyTransfer)
                {
                    var toaccount = accountList.Where(a => a.Id == item.ToAccountId).FirstOrDefault();
                    var fromaccount = accountList.Where(a => a.Id == item.FromAccountId).FirstOrDefault();
                    var toteller = tellerList.Where(a => a.Id == item.ToTellerId).FirstOrDefault();
                    var fromorg = orgList.Where(a => a.Id == item.FromOrgId).FirstOrDefault();
                    var toorg = orgList.Where(a => a.Id == item.ToOrgId).FirstOrDefault();
                    item.FromAccountName = fromaccount?.AccountName;
                    item.ToAccountName = toaccount?.AccountName;
                    var fromtellername = tellerList.Where(a => a.Id == item.FromTellerId).FirstOrDefault();
                    item.FromTellerName = fromtellername?.TellerName;
                    item.ToTellerName = toteller?.TellerName;
                    item.FromOrgName = fromorg?.OrgName;
                    item.ToOrgName = toorg?.OrgName;

                    var dayClosedAccount = dayClosedRecords.FirstOrDefault(rc => rc.AccountId == item.FromAccountId && rc.BalanceDate == item.TransDate);
                    if (dayClosedAccount != null)
                    {
                        item.IsDayClosed = true;
                    }
                }
            }

            return pettyTransfer;

            //var pettyQuery = (from transfer in pettyTransfer
            //                  join acc in accountList on bal.AccountId equals acc.Id
            //                  join teller in tellerList on bal.TellerId equals teller.Id
            //                  join user in userInfo on teller.UserId equals user.Id
            //                  where acc.Active == "Y"
            //                  && teller.Active == "Y"
            //                  && bal.Active == "Y"
            //                  && user.Active == "Y"
            //                  && (search.AccountId == Guid.Empty || search.AccountId == acc.Id)
            //                  && (search.TellerId == Guid.Empty || search.TellerId == teller.Id)
            //                  && (search.UserId == Guid.Empty || search.UserId == user.Id)
            //                  && (search.BalanceDate <= DateTime.MinValue || search.BalanceDate == bal.BalanceDate)
            //                  && (search.FromTransDate <= DateTime.MinValue || search.ToTransDate <= DateTime.MinValue
            //                            || (bal.BalanceDate >= search.FromTransDate && bal.BalanceDate <= search.ToTransDate))
            //                  select new PettyCashBalanceResponse
            //                  {
            //                      AccountId = acc.Id,
            //                      AccountName = acc.AccountName,
            //                      BalanceDate = bal.BalanceDate,
            //                      ClosingBalance = bal.ClosingBalance,
            //                      Credit = bal.Credit,
            //                      Debit = bal.Debit,
            //                      FinYear = bal.FinYear,
            //                      OpeningBalance = bal.OpeningBalance,
            //                      TellerId = teller.Id,
            //                      TellerName = teller.TellerName,
            //                      TellerUserId = teller.UserId,
            //                      UserName = user.UserName,
            //                      IsHeadAccount = acc.IsHeadAccount,
            //                      IsHeadTeller = teller.IsHeadTeller
            //                  }
            //                 );
        }

        public AppResponse SavePettyCashTrans(DataModel.DTO.PettyCashTransfer transaction)
        {
            AppResponse appResponse = new AppResponse();

            if (transaction.FromTellerId == Guid.Empty || transaction.ToTellerId == Guid.Empty
                || transaction.FromAccountId == Guid.Empty || transaction.ToAccountId == Guid.Empty
                || transaction.Amount == 0 || transaction.TransDate <= DateTime.MinValue)
            {
                appResponse.Status = APPMessageKey.MANDMISSING;
                appResponse.Messages = new List<string>();
                appResponse.Messages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.MANDMISSING]);
                return appResponse;
            }

            InsertUpdatePettyCashTeller(transaction);

            appResponse.Status = APPMessageKey.DATASAVESUCSS;
            return appResponse;
        }

        private void InsertUpdatePettyCashTeller(DataModel.DTO.PettyCashTransfer transaction)
        {
            if (transaction.Id == Guid.Empty)
            {
                transaction.Id = Guid.NewGuid();
                _repository.Add(transaction, false);
            }
            else
                _repository.Update(transaction, false);
            _repository.SaveChanges();
        }

    }
}