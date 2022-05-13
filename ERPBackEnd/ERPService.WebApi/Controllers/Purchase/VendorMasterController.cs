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
    public class VendorMasterController : AppApiBaseController
    {
        public VendorMasterController(ILogger logger, IRepository repository, IOwinContext context) : base(logger, repository, context)
        {
        }

        [HttpGet]
        [Route("getVendorMasterById/{vendorid}")]
        [Authorize]
        public VendorMaster GetVendorMasterById(Guid vendorId)
        {
            VendorMasterBC vendorBC = new VendorMasterBC(_logger, _repository, _userContext);
            return vendorBC.GetVendorMasterById(vendorId);
        }

        [HttpGet]
        [Route("getVendorMasterList")]
        [Authorize]
        public List<VendorMaster> getVendorMasterList()
        {
            VendorMasterBC vendorBC = new VendorMasterBC(_logger, _repository, _userContext);
            return vendorBC.GetVendorMasterList();
        }
        [HttpPost]
        [Route("saveVendor")]
        [Authorize]
        public AppResponse SaveVendor(VendorMaster vendor)
        {
            VendorMasterBC vendorBC = new VendorMasterBC(_logger, _repository, _userContext);
            return vendorBC.SaveVendor(vendor);
        }
        [HttpPost]
        [Route("getVendorSerachFilter")]
        [Authorize]
        public List<VendorMaster> GetVendorSerachFilter(VendorMaster vendor)
        {
            VendorMasterBC vendorBC = new VendorMasterBC(_logger, _repository, _userContext);
            return vendorBC.GetVendorSerachFilter(vendor);
        }
        [HttpPost]
        [Route("fetchAllVendorMasterLangBased")]
        [Authorize]
        public List<VendorMaster> FetchAllVendorMasterLangBased(LangMaster input)
        {
            var langMasterBC = new LangMasterBC(_logger, _repository);
            return langMasterBC.GetLangBasedDataForVendorMaster(input.Language);
        }
    }
}