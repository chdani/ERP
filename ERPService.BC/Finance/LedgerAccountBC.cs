using ERPService.BC.Utility;
using ERPService.Common;
using ERPService.Common.Interfaces;
using ERPService.Common.Shared;
using ERPService.DataModel.DTO;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

namespace ERPService.BC
{
    public class LedgerAccountBC
    {
        private ILogger _logger;
        private IRepository _repository;
        public LedgerAccountBC(ILogger logger, IRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }


        public AppResponse SaveLedgerAccountGroup(LedgerAccountGrp acctGroup)
        {
            AppResponse appResponse = new AppResponse();
            List<String> validationMessages = new List<string>();

            bool validation = true;
            if (string.IsNullOrEmpty(acctGroup.AccountCode) || string.IsNullOrEmpty(acctGroup.AccountDesc))
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.CODEORDESMISSING]);
                validation = false;
            }

            var ledger = _repository.GetQuery<LedgerAccountGrp>().Where(a => a.Id != acctGroup.Id && (a.AccountCode == acctGroup.AccountCode || a.AccountDesc == acctGroup.AccountDesc));
            if (ledger.Count() > 0)
            {
                validationMessages.Add(String.Format(ERPExceptions.APP_MESSAGES[APPMessageKey.DUPLICODEDES], acctGroup.AccountCode, acctGroup.AccountDesc));
                validation = false;
            }

            ledger = _repository.GetQuery<LedgerAccountGrp>().Where(a => a.Id != acctGroup.Id &&
                ((a.LedgerCodeFrom <= acctGroup.LedgerCodeFrom && a.LedgerCodeTo >= acctGroup.LedgerCodeTo) //start and is inside the range of existing record
                || (a.LedgerCodeFrom >= acctGroup.LedgerCodeFrom && a.LedgerCodeTo <= acctGroup.LedgerCodeTo) //Start from outside the range and end out side the rang of existing record
                || (a.LedgerCodeFrom <= acctGroup.LedgerCodeFrom && a.LedgerCodeTo >= acctGroup.LedgerCodeFrom) //start inside the existing range and finish outside the existing rannge
                || (a.LedgerCodeFrom <= acctGroup.LedgerCodeTo && a.LedgerCodeTo >= acctGroup.LedgerCodeTo) // Start out the existing range and finish inside the existing range 
                ));
            if (ledger.Count() > 0)
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.ACCFROM_TOCODELAP]);
                validation = false;
            }

            if (validation)
            {
                InsertUpdateLedgerAccGrp(acctGroup, true);
                appResponse.Status = APPMessageKey.DATASAVESUCSS;
                ObjectCache cache = MemoryCache.Default;
                cache.Remove(ERPCacheKey.LEDGERACCGRP);
            }
            else
            {
                appResponse.Status = APPMessageKey.ONEORMOREERR;
                appResponse.Messages = validationMessages;
            }
            return appResponse;
        }

        public AppResponse SaveLedgerAccountList(List<LedgerAccount> accounts)
        {
            AppResponse appResponse = new AppResponse();
            List<String> validationMessages = new List<string>();

            bool validation = true;
            foreach (var acc in accounts)
            {
                if (acc.LedgerCode == 0 || string.IsNullOrEmpty(acc.LedgerDesc))
                {
                    validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.CODEORDESMISSING]);
                    validation = false;
                }

                var ledger = _repository.GetQuery<LedgerAccount>().Where(a => a.Id != acc.Id && a.LedgerCode == acc.LedgerCode ).FirstOrDefault();
                if (ledger != null)
                {
                    validationMessages.Add(String.Format(ERPExceptions.APP_MESSAGES[APPMessageKey.DUPLICODEDES], acc.LedgerCode, acc.LedgerDesc));
                    validation = false;
                }

                if (acc.Action != 'N' || acc.Id != Guid.Empty)
                {
                    ledger = _repository.GetById<LedgerAccount>(acc.Id);
                    if (ledger == null)
                    {
                        validationMessages.Add(String.Format(ERPExceptions.APP_MESSAGES[APPMessageKey.RECNOTCODEDES], acc.LedgerCode, acc.LedgerDesc));
                        validation = false;
                    }
                }
            }
            if (validation)
            {
                foreach (var acc in accounts)
                {
                    InsertUpdateLedgerAcc(acc, false);
                    appResponse.Status = APPMessageKey.DATASAVESUCSS;
                }


            }
            if (appResponse.Status == APPMessageKey.DATASAVESUCSS)
            {
                try
                {
                    _repository.SaveChanges();

                    appResponse.Status = APPMessageKey.DATASAVESUCSS;
                    ObjectCache cache = MemoryCache.Default;
                    cache.Remove(ERPCacheKey.LEDGERACC);
                }
                catch
                {
                    appResponse.Status = APPMessageKey.ONEORMOREERR;
                    appResponse.Messages = validationMessages;
                }
            }

            return appResponse;
        }
        public AppResponse SaveLedgerAccountGrpList(List<LedgerAccountGrp> ledgerAccountGrpList)
        {
            AppResponse appResponse = new AppResponse();
            foreach (var ledgerAccountGrp in ledgerAccountGrpList)
            {
                InsertUpdateLedgerAccGrp(ledgerAccountGrp, false);
                appResponse.Status = APPMessageKey.DATASAVESUCSS;

            }
            if (appResponse.Status == APPMessageKey.DATASAVESUCSS)
            {
                try
                {
                    _repository.SaveChanges();
                    AppGeneralMethods.RemoveCache(ERPCacheKey.LEDGERACCGRP, _repository);
                    appResponse.Status = APPMessageKey.DATASAVESUCSS;
                    appResponse.Messages = new List<string>();
                }
                catch (Exception ex)
                {
                    appResponse.Status = APPMessageKey.ONEORMOREERR;
                    appResponse.Messages = new List<string>();
                    appResponse.Messages.Add(ex.Message);
                }
            }
            return appResponse;
        }

        public AppResponse SaveLedgerAccount(LedgerAccount account)
        {
            AppResponse appResponse = new AppResponse();
            List<String> validationMessages = new List<string>();

            bool validation = true;
            if (account.LedgerCode == 0 || string.IsNullOrEmpty(account.LedgerDesc))
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.CODEORDESMISSING]);
                validation = false;
            }

            var ledger = _repository.GetQuery<LedgerAccount>().Where(a => a.Id != account.Id && (a.LedgerCode == account.LedgerCode || a.LedgerDesc == account.LedgerDesc)).FirstOrDefault();
            if (ledger != null)
            {
                validationMessages.Add(String.Format(ERPExceptions.APP_MESSAGES[APPMessageKey.DUPLICODEDES], account.LedgerCode, account.LedgerDesc));
                validation = false;
            }

            if (account.Action != 'N' || account.Id != Guid.Empty)
            {
                ledger = _repository.GetById<LedgerAccount>(account.Id);
                if (ledger == null)
                {
                    validationMessages.Add(String.Format(ERPExceptions.APP_MESSAGES[APPMessageKey.RECNOTCODEDES], account.LedgerCode, account.LedgerDesc));
                    validation = false;
                }
            }

            if (validation)
            {
                InsertUpdateLedgerAcc(account, true);
                appResponse.Status = APPMessageKey.DATASAVESUCSS;
                ObjectCache cache = MemoryCache.Default;
                cache.Remove(ERPCacheKey.LEDGERACC);
            }
            else
            {
                appResponse.Status = APPMessageKey.ONEORMOREERR;
                appResponse.Messages = validationMessages;
            }
            return appResponse;
        }

        private void InsertUpdateLedgerAcc(LedgerAccount acc, bool saveChanges)
        {

            if (acc.Id == Guid.Empty)
            {
                acc.Id = Guid.NewGuid();
                _repository.Add(acc, false);
            }
            else
                _repository.Update(acc, false);
            if (saveChanges)
                _repository.SaveChanges();

        }

        private void InsertUpdateLedgerAccGrp(LedgerAccountGrp accGrp, bool saveChanges)
        {
            if (accGrp.Id == Guid.Empty)
            {
                accGrp.Id = Guid.NewGuid();
                _repository.Add(accGrp, false);
            }
            else
                _repository.Update(accGrp, false);
            if (saveChanges)
            {
                _repository.SaveChanges();
            }


        }

        public LedgerAccount GetLedgerAccountById(Guid ledgerAccountId)
        {
            return _repository.Get<LedgerAccount>(a => a.Id == ledgerAccountId);
        }

        public LedgerAccountGrp GetLedgerAccountGroupById(Guid ledgerAccGroupId)
        {
            return _repository.Get<LedgerAccountGrp>(a => a.Id == ledgerAccGroupId);
        }

        public List<LedgerAccount> GetLedgerAccountList(LedgerAccountSearch search)
        {
            ObjectCache cache = MemoryCache.Default;
            var cacheKey = ERPCacheKey.LEDGERACC;
            List<LedgerAccount> ledgerAccounts = null;
            if (cache.Contains(cacheKey))
                ledgerAccounts = (List<LedgerAccount>)cache.Get(cacheKey);
            else
            {
                ledgerAccounts = _repository.GetQuery<LedgerAccount>().Where(a => a.Active == "Y").ToList();
                cache.Set(cacheKey, ledgerAccounts, DateTime.Now.AddDays(1));
            }

            var accounts = ledgerAccounts.Where(a => (search.LedgerCodeFrom == 0 || a.LedgerCode >= search.LedgerCodeFrom)
                && (search.LedgerCodeTo == 0 || a.LedgerCode <= search.LedgerCodeTo)
                && (string.IsNullOrEmpty(search.LedgerDesc) || a.LedgerDesc.Contains(search.LedgerDesc)));

            return accounts.ToList();
        }
        public List<LedgerAccountGrp> GetLedgerAccountGroups()
        {
            ObjectCache cache = MemoryCache.Default;
            var cacheKey = ERPCacheKey.LEDGERACCGRP;
            List<LedgerAccountGrp> accGroups = null;
            if (cache.Contains(cacheKey))
                accGroups = (List<LedgerAccountGrp>)cache.Get(cacheKey);
            else
            {
                accGroups = _repository.GetQuery<LedgerAccountGrp>().Where(a => a.Active == "Y").ToList();
                cache.Set(cacheKey, accGroups, DateTime.Now.AddDays(1));
            }
            return accGroups;
        }
    }

}