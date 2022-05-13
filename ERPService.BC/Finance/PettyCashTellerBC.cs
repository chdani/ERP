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
    public class PettyCashTellerBC
    {
        private ILogger _logger;
        private IRepository _repository;
        public PettyCashTellerBC(ILogger logger, IRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public PettyCashTeller GetPettyCashTellerById(Guid pettyCashTellerId)
        {
            return _repository.Get<PettyCashTeller>(a => a.Id == pettyCashTellerId);
        }

        public List<PettyCashTeller> GetPettyCashTellerList()
        {
            ObjectCache cache = MemoryCache.Default;
            var cacheKey = ERPCacheKey.PETTYCASHTELLER;
            List<PettyCashTeller> pettyCashTellers = null;
            if (cache.Contains(cacheKey))
                pettyCashTellers = (List<PettyCashTeller>)cache.Get(cacheKey);
            else
            {
                var tellersQry = _repository.GetQuery<PettyCashTeller>();
                var userQuery = _repository.GetQuery<UserMaster>();

                var users = (from teller in tellersQry
                             join user in userQuery on teller.UserId equals user.Id
                             where teller.Active == "Y" && user.Active == "Y"
                             select new
                             {
                                 Id = teller.Id,
                                 UserId = teller.UserId,
                                 UserName = user.FirstName + " " + user.LastName,
                             }).ToList();
                pettyCashTellers = tellersQry.Where(a => a.Active == "Y").ToList();
                foreach (var teller in pettyCashTellers)
                {
                    var user = users.FirstOrDefault(a => a.Id == teller.Id);
                    if (user != null)
                        teller.UserName = user.UserName;
                }
                cache.Set(cacheKey, pettyCashTellers, DateTime.Now.AddDays(1));
            }
            return pettyCashTellers;
        }

        public AppResponse SavePettyCashTeller(PettyCashTeller tellers)
        {
            AppResponse appResponse = new AppResponse();
            List<String> validationMessages = new List<string>();

            bool validation = true;

            if (string.IsNullOrEmpty(tellers.TellerCode) || string.IsNullOrEmpty(tellers.TellerName) || tellers.UserId == Guid.Empty)
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.MANDMISSING]);
                validation = false;
            }

            var pettyTeller = _repository.GetQuery<PettyCashTeller>().Where(a => a.Id != tellers.Id &&
            (a.TellerCode == tellers.TellerCode || a.TellerName == tellers.TellerName) && a.Active == "Y").FirstOrDefault();

            if (pettyTeller != null)
            {
                validationMessages.Add(String.Format(ERPExceptions.APP_MESSAGES[APPMessageKey.DUPLICODEDES], tellers.TellerCode, tellers.TellerName));
                validation = false;
            }

            if (tellers.IsHeadTeller)
            {
                var IsHeadAvailable = _repository.GetQuery<PettyCashTeller>().Where(a => a.IsHeadTeller && a.Id != tellers.Id && a.Active == "Y").Count() > 0 ? true : false;
                if (IsHeadAvailable)
                {
                    validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.HEADTELLERALREADY]);
                    validation = false;
                }
            }

            pettyTeller = _repository.GetQuery<PettyCashTeller>().Where(a => a.Id != tellers.Id && a.UserId == tellers.UserId && a.Active == "Y").FirstOrDefault();
            if (pettyTeller != null)
            {
                validationMessages.Add(String.Format(ERPExceptions.APP_MESSAGES[APPMessageKey.NAMEALREDYANOTERTELL], pettyTeller.TellerName));
                validation = false;
            }

            if (validation)
            {
                InsertUpdatePettyCashTeller(tellers, true);

                appResponse.Status = APPMessageKey.DATASAVESUCSS;
                ObjectCache cache = MemoryCache.Default;
                cache.Remove(ERPCacheKey.PETTYCASHTELLER);
            }
            else
            {
                appResponse.Status = APPMessageKey.ONEORMOREERR;
                appResponse.Messages = validationMessages;
            }
            return appResponse;
        }
        public AppResponse SavePettyCashTellerList(List<PettyCashTeller> pettyCashTellerList)
        {
            AppResponse appResponse = new AppResponse();
            foreach (var pettyCashTeller in pettyCashTellerList)
            {
                InsertUpdatePettyCashTeller(pettyCashTeller, false);
                appResponse.Status = APPMessageKey.DATASAVESUCSS;

            }
            if (appResponse.Status == APPMessageKey.DATASAVESUCSS)
            {
                try
                {
                    _repository.SaveChanges();
                    AppGeneralMethods.RemoveCache(ERPCacheKey.PETTYCASHTELLER, _repository);
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

        private void InsertUpdatePettyCashTeller(PettyCashTeller pettyTeller, bool saveChanges)
        {
            if (pettyTeller.Id == Guid.Empty)
            {
                pettyTeller.Id = Guid.NewGuid();
                _repository.Add(pettyTeller, false);
            }
            else
                _repository.Update(pettyTeller, false);
            if (saveChanges)
            {
                _repository.SaveChanges();
            }

        }
    }

}