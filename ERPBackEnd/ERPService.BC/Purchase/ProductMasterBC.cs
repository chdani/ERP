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
    public class ProductMasterBC
    {
        private ILogger _logger;
        private IRepository _repository;
        public ProductMasterBC(ILogger logger, IRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public ProductMaster GetProductMasterById(Guid productMasterId)
        {
            return _repository.GetQuery<ProductMaster>().Where(a => a.Id == productMasterId && a.Active == "Y").FirstOrDefault();
        }
        public AppResponse SaveProductMaster(ProductMaster productMaster)
        {
            List<String> validationMessages = new List<string>();
            AppResponse appResponse = new AppResponse();
            bool validation = true;
            if (productMaster != null)
            {
                var productMasterList = _repository.GetQuery<ProductMaster>().FirstOrDefault(a => a.Id != productMaster.Id && a.ProdCode == productMaster.ProdCode);
                if (productMasterList != null)
                {
                    validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.CODEALREADYEXIST]);
                    validation = false;
                }
            }

            if (productMaster != null && validation)

            {
                InsertUpdateProductMaster(productMaster, true);
                appResponse.Status = APPMessageKey.DATASAVESUCSS;
                AppGeneralMethods.RemoveCache(ERPCacheKey.PRODUCTMASTER, _repository);

            }
            else
            {
                appResponse.Status = APPMessageKey.ONEORMOREERR;
                appResponse.Messages = validationMessages;
            }
            return appResponse;
        }
        public AppResponse SaveProductMasterList(List<ProductMaster> productMasterList)
        {
            AppResponse appResponse = new AppResponse();
            foreach (var productMaster in productMasterList)
            {
                InsertUpdateProductMaster(productMaster, false);
                appResponse.Status = APPMessageKey.DATASAVESUCSS;

            }
            if (appResponse.Status == APPMessageKey.DATASAVESUCSS)
            {
                try
                {
                    _repository.SaveChanges();
                    AppGeneralMethods.RemoveCache(ERPCacheKey.PRODUCTMASTER, _repository);
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

        private void InsertUpdateProductMaster(ProductMaster productMaster, bool savechanges)
        {
            if (productMaster.Id == Guid.Empty)
            {
                productMaster.Id = Guid.NewGuid();
                _repository.Add(productMaster, false);

            }
            else
            {
                _repository.Update(productMaster, false);
            }
            if (savechanges)
            {
                _repository.SaveChanges();
            }

        }

        public List<ProductMaster> GetProductsterSerachFilter(ProductMaster productMaster)
        {

            return _repository.GetQuery<ProductMaster>().Where(a =>
                 (string.IsNullOrEmpty(productMaster.ProdCode) || a.ProdCode.Contains(productMaster.ProdCode))
                && (string.IsNullOrEmpty(productMaster.ProdDescription) || a.ProdDescription.Contains(productMaster.ProdDescription))
               && (string.IsNullOrEmpty(productMaster.Barcode) || a.Barcode.Contains(productMaster.Barcode))
                 && (productMaster.DefaultUnitId == Guid.Empty || productMaster.DefaultUnitId == a.DefaultUnitId)
                  && (productMaster.ProdCategoryId == Guid.Empty || productMaster.ProdCategoryId == a.ProdCategoryId)
                  &&(productMaster.ProdSubCategoryId == Guid.Empty || productMaster.ProdSubCategoryId == a.ProdSubCategoryId)
                && a.Active == "Y").OrderByDescending(a => a.CreatedDate).ToList(); ;
        }


    }
}