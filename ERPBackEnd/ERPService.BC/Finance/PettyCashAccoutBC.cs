using ERPService.BC.Utility;
using ERPService.Common;
using ERPService.Common.Interfaces;
using ERPService.Common.Shared;
using ERPService.Data;
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
    public class PettyCashAccoutBC
    {
        private ILogger _logger;
        private IRepository _repository;
        public PettyCashAccoutBC(ILogger logger, IRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public PettyCashAccount GetPettyCashAccountById(Guid pettyCashAccountId)
        {
            return _repository.Get<PettyCashAccount>(a => a.Id == pettyCashAccountId);
        }

        public AppResponse markPettyAccountInactive(Guid pettyCashAccountId)
        {
            AppResponse appResponse = new AppResponse();
            List<String> validationMessages = new List<string>();
            bool validation = true;

            try
            {
                var account = _repository.Get<PettyCashAccount>(a => a.Id == pettyCashAccountId);
                account.Active = "N";
                InsertUpdatePettyAcc(account, true);
            }
            catch (Exception)
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.UNHDLDEX]);
                validation = false;
            }

            if (validation)
            {
                appResponse.Status = APPMessageKey.DATASAVESUCSS;
                ObjectCache cache = MemoryCache.Default;
                cache.Remove(ERPCacheKey.PETTYCASHACCOUNT);
            }
            else
            {
                appResponse.Status = APPMessageKey.ONEORMOREERR;
                appResponse.Messages = validationMessages;
            }
            return appResponse;
        }

        public AppResponse markPettyAccountActive(Guid pettyCashAccountId)
        {
            AppResponse appResponse = new AppResponse();
            List<String> validationMessages = new List<string>();
            bool validation = true;

            try
            {
                var account = _repository.Get<PettyCashAccount>(a => a.Id == pettyCashAccountId);
                account.Active = "Y";
                InsertUpdatePettyAcc(account, true);
            }
            catch (Exception)
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.UNHDLDEX]);
                validation = false;
            }

            if (validation)
                appResponse.Status = APPMessageKey.DATASAVESUCSS;
            else
            {
                appResponse.Status = APPMessageKey.ONEORMOREERR;
                appResponse.Messages = validationMessages;
            }
            return appResponse;
        }

        public List<PettyCashAccount> GetPettyCashAccountList()
        {
            ObjectCache cache = MemoryCache.Default;
            var cacheKey = ERPCacheKey.PETTYCASHACCOUNT;
            List<PettyCashAccount> pettyCashAccounts = null;
            if (cache.Contains(cacheKey))
                pettyCashAccounts = (List<PettyCashAccount>)cache.Get(cacheKey);
            else
            {
                pettyCashAccounts = _repository.GetQuery<PettyCashAccount>().Where(a => a.Active == "Y").ToList();
                cache.Set(cacheKey, pettyCashAccounts, DateTime.Now.AddDays(1));
            }
            return pettyCashAccounts;
        }

        public AppResponse SavePettyAccount(PettyCashAccount accounts)
        {
            AppResponse appResponse = new AppResponse();
            List<String> validationMessages = new List<string>();

            bool validation = true;

            if (string.IsNullOrEmpty(accounts.AccountCode) || string.IsNullOrEmpty(accounts.AccountName))
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.CODEORNAMEMISSING]);
                validation = false;
            }

            var pettyAccount = _repository.GetQuery<PettyCashAccount>().Where(a => a.Id != accounts.Id &&
            (a.AccountCode == accounts.AccountCode || a.AccountName == accounts.AccountName) && a.Active == "Y").FirstOrDefault();

            if (pettyAccount != null)
            {
                validationMessages.Add(String.Format(ERPExceptions.APP_MESSAGES[APPMessageKey.DUPLICODEDES], accounts.AccountCode, accounts.AccountName));
                validation = false;
            }

            //if (accounts.IsHeadAccount)
            //{
            //    var IsHeadAvailable = _repository.GetQuery<PettyCashAccount>().Where(a => a.IsHeadAccount && a.Id !=  accounts.Id && a.Active == "Y").Count() > 0 ? true : false;
            //    if (IsHeadAvailable)
            //    {
            //        validationMessages.Add(ERPExceptions.DB_Dublicate_Entry.Value + " : Head Account already available in the system");
            //        validation = false;
            //    }
            //}

            if (validation)
            {
                var pettyCashAccount = new PettyCashAccount();
                //Setting Value for Update
                if (accounts.Id != null && accounts.Id != Guid.Empty)
                    pettyCashAccount = _repository.GetQuery<PettyCashAccount>().Where(a => a.Id == accounts.Id).FirstOrDefault();

                pettyCashAccount.AccountCode = accounts.AccountCode;
                pettyCashAccount.AccountName = accounts.AccountName;
                // pettyCashAccount.IsHeadAccount = accounts.IsHeadAccount;
                pettyCashAccount.Active = accounts.Active;

                InsertUpdatePettyAcc(pettyCashAccount, true);
                appResponse.Status = APPMessageKey.DATASAVESUCSS;
                ObjectCache cache = MemoryCache.Default;
                cache.Remove(ERPCacheKey.PETTYCASHACCOUNT);
            }
            else
            {
                appResponse.Status = APPMessageKey.ONEORMOREERR;
                appResponse.Messages = validationMessages;
            }
            return appResponse;
        }
        public AppResponse SavePettyCashAccountList(List<PettyCashAccount> pettyCashAccountList)
        {
            AppResponse appResponse = new AppResponse();
            foreach (var pettyCashAccount in pettyCashAccountList)
            {
                InsertUpdatePettyAcc(pettyCashAccount, false);
                appResponse.Status = APPMessageKey.DATASAVESUCSS;

            }
            if (appResponse.Status == APPMessageKey.DATASAVESUCSS)
            {
                try
                {
                    _repository.SaveChanges();
                    AppGeneralMethods.RemoveCache(ERPCacheKey.PETTYCASHACCOUNT, _repository);
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

        private void InsertUpdatePettyAcc(PettyCashAccount pettyAccount, bool saveChanges)
        {
            if (pettyAccount.Id == Guid.Empty)
            {
                pettyAccount.Id = Guid.NewGuid();
                _repository.Add(pettyAccount, false);
            }
            else
                _repository.Update(pettyAccount, false);
            if (saveChanges)
            {
                _repository.SaveChanges();
            }

        }
    }

}