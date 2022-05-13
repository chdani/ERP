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
    public class CashTransactionBC
    {
        private ILogger _logger;
        private IRepository _repository;
        public CashTransactionBC(ILogger logger, IRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }
        public Organization GetDefaultOrgId()
        {
            return _repository.GetQuery<Organization>().FirstOrDefault(a => a.Active == "Y" && a.DefaultOrg == true);
        }
        public AppResponse SaveCashTransaction(CashTransaction cashTrans)
        {
            AppResponse appResponse = new AppResponse();
            List<String> validationMessages = new List<string>();

            bool validation = true;
            var defaultOrganization = _repository.GetQuery<Organization>().FirstOrDefault(a => a.Active == "Y" && a.DefaultOrg == true);
            cashTrans.OrgId = defaultOrganization.Id;
            cashTrans.TransDate = cashTrans.TransDate.ToLocalTime();

            if (string.IsNullOrEmpty(cashTrans.ProcessType) || string.IsNullOrEmpty(cashTrans.TransType) || (cashTrans.Credit <= 0 && cashTrans.Debit <= 0) ||
                cashTrans.TransDate <= DateTime.MinValue || string.IsNullOrEmpty(cashTrans.FinYear) || cashTrans.LedgerCode <= 0
                || cashTrans.AccountId == Guid.Empty || cashTrans.TellerId == Guid.Empty || cashTrans.TellerUserId == Guid.Empty)
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.MANDMISSING]);
                validation = false;
            }

            var teller = _repository.GetQuery<PettyCashTeller>().FirstOrDefault(a => a.UserId == cashTrans.TellerUserId && a.Id == cashTrans.TellerId && a.Active == "Y");
            if (teller == null)
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.CONTACTHEADTELLER]);
                validation = false;
            }
            if (!teller.IsHeadTeller && cashTrans.TransType == "R")
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.NOTALOWRECTRANS]);
                validation = false;
            }

            var pettyCashBalance = _repository.GetQuery<PettyCashBalance>().FirstOrDefault(a => a.AccountId == cashTrans.AccountId
                && a.TellerId == cashTrans.TellerId
                && a.OrgId == cashTrans.OrgId
                && a.BalanceDate == cashTrans.TransDate && a.Active == "Y");
            if (pettyCashBalance == null && cashTrans.TransType != "R")
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.NOBALPETYCASHACC]);
                validation = false;
            }

            if (cashTrans.TransType == "E" && (pettyCashBalance == null || pettyCashBalance.ClosingBalance < cashTrans.Debit))
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.NOSUFFICIENTACC]);
                validation = false;
            }

            if (pettyCashBalance == null && teller.IsHeadTeller && cashTrans.TransType == "R")
            {
                pettyCashBalance = new PettyCashBalance()
                {
                    OpeningBalance = 0,
                    Credit = cashTrans.Credit,
                    Debit = 0,
                    ClosingBalance = cashTrans.Credit,
                    TellerId = teller.Id,
                    AccountId = cashTrans.AccountId,
                    Active = "Y",
                    BalanceDate = cashTrans.TransDate,
                    FinYear = cashTrans.FinYear,
                    Id = Guid.NewGuid(),
                    OrgId = cashTrans.OrgId,
                    Action = 'N'
                };
            }

            var ledgerAcc = _repository.GetQuery<LedgerAccount>().Where(a => a.LedgerCode == cashTrans.LedgerCode && a.Active == "Y").ToList();
            if (ledgerAcc == null || ledgerAcc.Count() == 0)
            {
                validationMessages.Add(string.Format(ERPExceptions.APP_MESSAGES[APPMessageKey.LEDCODENOTEXIT], cashTrans.LedgerCode));
                validation = false;
            }

            if (cashTrans.Id != Guid.Empty)
            {
                var trans = _repository.GetById<CashTransaction>(cashTrans.Id);
                if (trans == null)
                {
                    validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.RECNOTFOUND]);
                    validation = false;
                }
            }

            if (validation)
            {
                var id = InsertUpdateCashTransaction(cashTrans, pettyCashBalance);

                appResponse.Status = APPMessageKey.DATASAVESUCSS;
                appResponse.ReferenceId = id;
            }
            else
            {
                appResponse.Status = APPMessageKey.ONEORMOREERR;
                appResponse.Messages = validationMessages;
            }
            return appResponse;
        }

        private Guid InsertUpdateCashTransaction(CashTransaction cashTransaction, PettyCashBalance pettyCashBalance)
        {
            List<AppAudit> HdrHistory = new List<AppAudit>();
            CashTransacionHist cashTransacionHist = new CashTransacionHist();

            if (cashTransaction.Id != Guid.Empty)
            {
                var Oldhdr = _repository.GetById<CashTransaction>(cashTransaction.Id);
                HdrHistory = AuditUtility.GetAuditableObject<CashTransaction>(Oldhdr, cashTransaction);

            }
            else
                HdrHistory.Add(new AppAudit() { FieldName = "APP_RECORD_CREATED", NewValue = "", OldValue = "" });
            if (cashTransaction.Id == Guid.Empty)
            {
                cashTransaction.Id = Guid.NewGuid();
                _repository.Add(cashTransaction, false);
            }
            else
            {
                var prevCash = _repository.GetById<CashTransaction>(cashTransaction.Id);
                if (prevCash != null)
                {
                    pettyCashBalance.Credit -= prevCash.Credit;
                    pettyCashBalance.Debit -= prevCash.Debit;
                }
                _repository.Update(cashTransaction, false);

            }

            if (pettyCashBalance.Action == 'N')
                _repository.Add(pettyCashBalance);
            else
            {
                if (cashTransaction.Active == "Y")
                {
                    pettyCashBalance.Credit += cashTransaction.Credit;
                    pettyCashBalance.Debit += cashTransaction.Debit;
                }

                pettyCashBalance.ClosingBalance = pettyCashBalance.OpeningBalance + pettyCashBalance.Credit - pettyCashBalance.Debit;
                _repository.Update(pettyCashBalance);
            }
            var cashTransactionHistoryCommentsBC = new CashTransactionHistoryCommentsBC(_logger, _repository);
            if (HdrHistory != null)
            {
                HdrHistory.ForEach(Hdr =>
                {
                    cashTransacionHist = new CashTransacionHist()
                    {
                        CashTransacionId = cashTransaction.Id,
                        FieldName = Hdr.FieldName,
                        PrevValue = Hdr.OldValue,
                        CurrentValue = Hdr.NewValue,
                        Active = "Y"

                    };
                    cashTransactionHistoryCommentsBC.SaveCashTransactionHist(cashTransacionHist, false);
                });

            }

            _repository.SaveChanges();
            return cashTransaction.Id;
        }
        public CashTransaction GetCashTransactionById(Guid transactoinId)
        {
            return _repository.Get<CashTransaction>(a => a.Id == transactoinId); ;
        }
        public List<CashTransaction> GetCashTransactions(CashTransaction search, bool isExport = false)
        {
            search.FromTransDate = search.FromTransDate.ToLocalTime().Date;
            search.ToTransDate = search.ToTransDate.ToLocalTime().Date;
            var cashTransactions = _repository.GetQuery<CashTransaction>().Where(a =>
              a.FinYear == search.FinYear
              && a.TransType == search.TransType
              && (search.OrgId == Guid.Empty || search.OrgId == a.OrgId)
              && (search.AccountId == Guid.Empty || a.AccountId == search.AccountId)
              && (search.TellerId == Guid.Empty || a.TellerId == search.TellerId)
              && (search.TellerUserId == Guid.Empty || a.TellerUserId == search.TellerUserId)
              && (search.Credit <= 0 || a.Credit == search.Credit)
              && (search.Debit <= 0 || a.Debit == search.Debit)
              && ((search.FromTransDate <= DateTime.MinValue || search.ToTransDate <= DateTime.MinValue)
                        || (a.TransDate >= search.FromTransDate && a.TransDate <= search.ToTransDate))
              && a.Active == "Y").ToList();
            if (search.SelectLedger.Count > 0 && search.SelectLedger != null)
            {
                var filterLedger = cashTransactions.Where(a => search.SelectLedger.Contains(a.LedgerCode)).ToList();
                cashTransactions = filterLedger;
            }
            if (search.SelectProcessType.Count > 0 && search.SelectProcessType != null)
            {
                var filterprocess = cashTransactions.Where(a => search.SelectProcessType.Contains(a.ProcessType)).ToList();
                cashTransactions = filterprocess;
            }
            if (search.SelectCostCenter.Count > 0 && search.SelectCostCenter != null)
            {
                var filterCostCenter = cashTransactions.Where(a => search.SelectCostCenter.Contains(a.CostCenter)).ToList();
                cashTransactions = filterCostCenter;
            }
            if (isExport)
            {
                FillTransactionsDetails(cashTransactions);
            }
            else
            {
                var fromTellerIds = cashTransactions.Select(ct => ct.TellerId).ToList();
                var fromOrgIds = cashTransactions.Select(ct => ct.OrgId).ToList();
                if (fromTellerIds != null && fromTellerIds.Any())
                {
                    var transDate = DateTime.Now.ToLocalTime();

                    var dayClosedRecords = _repository.GetQuery<PettyCashBalance>().Where(a => a.FinYear == search.FinYear
                    && a.Active == "Y"
                    && fromTellerIds.Contains(a.TellerId)
                    && fromOrgIds.Contains(a.OrgId)
                    && a.BalanceDate <= transDate.Date
                    && a.DayClosed).ToList();

                    foreach (var item in cashTransactions)
                    {
                        var dayClosedAccount = dayClosedRecords.FirstOrDefault(rc => rc.AccountId == item.AccountId && rc.BalanceDate == item.TransDate);
                        if (dayClosedAccount != null)
                        {
                            item.IsDayClosed = true;
                        }
                    }
                }
            }

            return cashTransactions;
        }

        public List<CashTransaction> FillTransactionsDetails(List<CashTransaction> cashTransactions)
        {
            var codemaster = _repository.GetQuery<CodesMaster>().Where(a => a.Code == "CASHPROCESSTYPE").FirstOrDefault();

            var codedetails = _repository.GetQuery<CodesDetails>().Where(a => a.CodesMasterId == codemaster.Id).ToList();

            var ledgerCodes = cashTransactions.Select(cd => cd.LedgerCode).ToList();

            var ledger = _repository.GetQuery<LedgerAccount>().Where(la => ledgerCodes.Contains(la.LedgerCode)).ToList();

            var costCenterCodes = cashTransactions.Select(cd => cd.CostCenter).ToList();

            var costCenters = _repository.GetQuery<CostCenter>().Where(la => costCenterCodes.Contains(la.Code)).ToList();

            cashTransactions.ForEach(cd =>
            {
                var ledgerAccount = ledger.FirstOrDefault(la => la.LedgerCode == cd.LedgerCode);
                var costCenter = costCenters.FirstOrDefault(la => la.Code == cd.CostCenter);

                cd.ProcessName = codedetails.FirstOrDefault(det => det.Code == cd.ProcessType)?.Description;
                cd.LedgerCodeWithDesc = string.Format($"{cd.LedgerCode} - {ledgerAccount?.LedgerDesc}");
                cd.CostCenterWithDesc = string.Format($"{cd.CostCenter} - {costCenter?.Description}");
            });

            return cashTransactions;
        }

        public List<PettyCashBalanceResponse> GetPettyCashBalance(PettyCashBalanceSearch search)
        {
            var bcPettyAccount = new PettyCashAccoutBC(_logger, _repository);
            var accountList = bcPettyAccount.GetPettyCashAccountList();

            var bcTeller = new PettyCashTellerBC(_logger, _repository);
            var tellerList = bcTeller.GetPettyCashTellerList();
            var pettyBalance = _repository.GetQuery<PettyCashBalance>().Where(a => a.FinYear == search.FinYear && a.Active == "Y"
                    && (search.OrgId == Guid.Empty || a.OrgId == search.OrgId)).ToList();
            var userInfo = _repository.GetQuery<UserMaster>().Where(a => a.Active == "Y").ToList();

            var pettyQuery = (from bal in pettyBalance
                              join acc in accountList on bal.AccountId equals acc.Id
                              join teller in tellerList on bal.TellerId equals teller.Id
                              join user in userInfo on teller.UserId equals user.Id
                              where acc.Active == "Y"
                              && teller.Active == "Y"
                              && bal.Active == "Y"
                              && user.Active == "Y"
                              && (search.AccountId == Guid.Empty || search.AccountId == acc.Id)
                              && (search.TellerId == Guid.Empty || search.TellerId == teller.Id)
                              && (search.UserId == Guid.Empty || search.UserId == user.Id)
                              && (search.BalanceDate <= DateTime.MinValue || search.BalanceDate == bal.BalanceDate)
                              && (search.FromTransDate <= DateTime.MinValue || search.ToTransDate <= DateTime.MinValue
                                        || (bal.BalanceDate >= search.FromTransDate && bal.BalanceDate <= search.ToTransDate))
                              select new PettyCashBalanceResponse
                              {
                                  AccountId = acc.Id,
                                  AccountName = acc.AccountName,
                                  BalanceDate = bal.BalanceDate,
                                  ClosingBalance = bal.ClosingBalance,
                                  Credit = bal.Credit,
                                  Debit = bal.Debit,
                                  FinYear = bal.FinYear,
                                  OpeningBalance = bal.OpeningBalance,
                                  TellerId = teller.Id,
                                  TellerName = teller.TellerName,
                                  TellerUserId = teller.UserId,
                                  UserName = user.UserName,
                                  // IsHeadAccount = acc.IsHeadAccount,
                                  IsHeadTeller = teller.IsHeadTeller
                              }
                             );


            return pettyQuery.ToList();
        }

        public AppResponse TransferPettyCash(PettyCashTransfer transfer, UserContext context)
        {
            AppResponse appResponse = new AppResponse();
            PettyCashTransferHist pettyCashtransHdrHist = new PettyCashTransferHist();
            List<String> validationMessages = new List<string>();
            bool validation = true;
            string finYear = transfer.FinYear;

            if (transfer.FromAccountId == Guid.Empty || transfer.ToAccountId == Guid.Empty
                || transfer.FromTellerId == Guid.Empty || transfer.ToTellerId == Guid.Empty
                || transfer.Amount == 0 || transfer.Amount < 0 || context.Id == Guid.Empty)
            {
                appResponse.Status = APPMessageKey.MANDMISSING;
                appResponse.Messages = new List<string>();
                appResponse.Messages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.MANDMISSING]);
                return appResponse;
            }

            //Check if LoggedIn person is HeadTeller
            var teller = _repository.GetById<PettyCashTeller>(context.Id);  //<PettyCashTeller>().Id == context.Id;
            if (teller != null && !teller.IsHeadTeller)
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.NOTALOWHEADTELOPE]);
                validation = false;
            }

            transfer.TransDate = transfer.TransDate.ToLocalTime();
            var fromAccount = _repository.GetQuery<PettyCashBalance>().Where(a => a.AccountId == transfer.FromAccountId
            && a.TellerId == transfer.FromTellerId && a.Active == "Y" && a.FinYear == finYear && a.BalanceDate == transfer.TransDate.Date
            && a.OrgId == transfer.FromOrgId).FirstOrDefault();

            var toAccount = _repository.GetQuery<PettyCashBalance>().Where(a => a.AccountId == transfer.ToAccountId
            && a.TellerId == transfer.ToTellerId && a.Active == "Y" && a.FinYear == finYear && a.BalanceDate == transfer.TransDate.Date
            && a.OrgId == transfer.ToOrgId).FirstOrDefault();

            decimal prevAmount = 0;
            PettyCashTransfer prevTransaction = null;
            if (transfer.Id != Guid.Empty)
            {
                prevTransaction = _repository.GetById<PettyCashTransfer>(transfer.Id);
                if (prevTransaction != null)
                    prevAmount = prevTransaction.Amount;
            }
            if (fromAccount == null || (prevAmount + fromAccount.ClosingBalance) == 0)
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.INVOPEACCNOBAL]);
                validation = false;
            }
            //Check if Account has Sufficient balance
            else if (transfer.Amount > (fromAccount.ClosingBalance + prevAmount)) //Complete Transfer Allowed
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.INVOPEACCNOSUFBAL]);
                validation = false;
            }

            if (validation)
            {
                if (prevTransaction != null)
                {
                    var prevFromAccount = _repository.GetQuery<PettyCashBalance>().Where(a => a.AccountId == prevTransaction.FromAccountId
                            && a.TellerId == prevTransaction.FromTellerId && a.Active == "Y" && a.FinYear == finYear && a.BalanceDate == prevTransaction.TransDate
                            && a.OrgId == prevTransaction.FromOrgId).FirstOrDefault();

                    var prevToAccount = _repository.GetQuery<PettyCashBalance>().Where(a => a.AccountId == prevTransaction.ToAccountId
                            && a.TellerId == prevTransaction.ToTellerId && a.Active == "Y" && a.FinYear == finYear && a.BalanceDate == prevTransaction.TransDate
                            && a.OrgId == prevTransaction.ToOrgId).FirstOrDefault();
                    if (fromAccount != null && fromAccount.Id == prevToAccount.Id)
                    {
                        fromAccount.Credit = prevToAccount.Credit - prevTransaction.Amount;
                        fromAccount.ClosingBalance = prevToAccount.ClosingBalance - prevTransaction.Amount;
                    }
                    else if (fromAccount != null && fromAccount.Id == prevFromAccount.Id)
                    {
                        fromAccount.Debit = prevFromAccount.Debit - prevTransaction.Amount;
                        fromAccount.ClosingBalance = prevFromAccount.ClosingBalance + prevTransaction.Amount;
                    }
                    else
                    {
                        prevToAccount.Credit = prevToAccount.Credit - prevTransaction.Amount;
                        prevToAccount.ClosingBalance = prevToAccount.ClosingBalance - prevTransaction.Amount;
                        InsertUpdatePettyCashBalance(prevToAccount, false);
                    }

                    if (toAccount != null && toAccount.Id == prevFromAccount.Id)
                    {
                        toAccount.Debit = prevFromAccount.Debit - prevTransaction.Amount;
                        toAccount.ClosingBalance = prevFromAccount.ClosingBalance + prevTransaction.Amount;
                    }
                    else if (toAccount != null && toAccount.Id == prevToAccount.Id)
                    {
                        toAccount.Credit = prevToAccount.Credit - prevTransaction.Amount;
                        toAccount.ClosingBalance = prevToAccount.ClosingBalance - prevTransaction.Amount;
                    }
                    else
                    {
                        prevFromAccount.Debit = prevFromAccount.Debit - prevTransaction.Amount;
                        prevFromAccount.ClosingBalance = prevFromAccount.ClosingBalance + prevTransaction.Amount;
                        InsertUpdatePettyCashBalance(prevFromAccount, false);
                    }
                }

                //Insert to PettyCashTransfer
                InsertUpdatePettyCashTransfer(transfer);

                if (transfer.Active == "N")
                {
                    InsertUpdatePettyCashBalance(fromAccount, false);
                    InsertUpdatePettyCashBalance(toAccount, false);
                }
                else
                {
                    //Debit from - FromAccount
                    fromAccount.Debit = (fromAccount.Debit + transfer.Amount);
                    fromAccount.ClosingBalance = fromAccount.ClosingBalance - transfer.Amount;
                    InsertUpdatePettyCashBalance(fromAccount, false);

                    //Transfer to Existing Account
                    if (toAccount != null)
                    {
                        toAccount.Credit = (toAccount.Credit + transfer.Amount);
                        toAccount.ClosingBalance = toAccount.ClosingBalance + transfer.Amount;
                        InsertUpdatePettyCashBalance(toAccount, false);
                    }
                    //Transfer to Non Exist new account
                    else
                    {
                        PettyCashBalance bal = new PettyCashBalance();
                        bal.TellerId = transfer.ToTellerId;
                        bal.AccountId = transfer.ToAccountId;
                        bal.BalanceDate = DateTime.Now;
                        bal.OpeningBalance = 0;
                        bal.FinYear = finYear;
                        bal.Credit = transfer.Amount;
                        bal.Debit = 0;
                        bal.ClosingBalance = transfer.Amount;
                        bal.Active = "Y";
                        bal.OrgId = transfer.ToOrgId;
                        InsertUpdatePettyCashBalance(bal, false);
                    }
                }

                _repository.SaveChanges();
            }


            if (validation)
            {
                appResponse.Status = APPMessageKey.DATASAVESUCSS;
            }
            else
            {
                appResponse.Status = APPMessageKey.ONEORMOREERR;
                appResponse.Messages = validationMessages;
            }
            return appResponse;
        }

        public AppResponse ProcessDayClosure(DayClosureRequest request)
        {
            AppResponse appResponse = new AppResponse();
            try
            {
                if (request.FromTellerId == Guid.Empty || request.TransDate == DateTime.MinValue
               || string.IsNullOrEmpty(request.FinYear) || request.FromOrgId == Guid.Empty)
                {
                    appResponse.Status = APPMessageKey.MANDMISSING;
                    appResponse.Messages = new List<string>();
                    appResponse.Messages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.MANDMISSING]);
                    return appResponse;
                }

                // Get all teller details.
                var tellerDetails = _repository.GetQuery<PettyCashTeller>().ToList();

                // Get the request initiated teller detail.
                var tellerDetail = tellerDetails.FirstOrDefault(rq => rq.Id == request.FromTellerId);

                // Get the head teller detail if the request is not initiated by head teller.
                var headTeller = !tellerDetail.IsHeadTeller ? tellerDetails.FirstOrDefault(rq => rq.IsHeadTeller) :
                                                         null;

                request.TransDate = request.TransDate.ToLocalTime();

                // Fetch transaction initiated teller details.
                var reqTellerBalance = _repository.GetQuery<PettyCashBalance>().Where(a => a.TellerId == request.FromTellerId
                                                                                      && a.Active == "Y"
                                                                                      && a.FinYear == request.FinYear
                                                                                      && a.BalanceDate <= request.TransDate.Date
                                                                                      // && !a.DayClosed 
                                                                                      && a.OrgId == request.FromOrgId).ToList();
                // Fetch open balance of current date.
                var reqTellrCurrDateOpenBal = reqTellerBalance.Where(rq => !rq.DayClosed
                                                                    && rq.BalanceDate == request.TransDate.Date).ToList();

                // If teller has no balance for the current date.
                if (!reqTellrCurrDateOpenBal.Any())
                {
                    appResponse.Status = APPMessageKey.NODATAAVAILABLE;
                    appResponse.Messages = new List<string>();
                    appResponse.Messages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.NODATAAVAILABLE]);
                    return appResponse;
                }

                // Normal Teller is closing the day.
                if (!tellerDetail.IsHeadTeller)
                {
                    // Fetching the details of the head teller.
                    var headTellerBalance = _repository.GetQuery<PettyCashBalance>().Where(a => a.TellerId == headTeller.Id
                                                                                    && a.Active == "Y"
                                                                                    && a.FinYear == request.FinYear
                                                                                    && a.BalanceDate == request.TransDate.Date
                                                                                    && a.OrgId == request.FromOrgId).ToList();
                    // Current Date open balance transactions.
                    reqTellrCurrDateOpenBal.ForEach(tellerBal =>
                    {
                        var headTellerBal = headTellerBalance.FirstOrDefault(acc => acc.AccountId == tellerBal.AccountId);

                        if (headTellerBal != null)
                        {
                            headTellerBal.ClosingBalance += tellerBal.ClosingBalance;
                            headTellerBal.Credit += tellerBal.Credit;
                            InsertUpdatePettyCashBalance(headTellerBal, false);
                        }

                        tellerBal.Debit += tellerBal.ClosingBalance;
                        tellerBal.ClosingBalance = 0;
                        tellerBal.DayClosed = true;
                        InsertUpdatePettyCashBalance(tellerBal, false);
                    });

                    // Teller previous date balance which are not day closed.
                    var tellerPreviousDateOpenBalance = reqTellerBalance.Where(rq => !rq.DayClosed
                                                                    && rq.BalanceDate < request.TransDate.Date).ToList();

                    // Teller previous date balance closing.
                    tellerPreviousDateOpenBalance.ForEach(frmAccount =>
                    {
                        frmAccount.DayClosed = true;
                        InsertUpdatePettyCashBalance(frmAccount, false);
                    });
                }
                else
                {
                    // Check tellers(other than Head teller) whose day is not closed.
                    var openTellerBalance = _repository.GetQuery<PettyCashBalance>().Where(a => a.TellerId != request.FromTellerId
                            && a.Active == "Y" && a.FinYear == request.FinYear && a.BalanceDate <= request.TransDate.Date
                            && a.OrgId == request.FromOrgId && !a.DayClosed).ToList();

                    // Check any teller with day not closed.
                    var openTellerBalanceForToday = openTellerBalance.Where(dt => dt.BalanceDate == request.TransDate.Date);

                    // If any teller with day not closed, then do not allow head teller to close the day.
                    if (openTellerBalanceForToday.Any())
                    {
                        appResponse.Status = APPMessageKey.TELLERDAYNOTCLO;
                        appResponse.Messages = new List<string>();
                        appResponse.Messages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.TELLERDAYNOTCLO]);
                        return appResponse;
                    }

                    // Check any tellers(other than Head teller) previous day is not closed.
                    var openTellerBalancePrevDates = openTellerBalance.Where(dt => dt.BalanceDate < request.TransDate.Date).ToList();

                    // Marking their day as closed.
                    openTellerBalancePrevDates.ToList().ForEach(dt =>
                    {
                        dt.DayClosed = true;
                        InsertUpdatePettyCashBalance(dt, false);
                    });

                    // Head teller is closing his day.
                    reqTellrCurrDateOpenBal.ForEach(frmAccount =>
                    {
                        // Head teller remaining balance is carry forwarded to the next day.
                        // Check if head teller has no balance this code is required
                        PettyCashBalance bal = new PettyCashBalance();
                        bal.TellerId = frmAccount.TellerId;
                        bal.AccountId = frmAccount.AccountId;
                        bal.BalanceDate = request.TransDate.AddDays(1);
                        bal.OpeningBalance = frmAccount.ClosingBalance;
                        bal.FinYear = request.FinYear;
                        bal.Credit = 0;
                        bal.Debit = 0;
                        bal.ClosingBalance = frmAccount.ClosingBalance;
                        bal.Active = "Y";
                        bal.OrgId = frmAccount.OrgId;
                        InsertUpdatePettyCashBalance(bal, false);

                        frmAccount.DayClosed = true;
                        InsertUpdatePettyCashBalance(frmAccount, false);
                    });
                }

                _repository.SaveChanges();

                appResponse.Status = APPMessageKey.DATASAVESUCSS;
            }

            catch (Exception ex)
            {
                appResponse.Status = APPMessageKey.ONEORMOREERR;
                appResponse.Messages = new List<string>();
                appResponse.Messages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.UNHDLDEX]);
            }

            return appResponse;
        }

        public bool CheckDayClosureStatus(DayClosureRequest request)
        {
            bool isDayClosed = false;
            try
            {
                var transDate = request.TransDate.ToLocalTime();
                var toAccounts = _repository.GetQuery<PettyCashBalance>().Where(a => a.TellerId == request.FromTellerId
                            && a.Active == "Y" && a.FinYear == request.FinYear && a.BalanceDate == transDate.Date
                            && a.OrgId == request.FromOrgId && a.DayClosed).ToList();
                isDayClosed = toAccounts.Any();
            }

            catch (Exception ex)
            {
                isDayClosed = false;
            }

            return isDayClosed;
        }

        private void InsertUpdatePettyCashBalance(PettyCashBalance transaction, bool isCommit)
        {
            if (transaction.Id == Guid.Empty)
            {
                transaction.Id = Guid.NewGuid();
                _repository.Add(transaction, false);
            }
            else
                _repository.Update(transaction, false);

            if (isCommit)
                _repository.SaveChanges();
        }
        private void InsertUpdatePettyCashTransfer(DataModel.DTO.PettyCashTransfer transaction)
        {
            List<AppAudit> HdrHistory = new List<AppAudit>();
            PettyCashTransferHist pettyCashTransferHist = new PettyCashTransferHist();

            if (transaction.Id != Guid.Empty)
            {
                var Oldhdr = _repository.GetById<PettyCashTransfer>(transaction.Id);
                HdrHistory = AuditUtility.GetAuditableObject<PettyCashTransfer>(Oldhdr, transaction);

            }
            else
                HdrHistory.Add(new AppAudit() { FieldName = "APP_RECORD_CREATED", NewValue = "", OldValue = "" });

            var pettyCashTransferHistCommentsBC = new PettyCashTransferHistCommentsBC(_logger, _repository);
            if (HdrHistory != null)
            {
                HdrHistory.ForEach(Hdr =>
                {
                    pettyCashTransferHist = new PettyCashTransferHist()
                    {
                        PettyCashTransferId = transaction.Id,
                        FieldName = Hdr.FieldName,
                        PrevValue = Hdr.OldValue,
                        CurrentValue = Hdr.NewValue,
                        Active = "Y"

                    };
                    pettyCashTransferHistCommentsBC.SavePettyCashTransferHist(pettyCashTransferHist, false);
                });

            }
            if (transaction.Id == Guid.Empty)
            {
                transaction.Id = Guid.NewGuid();
                _repository.Add(transaction, false);
            }
            else
            {
                _repository.Update(transaction, false);
            }
        }
        public List<PettyCashTeller> tellerNameList()
        {
            var pettycashtellerlist = _repository.GetQuery<PettyCashTeller>().ToList();
            var map = new List<PettyCashTeller>();
            foreach (var tellerlist in pettycashtellerlist)
            {
                map.Add(new PettyCashTeller
                {
                    Id = tellerlist.Id,
                    TellerCode = tellerlist.TellerCode,
                    TellerName = tellerlist.TellerName

                });
            }
            return map;
        }

        public List<PettyCashAccount> accountNameList()
        {
            var pettycashaccountlist = _repository.GetQuery<PettyCashAccount>().ToList();
            var map = new List<PettyCashAccount>();
            foreach (var accountlist in pettycashaccountlist)
            {
                map.Add(new PettyCashAccount
                {
                    Id = accountlist.Id,
                    AccountCode = accountlist.AccountCode,
                    AccountName = accountlist.AccountName

                });
            }
            return map;
        }
    }
}