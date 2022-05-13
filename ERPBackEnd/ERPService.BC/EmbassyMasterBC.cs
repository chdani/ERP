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
    public class EmbassyMasterBC
    {
        private ILogger _logger;
        private IRepository _repository;
        public EmbassyMasterBC(ILogger logger, IRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public EmbassyMaster GetEmbassyMasterById(Guid embassyMasterId)
        {
            return _repository.Get<EmbassyMaster>(a => a.Id == embassyMasterId);
        }

        public List<EmbassyMaster> GetEmbassyMasterList()
        {
            ObjectCache cache = MemoryCache.Default;
            var cacheKey = ERPCacheKey.EMBASSYMASTER;
            List<EmbassyMaster> embassyMasters = null;
            if (cache.Contains(cacheKey))
                embassyMasters = (List<EmbassyMaster>)cache.Get(cacheKey);
            else
            {
                embassyMasters = _repository.GetQuery<EmbassyMaster>().Where(a => a.Active == "Y").ToList();
                cache.Set(cacheKey, embassyMasters, DateTime.Now.AddDays(1));
            }
            return embassyMasters;
        }

        public AppResponse SaveEmbassyMaster(EmbassyMaster embassy)
        {
            AppResponse appResponse = new AppResponse();
            List<String> validationMessages = new List<string>();

            bool validation = true;

            if (string.IsNullOrEmpty(embassy.NameEng) || string.IsNullOrEmpty(embassy.NameArabic) ||
                embassy.Number <= 0 || string.IsNullOrEmpty(embassy.CountryCode))
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.MANDMISSING]);
                validation = false;
            }

            var embassyMaster = _repository.GetQuery<EmbassyMaster>().Where(a => a.Id != embassy.Id &&
            (a.Number == embassy.Number || a.CountryCode == embassy.CountryCode) && a.Active == "Y").FirstOrDefault();

            if (embassyMaster != null)
            {
                validationMessages.Add(String.Format(ERPExceptions.APP_MESSAGES[APPMessageKey.DUPDATACODENAME], embassy.CountryCode, embassy.Number));
                validation = false;
            }

            if (validation)
            {
                InsertUpdateEmbassyMaster(embassy, true);

                appResponse.Status = APPMessageKey.DATASAVESUCSS;
                ObjectCache cache = MemoryCache.Default;
                cache.Remove(ERPCacheKey.EMBASSYMASTER);
            }
            else
            {
                appResponse.Status = APPMessageKey.ONEORMOREERR;
                appResponse.Messages = validationMessages;
            }
            return appResponse;
        }
        public AppResponse SaveEmbassyMasterList(List<EmbassyMaster> embassyMasterList)
        {
            AppResponse appResponse = new AppResponse();
            foreach (var embassyMaster in embassyMasterList)
            {
                InsertUpdateEmbassyMaster(embassyMaster, false);
                appResponse.Status = APPMessageKey.DATASAVESUCSS;

            }
            if (appResponse.Status == APPMessageKey.DATASAVESUCSS)
            {
                try
                {
                    _repository.SaveChanges();
                    AppGeneralMethods.RemoveCache(ERPCacheKey.EMBASSYMASTER, _repository);
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

        private void InsertUpdateEmbassyMaster(EmbassyMaster embassyMaster, bool saveChanges)
        {
            if (embassyMaster.Id == Guid.Empty)
            {
                embassyMaster.Id = Guid.NewGuid();
                _repository.Add(embassyMaster, false);
            }
            else
                _repository.Update(embassyMaster, false);
            if (saveChanges)
            {
                _repository.SaveChanges();
            }

        }
    }

}