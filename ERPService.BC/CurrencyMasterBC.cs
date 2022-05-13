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
    public class CurrencyMasterBC
    {
        private ILogger _logger;
        private IRepository _repository;
        public CurrencyMasterBC(ILogger logger, IRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public CurrencyMaster GetCurrencyMasterById(Guid currencyMasterId)
        {
            return _repository.Get<CurrencyMaster>(a => a.Id == currencyMasterId);
        }

        public List<CurrencyMaster> GetCurrencyMasterList()
        {
            ObjectCache cache = MemoryCache.Default;
            var cacheKey = ERPCacheKey.CURRENCYMASTER;
            List<CurrencyMaster> currencyMasters = null;
            if (cache.Contains(cacheKey))
                currencyMasters = (List<CurrencyMaster>)cache.Get(cacheKey);
            else
            {
                currencyMasters = _repository.GetQuery<CurrencyMaster>().Where(a => a.Active == "Y").ToList();
                cache.Set(cacheKey, currencyMasters, DateTime.Now.AddDays(1));
            }
            return currencyMasters;
        }

        public AppResponse SaveCurrencyMaster(CurrencyMaster currency)
        {
            AppResponse appResponse = new AppResponse();
            List<String> validationMessages = new List<string>();

            bool validation = true;

            if (string.IsNullOrEmpty(currency.Code) || string.IsNullOrEmpty(currency.Name) || string.IsNullOrEmpty(currency.CountryCode))
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.MANDMISSING]);
                validation = false;
            }

            var currencyMaster = _repository.GetQuery<CurrencyMaster>().Where(a => a.Id != currency.Id &&
            (a.Code == currency.Code || a.Name == currency.Name) && a.Active == "Y").FirstOrDefault();

            if (currencyMaster != null)
            {
                validationMessages.Add(String.Format(ERPExceptions.APP_MESSAGES[APPMessageKey.DUPDATACODENAME], currency.Code, currency.Name));
                validation = false;
            }

            if (validation)
            {
                InsertUpdateCurrencyMaster(currency, true);

                appResponse.Status = APPMessageKey.DATASAVESUCSS;
                ObjectCache cache = MemoryCache.Default;
                cache.Remove(ERPCacheKey.CURRENCYMASTER);
            }
            else
            {
                appResponse.Status = APPMessageKey.ONEORMOREERR;
                appResponse.Messages = validationMessages;
            }
            return appResponse;
        }
        public AppResponse SaveCurrencyMasterList(List<CurrencyMaster> currencyMasterList)
        {
            AppResponse appResponse = new AppResponse();
            foreach (var currencyMaster in currencyMasterList)
            {
                InsertUpdateCurrencyMaster(currencyMaster, false);
                appResponse.Status = APPMessageKey.DATASAVESUCSS;

            }
            if (appResponse.Status == APPMessageKey.DATASAVESUCSS)
            {
                try
                {
                    _repository.SaveChanges();
                    AppGeneralMethods.RemoveCache(ERPCacheKey.CURRENCYMASTER, _repository);
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

        private void InsertUpdateCurrencyMaster(CurrencyMaster currencyMaster, bool saveChanges)
        {
            if (currencyMaster.Id == Guid.Empty)
            {
                currencyMaster.Id = Guid.NewGuid();
                _repository.Add(currencyMaster, false);
            }
            else
                _repository.Update(currencyMaster, false);
            if (saveChanges)
            {
                _repository.SaveChanges();
            }

        }
    }

}