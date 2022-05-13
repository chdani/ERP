using ERPService.BC;
using ERPService.Common.Interfaces;
using ERPService.Common.Shared;
using ERPService.DataModel.CTO;
using ERPService.DataModel.DTO;
using Microsoft.Owin;
using Serilog;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace ERPService.WebApi
{
    [Route("api/[controller]")]
    [Authorize]
    public class ProductCategoryController : AppApiBaseController
    {
        public ProductCategoryController(ILogger logger, IRepository repository, IOwinContext context) : base(logger, repository, context)
        {
        }

        [HttpGet]
        [Route("getProductCategoryById/{productCategoryId}")]
        [Authorize]
        public ProductCategory GetProductCategoryById(Guid productCategoryId)
        {
            ProductCategoryBC productBC = new ProductCategoryBC(_logger, _repository);
            return productBC.GetProductCategoryById(productCategoryId);
        }

        [HttpGet]
        [Route("getProductCategoryList")]
        [Authorize]
        public List<ProductCategory> getProductCategoryList()
        {
            ProductCategoryBC productBC = new ProductCategoryBC(_logger, _repository);
            return productBC.GetProductCategoryList();
        }
        [HttpPost]
        [Route("saveProductCategory")]
        [Authorize]
        public AppResponse SaveProductCategory(ProductCategory productCategory)
        {
            ProductCategoryBC productBC = new ProductCategoryBC(_logger, _repository);
            return productBC.SaveProductCategory(productCategory);
        }      
        [HttpPost]
        [Route("getProductCategorySerachFilter")]
        [Authorize]
        public List<ProductCategory> GetProductCategorySerachFilter(ProductCategory productCategory)
        {
            ProductCategoryBC productBC = new ProductCategoryBC(_logger, _repository);
            return productBC.GetProductCategoryrSerachFilter(productCategory);
        }

        [HttpPost]
        [Route("fetchLangBasedDataForProductCategory")]
        [Authorize]
        public List<ProductCategory> FetchLangBasedDataForProductCategory(UserLanguage input)
        {
            var langMasterBC = new LangMasterBC(_logger, _repository);
            return langMasterBC.GetLangBasedDataForProductCategory(input.Language);
        }
        [HttpPost]
        [Route("fetchLangBasedDataForProductSubCategory")]
        [Authorize]
        public List<ProdSubCategory> FetchLangBasedDataForProductSubCategory(UserLanguage input)
        {
            var langMasterBC = new LangMasterBC(_logger, _repository);
            return langMasterBC.FetchLangBasedDataForProductSubCategory(input.Language);
        }
    }
}