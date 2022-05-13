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
    public class CostCenterBC
    {
        private ILogger _logger;
        private IRepository _repository;
        public CostCenterBC(ILogger logger, IRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public AppResponse SaveCostCenters(List<CostCenter> accounts)
        {
            AppResponse appResponse = new AppResponse();
            List<String> validationMessages = new List<string>();

            bool validation = true;
            foreach (var acc in accounts)
            {
                if (string.IsNullOrEmpty(acc.Code) || string.IsNullOrEmpty(acc.Description))
                {
                    validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.MANDMISSING]);
                    validation = false;
                }

                var cost = _repository.GetQuery<CostCenter>().Where(a => a.Id != acc.Id && (a.Code == acc.Code || a.Description == acc.Description)).FirstOrDefault();
                if (cost != null)
                {
                    validationMessages.Add(string.Format(ERPExceptions.APP_MESSAGES[APPMessageKey.DUPLICODEDES], acc.Code, acc.Description));
                    validation = false;
                }

                if (acc.Action != 'N' || acc.Id != Guid.Empty)
                {
                    cost = _repository.GetById<CostCenter>(acc.Id);
                    if (cost == null)
                    {
                        validationMessages.Add(String.Format(ERPExceptions.APP_MESSAGES[APPMessageKey.DUPLICODEDES], acc.Code, acc.Description));
                        validation = false;
                    }
                }
            }
            if (validation)
            {
                InsertUpdateCostCenter(accounts);
                appResponse.Status = APPMessageKey.DATASAVESUCSS;
                ObjectCache cache = MemoryCache.Default;
                cache.Remove(ERPCacheKey.COSTCENTERS);
            }
            else
            {
                appResponse.Status = APPMessageKey.ONEORMOREERR;
                appResponse.Messages = validationMessages;
            }
            return appResponse;
        }

        private void InsertUpdateCostCenter(List<CostCenter> costCenters)
        {
            foreach (var cc in costCenters)
            {
                if (cc.Id == Guid.Empty)
                {
                    cc.Id = Guid.NewGuid();
                    _repository.Add(cc, false);
                }
                else
                    _repository.Update(cc, false);
            }
            _repository.SaveChanges();
        }
        public CostCenter GetCostCenterById(Guid costCenterId)
        {
            return _repository.Get<CostCenter>(a => a.Id == costCenterId);
        }
        public List<CostCenter> GetCostCenterList(CostCenter search)
        {
            ObjectCache cache = MemoryCache.Default;
            var cacheKey = ERPCacheKey.COSTCENTERS;
            List<CostCenter> costCenters = null;
            List<CostCenter> costCs = null;
            if (cache.Contains(cacheKey))
                costCenters = (List<CostCenter>)cache.Get(cacheKey);
            else
            {
                costCenters = _repository.GetQuery<CostCenter>().Where(a => a.Active == "Y").ToList();
                cache.Set(cacheKey, costCenters, DateTime.Now.AddDays(1));
            }

            if (costCenters != null)
            {
                costCs = costCenters.Where(a =>
                    (string.IsNullOrEmpty(search.Description) || a.Description.Contains(search.Description))
                    && (string.IsNullOrEmpty(search.Code) || a.Code == search.Code)
                    ).ToList();
            }
            return costCs;
        }
    }

}