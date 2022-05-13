using ERPService.Common;
using ERPService.DataModel.CTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using ERPService.Data;
using Serilog;
using ERPService.DataModel.DTO;
using ERPService.Common.Shared;
using ERPService.Common.Interfaces;
using System.Net;
using System.Net.Mail;
using System.Runtime.Caching;
using System.IO;
using ERPService.BC.Utility;

namespace ERPService.BC
{
    public class ProdUnitMasterBC
    {
        private ILogger _logger;
        private IRepository _repository;
        private ObjectCache cache = MemoryCache.Default;

        public ProdUnitMasterBC(ILogger logger, IRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }
        public AppResponse SaveProdUnitMaster(ProdUnitMaster prodUnitInfo)
        {
            AppResponse appResponse = new AppResponse();
            List<String> validationMessages = new List<string>();
            bool validation = true;
            var prodUnit = _repository.GetQuery<ProdUnitMaster>().FirstOrDefault(a => a.Id != prodUnitInfo.Id && a.UnitCode == prodUnitInfo.UnitCode && a.Active == "Y");
            if (prodUnit != null)
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.DUPLICATE]);
                validation = false;
            }
            if (prodUnitInfo.Active == "N")
            {
                var productMaster = _repository.GetQuery<ProductMaster>().FirstOrDefault(a => a.DefaultUnitId == prodUnitInfo.Id && a.Active == "Y");
                if (productMaster != null)
                {
                    validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.NOTDETASSOCIPRODMAS]);
                    validation = false;
                }
            }
            if (prodUnitInfo != null && validation)
            {
                InsertUpdateProdUnit(prodUnitInfo, true);
                AppGeneralMethods.RemoveCache(ERPCacheKey.UNITS, _repository);


                appResponse.Status = APPMessageKey.DATASAVESUCSS;
            }
            else
            {
                appResponse.Status = APPMessageKey.ONEORMOREERR;
                appResponse.Messages = validationMessages;
            }
            return appResponse;
        }
        public AppResponse SaveProdUnitMasterList(List<ProdUnitMaster> prodUnitMasterList)
        {
            AppResponse appResponse = new AppResponse();
            foreach (var prodUnitMaster in prodUnitMasterList)
            {
                InsertUpdateProdUnit(prodUnitMaster, false);
                appResponse.Status = APPMessageKey.DATASAVESUCSS;

            }
            if (appResponse.Status == APPMessageKey.DATASAVESUCSS)
            {
                try
                {
                    _repository.SaveChanges();
                    AppGeneralMethods.RemoveCache(ERPCacheKey.UNITS, _repository);
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
        private void InsertUpdateProdUnit(ProdUnitMaster prodUnit, bool saveChanges)
        {

            if (prodUnit.Id == Guid.Empty)
            {
                prodUnit.Id = Guid.NewGuid();
                _repository.Add(prodUnit, true);

            }
            else
            {
                _repository.Update(prodUnit, true);
            }
            if (saveChanges)
            {
                _repository.SaveChanges();
            }
        }

        public List<ProdUnitMaster> GetAllProdUnitMaster()
        {
            var getProdUnit = _repository.GetQuery<ProdUnitMaster>().Where(a => a.Active == "Y").ToList();
            return getProdUnit;
        }
        public ProdUnitMaster GetProdUnitMasterById(Guid prodUnitId)
        {
            var prodUnit = _repository.GetById<ProdUnitMaster>(prodUnitId);
            return prodUnit;
        }

        public List<ProdUnitMaster> GetSerachFilterProdUnitMaster(ProdUnitMaster prodUnit)
        {
            var prodUnitMaster = _repository.GetQuery<ProdUnitMaster>().Where(a =>
              (string.IsNullOrEmpty(prodUnit.UnitCode) || a.UnitCode.Contains(prodUnit.UnitCode))
             && (string.IsNullOrEmpty(prodUnit.UnitName) || a.UnitName.Contains(prodUnit.UnitName)) &&
              (prodUnit.ConversionUnit == 0 || a.ConversionUnit == prodUnit.ConversionUnit) && a.Active == "Y").OrderByDescending(a => a.CreatedDate).ToList();
            return prodUnitMaster;
        }
    }
}