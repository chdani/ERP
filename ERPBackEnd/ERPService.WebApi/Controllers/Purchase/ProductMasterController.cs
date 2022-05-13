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

namespace ERPService.WebApi.Controllers
{
    public class ProductMasterController : AppApiBaseController
    {
        public ProductMasterController(ILogger logger, IRepository repository, IOwinContext context) : base(logger, repository, context)
        {
        }

        [HttpGet]
        [Route("getProductMasterById/{productMasterId}")]
        [Authorize]
        public ProductMaster GetProductMasterById(Guid productMasterId)
        {
            ProductMasterBC productBC = new ProductMasterBC(_logger, _repository);
            return productBC.GetProductMasterById(productMasterId);
        }

       
        [HttpPost]
        [Route("saveProductMaster")]
        [Authorize]
        public AppResponse SaveProductMaster(ProductMaster productMaster)
        {
            ProductMasterBC productBC = new ProductMasterBC(_logger, _repository);
            return productBC.SaveProductMaster(productMaster);
        }
       
        [HttpPost]
        [Route("getProductsterSerachFilter")]
        [Authorize]
        public List<ProductMaster> GetProductsterSerachFilter(ProductMaster productMaster)
        {
            ProductMasterBC productBC = new ProductMasterBC(_logger, _repository);
            return productBC.GetProductsterSerachFilter(productMaster);
        }


        [HttpPost]
        [Route("fetchLangBasedDataForProductMaster")]
        [Authorize]
        public List<ProductMaster> FetchLangBasedDataForProductMaster(UserLanguage input)
        {
            var langMasterBC = new LangMasterBC(_logger, _repository);
            return langMasterBC.GetLangBasedDataForProductMaster(input.Language);
        }

      
    }
}