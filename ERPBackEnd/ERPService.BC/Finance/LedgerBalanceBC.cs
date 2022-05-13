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
    public class LedgerBalanceBC
    {
        private ILogger _logger;
        private IRepository _repository;
        public LedgerBalanceBC(ILogger logger, IRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public AppResponse SaveLedgerBalance(DataModel.DTO.LedgerBalance ledgerBalance)
        {
            AppResponse appResponse = new AppResponse();
            List<String> validationMessages = new List<string>();

            bool validation = true;

            if (ledgerBalance.LedgerCode == 0 || (ledgerBalance.Credit <= 0 && ledgerBalance.Debit <= 0) || ledgerBalance.LedgerDate <= DateTime.MinValue ||
                string.IsNullOrEmpty(ledgerBalance.FinYear))
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.MANDMISSING]);
                validation = false;
            }

            if (validation)
            {
                if (ledgerBalance.Id == Guid.Empty)
                {
                    ledgerBalance.Id = Guid.NewGuid();
                    _repository.Add(ledgerBalance, false);
                }
                else
                    _repository.Update(ledgerBalance, false);
                appResponse.Status = APPMessageKey.DATASAVESUCSS;
            }
            else
            {
                appResponse.Status = APPMessageKey.ONEORMOREERR;
                appResponse.Messages = validationMessages;
            }
            return appResponse;
        }

        public DataModel.DTO.LedgerBalance GetLedgerBalanceById(Guid ledgerId)
        {
            var ledger = _repository.Get<DataModel.DTO.LedgerBalance>(a => a.Id == ledgerId);
            return ledger;
        }
        public List<LedgerBalanceSummary> GetLedgerBalances(LedgerBalanceSearch search, bool isExport = false)
        {
            var ledgerBalance = new List<LedgerBalanceSummary>();
            var ledgerHistroy = GetLedgerHistroy(search, isExport = false);
            var ledgerBalance1 = GetLedgerAccWiseCurrentBalance(search);
            var ledgerHis = ledgerHistroy.Select(a => a.LedgerCode);
            var amount = ledgerBalance1.Where(a => ledgerHis.Contains(a.LedgerCode)).ToList();
            amount.ForEach(a =>
            {
                if (a.Balance > 0)
                    ledgerBalance.Add(a);
            });

            return ledgerBalance;
        }
        public List<LedgerBalanceReport> GetLedgerHistroy(LedgerBalanceSearch search, bool isExport = false)
        {
            List<LedgerBalance> ledgers = new List<LedgerBalance>();
            var userBC = new UserMasterBC(_logger, _repository);
            var LedgerAccounts = userBC.GetUserLedgerAccount(search.UserId);

            search.FromDate = search.FromDate.ToLocalTime().Date;
            search.ToDate = search.ToDate.ToLocalTime().Date;

            var allLedgers = _repository.GetQuery<LedgerBalance>().Where(a =>
                a.FinYear == search.FinYear
                && a.OrgId == search.OrgId
                && a.Active == "Y"
                && (search.Credit <= 0 || a.Credit == search.Credit)
                && (search.Debit <= 0 || a.Debit == search.Debit)
                && a.LedgerDate >= search.FromDate
                && a.LedgerDate <= search.ToDate
                ).OrderBy(a => a.LedgerDate).ToList();
            foreach (var item in allLedgers)
            {
                var ledAcc = LedgerAccounts.Where(a => a.LedgerCode == item.LedgerCode).FirstOrDefault();
                if (ledAcc != null)
                {
                    ledgers.Add(new LedgerBalance
                    {
                        TransactionId = item.TransactionId,
                        OrgId = item.OrgId,
                        TransactionType = item.TransactionType,
                        Credit = item.Credit,
                        Debit = item.Debit,
                        LedgerCode = item.LedgerCode,
                        LedgerDate = item.LedgerDate,
                        FinYear = item.FinYear,
                        Remarks = item.Remarks

                    });
                }

            }

            var ledgerDrafts = _repository.GetQuery<LedgerBalanceDraft>().Where(a =>
                        a.FinYear == search.FinYear
                        && a.OrgId == search.OrgId
                        && a.Amount > 0
                        && a.Active == "Y"
                        && (search.LedgerCode == 0 || a.LedgerCode == search.LedgerCode)
                        && (search.Debit <= 0 || a.Amount == search.Debit)
                        && a.LedgerDate >= search.FromDate
                        && a.LedgerDate <= search.ToDate
                        ).OrderBy(a => a.LedgerDate).ToList();

            foreach (var item in ledgerDrafts)
            {
                ledgers.Add(new LedgerBalance
                {
                    TransactionId = item.TransactionId,
                    OrgId = item.OrgId,
                    TransactionType = item.TransactionType,
                    Credit = 0,
                    Debit = item.Amount,
                    LedgerCode = item.LedgerCode,
                    LedgerDate = item.LedgerDate,
                    FinYear = item.FinYear,
                });
            }

            var openingBalance = _repository.GetQuery<LedgerBalance>().Where(a =>
                a.FinYear == search.FinYear
                && a.OrgId == search.OrgId
                && a.Active == "Y"
                && a.LedgerDate < search.FromDate
                && (search.Credit <= 0 || a.Credit == search.Credit)
                && (search.Debit <= 0 || a.Debit == search.Debit)
                ).ToList();
            if (search.LedgerCodes.Count > 0 && search.LedgerCodes != null)
            {
                var filterLedger = openingBalance.Where(a => search.LedgerCodes.Contains(a.LedgerCode)).ToList();
                openingBalance = filterLedger;
            }
            if (search.TransactionType.Count > 0 && search.TransactionType != null)
            {
                var filterTtansType = openingBalance.Where(a => search.TransactionType.Contains(a.TransactionType)).ToList();
                openingBalance = filterTtansType;
            }

            var totalDebit = openingBalance.Sum(a => a.Debit);
            var totalCredit = openingBalance.Sum(a => a.Credit);
            var openbalance = totalCredit - totalDebit;

            var openingBalanceDraft = _repository.GetQuery<LedgerBalanceDraft>().Where(a =>
                a.FinYear == search.FinYear
                && a.OrgId == search.OrgId
                && a.Active == "Y"
                && a.LedgerDate < search.FromDate
                && (search.Debit <= 0 || a.Amount == search.Debit)
                ).ToList();
            if (search.LedgerCodes.Count > 0 && search.LedgerCodes != null)
            {
                var filterLedger = openingBalanceDraft.Where(a => search.LedgerCodes.Contains(a.LedgerCode)).ToList();
                openingBalanceDraft = filterLedger;
            }
            if (search.TransactionType.Count > 0 && search.TransactionType != null)
            {
                var filterTtansType = openingBalanceDraft.Where(a => search.TransactionType.Contains(a.TransactionType)).ToList();
                openingBalanceDraft = filterTtansType;
            }
            totalDebit = openingBalanceDraft.Sum(a => a.Amount);
            openbalance -= totalDebit;


            var ledgerBal = new List<LedgerBalanceReport>();
            ledgerBal.Add(new LedgerBalanceReport
            {
                TransactionType = ERPTransaction.OPENING_BALANCE,
                Credit = openbalance >= 0 ? openbalance : 0,
                Debit = openbalance < 0 ? (-1 * openbalance) : 0,
            });


            foreach (var tran in ledgers)
            {
                ledgerBal.Add(new LedgerBalanceReport
                {
                    TransactionType = tran.TransactionType,
                    Credit = tran.Credit,
                    Debit = tran.Debit,
                    LedgerCode = tran.LedgerCode,
                    TransDate = tran.LedgerDate,
                    FinYear = tran.FinYear,
                    Remarks = tran.Remarks,
                    TransactionId = tran.TransactionId,
                });

                totalCredit += tran.Credit;
                totalDebit += tran.Debit;
            }

            var closingBalance = totalCredit - totalDebit;
            ledgerBal.Add(new LedgerBalanceReport
            {
                TransactionType = ERPTransaction.CLOSING_BALANCE,
                Credit = closingBalance >= 0 ? closingBalance : 0,
                Debit = closingBalance < 0 ? (-1 * closingBalance) : 0,
            });
            var led = _repository.GetQuery<LedgerAccount>();
            var type = _repository.GetQuery<CodesDetails>();

            if (search.LedgerCodes.Count > 0 && search.LedgerCodes != null)
            {
                var filterLedger = ledgerBal.Where(a => search.LedgerCodes.Contains(a.LedgerCode)).ToList();
                ledgerBal = filterLedger;
            }
            if (search.TransactionType.Count > 0 && search.TransactionType != null)
            {
                var filterTtansType = ledgerBal.Where(a => search.TransactionType.Contains(a.TransactionType)).ToList();
                ledgerBal = filterTtansType;
            }
            if (search.BudgetType.Count > 0 && search.BudgetType != null)
            {
                var budgAllocHdrType = _repository.List<BudgAllocHdr>(a => search.BudgetType.Contains(a.BudgetType) && a.Active == "Y").Select(a => a.Id);
                var budgAllocHdrDetailsList = _repository.List<BudgAllocDet>(a => a.Active == "Y" && budgAllocHdrType.Contains(a.BudgAllocHdrId)).Select(a => a.Id);
                var all = ledgerBal.Where(a => budgAllocHdrDetailsList.Contains(a.TransactionId)).ToList();
                ledgerBal = all;
            }
            if (isExport)
            {
                ledgerBal.ForEach(code =>
                {
                    if (code.LedgerCode > 0)
                    {
                        var LedgerCode = led.Where(a => a.LedgerCode == code.LedgerCode).FirstOrDefault();
                        code.LedgerDesc = LedgerCode.LedgerCode.ToString() + " - " + LedgerCode.LedgerDesc;
                    }
                    {
                        var description = type.Where(a => a.Code == code.TransactionType).FirstOrDefault();
                        if (description != null)
                            code.TransactionType = description.Description;
                    }

                });
            }
            return ledgerBal.OrderByDescending(a => a.TransDate).ToList();
        }

        public List<LedgerBalanceSummary> GetLedgerAccWiseCurrentBalance(LedgerBalanceSearch search)
        {

            var ledgers = _repository.GetQuery<LedgerBalance>().Where(a =>
                a.FinYear == search.FinYear
                && a.OrgId == search.OrgId
                && a.Active == "Y"
                && (search.LedgerCode == 0 || a.LedgerCode == search.LedgerCode))
                .GroupBy(a => a.LedgerCode)
                .Select(a => new LedgerBalanceSummary { LedgerCode = a.Key, Balance = a.Sum(x => x.Credit - x.Debit) });

            return ledgers.ToList();
        }

    }

}