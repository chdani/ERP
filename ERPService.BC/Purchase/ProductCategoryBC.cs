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
    public class ProductCategoryBC
    {
        private ILogger _logger;
        private IRepository _repository;
        public ProductCategoryBC(ILogger logger, IRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public ProductCategory GetProductCategoryById(Guid productCategoryId)
        {
            ProductCategory productCategory = new ProductCategory();
            productCategory = _repository.Get<ProductCategory>(a => a.Id == productCategoryId);
            productCategory.approvalWorkFlow = _repository.GetQuery<ProdCategoryWorkFlow>().Where(a => a.ProdCategoryId == productCategoryId && a.Active == "Y").OrderBy(a => a.ApprovalLevel).ToList();
            productCategory.prodsubCategory = _repository.GetQuery<ProdSubCategory>().Where(a => a.ProdCategoryId == productCategoryId && a.Active == "Y").OrderBy(a => a.Name).ToList();
            return productCategory;
        }

        public List<ProductCategory> GetProductCategoryList()
        {
            List<ProductCategory> productCategory = null;
            productCategory = _repository.GetQuery<ProductCategory>().Where(a => a.Active == "Y").ToList();
            return productCategory;
        }
        public AppResponse SaveProductCategory(ProductCategory productCategory)
        {
            List<String> validationMessages = new List<string>();
            AppResponse appResponse = new AppResponse();
            bool validation = true;
            var Product = _repository.GetQuery<ProductCategory>().FirstOrDefault(a => a.Id != productCategory.Id && (a.Code == productCategory.Code || a.Name == productCategory.Name) && a.Active == "Y");
            if (Product != null)
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.CODEALREADYEXIST]);
                validation = false;
            }
            if (productCategory.Active == "N")
            {
                var productMaster = _repository.GetQuery<ProductMaster>().FirstOrDefault(a => a.ProdCategoryId == productCategory.Id && a.Active == "Y");

                if (productMaster != null)
                {
                    validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.NOTDETASSOCIPRODMAS]);
                    validation = false;
                }
            }
            if (productCategory != null && validation)
            {

                var olddata = _repository.GetQuery<ProdCategoryWorkFlow>().Where(a => a.ProdCategoryId == productCategory.Id).ToList();
                foreach (var item in olddata)
                {
                    _repository.Delete(item);
                }              

                InsertUpdateProductCategory(productCategory, true);
                appResponse.Status = APPMessageKey.DATASAVESUCSS;
                AppGeneralMethods.RemoveCache(ERPCacheKey.PRODUCTCATEGORY, _repository);

            }
            
            else
            {
                appResponse.Status = APPMessageKey.ONEORMOREERR;
                appResponse.Messages = validationMessages;
            }
            return appResponse;
        }

        public AppResponse SaveProductCategoryList(List<ProductCategory> productCategoryList)
        {
            AppResponse appResponse = new AppResponse();
            foreach (var productCategory in productCategoryList)
            {
                InsertUpdateProductCategory(productCategory, false);
                appResponse.Status = APPMessageKey.DATASAVESUCSS;

            }
            if (appResponse.Status == APPMessageKey.DATASAVESUCSS)
            {
                try
                {
                    _repository.SaveChanges();
                    AppGeneralMethods.RemoveCache(ERPCacheKey.PRODUCTCATEGORY, _repository);
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
        private void InsertUpdateProdCategoryWorkFlow(ProdCategoryWorkFlow prodCategoryWork, bool saveChanges)
        {

            if (prodCategoryWork.Id == Guid.Empty)
            {
                prodCategoryWork.Id = Guid.NewGuid();
                _repository.Add(prodCategoryWork);
            }
            else
                _repository.Update(prodCategoryWork);
            if (saveChanges)
                _repository.SaveChanges();

        }
        private void InsertUpdateProdSubCategory(ProdSubCategory subcat, bool saveChanges)
        {

            if (subcat.Id == Guid.Empty)
            {
                subcat.Id = Guid.NewGuid();
                subcat.Active = "Y";
                _repository.Add(subcat);
            }
            else
                _repository.Update(subcat);
            if (saveChanges)
                _repository.SaveChanges();

        }
        private void InsertUpdateProductCategory(ProductCategory productCategory, bool saveChanges)
        {
            if (productCategory.Id == Guid.Empty)
            {
                productCategory.Id = Guid.NewGuid();
                _repository.Add(productCategory, false);
                if (productCategory.approvalWorkFlow != null && productCategory.approvalWorkFlow.Count > 0)
                {
                    foreach (var workFlow in productCategory.approvalWorkFlow)
                    {
                        workFlow.Active = productCategory.Active;
                        workFlow.ProdCategoryId = productCategory.Id;
                        InsertUpdateProdCategoryWorkFlow(workFlow, false);
                    }
                }
                 if (productCategory.prodsubCategory != null && productCategory.prodsubCategory.Count > 0)
                {
                    foreach (var subcat in productCategory.prodsubCategory)
                    {
                        subcat.Active = productCategory.Active;
                        subcat.ProdCategoryId = productCategory.Id;
                        InsertUpdateProdSubCategory(subcat, false);
                    }
                }

            }
            else
            {
                if (productCategory.approvalWorkFlow != null && productCategory.approvalWorkFlow.Count > 0)
                {
                    foreach (var workFlow in productCategory.approvalWorkFlow)
                    {
                        workFlow.Active = productCategory.Active;
                        workFlow.ProdCategoryId = productCategory.Id;
                        InsertUpdateProdCategoryWorkFlow(workFlow, false);
                    }
                }
                if (productCategory.prodsubCategory != null && productCategory.prodsubCategory.Count > 0)
                {
                    foreach (var subcat in productCategory.prodsubCategory)
                    {                      
                        subcat.ProdCategoryId = productCategory.Id;
                        InsertUpdateProdSubCategory(subcat, false);
                    }
                }
                _repository.Update(productCategory, false);
            }
            if (saveChanges)
            {
                _repository.SaveChanges();
            }

        }

        public List<ProductCategory> GetProductCategoryrSerachFilter(ProductCategory productCategory)
        {

            return _repository.GetQuery<ProductCategory>().Where(a =>
               (string.IsNullOrEmpty(productCategory.Name) || a.Name.Contains(productCategory.Name))
              && (string.IsNullOrEmpty(productCategory.Code) || a.Code.Contains(productCategory.Code))
                 && (productCategory.ApprovalLevels == 0 || a.ApprovalLevels == productCategory.ApprovalLevels) &&

               a.Active == "Y").OrderByDescending(a => a.CreatedDate).ToList(); ;
        }
    }
}