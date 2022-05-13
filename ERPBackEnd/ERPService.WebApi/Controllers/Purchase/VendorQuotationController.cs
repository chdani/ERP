using ERPService.BC;
using ERPService.BC.Utility;
using ERPService.Common;
using ERPService.Common.Interfaces;
using ERPService.Common.Shared;
using ERPService.DataModel.CTO;
using ERPService.DataModel.DTO;
using Microsoft.Owin;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
namespace ERPService.WebApi.Controllers.Purchase
{
    public class VendorQuotationController : AppApiBaseController
    {
        public VendorQuotationController(ILogger logger, IRepository repository, IOwinContext context) : base(logger, repository, context)
        {
        }


        [HttpGet]
        [Route("getVendorQuotationById/{quotationId}")]
        [Authorize]
        public VendorQuotation GetVendorQuotationById(Guid quotationId)
        {
            VendorQuotationBC quotationBC = new VendorQuotationBC(_logger, _repository,_userContext);
            return quotationBC.GetVendorQuotationById(quotationId);
        }

        [HttpGet]
        [Route("getVendorQuotationList")]
        [Authorize]
        public List<VendorQuotation> getVendorQuotationList()
        {
            VendorQuotationBC quotationBC = new VendorQuotationBC(_logger, _repository,_userContext);
            return quotationBC.GetVendorQuotationList();
        }
        [HttpGet]
        [Route("getVendorQuotationDetList/{quotationId}")]
        [Authorize]
        public List<VendorQuotationDet> getVendorQuotationDetList(Guid quotationId)
        {
            VendorQuotationBC quotationBC = new VendorQuotationBC(_logger, _repository,_userContext);
            return quotationBC.GetVendorQuotationDetList(quotationId);
        }


        





        [HttpPost]
        [Route("saveVendorQuotation")]
        [Authorize]
        public AppResponse SaveVendorQuotation(VendorQuotation vendorQuote)
        {
            VendorQuotationBC vendorQuoteBC = new VendorQuotationBC(_logger, _repository,_userContext);
            return vendorQuoteBC.SaveVendorQuotation(vendorQuote);
        }

        [HttpGet]
        [Route("markVendorQuotationInactive/{vendorQuoteId}")]
        [Authorize]
        public AppResponse MarkVendorQuotationInactive(Guid vendorQuoteId)
        {
            VendorQuotationBC vendorQuoteBC = new VendorQuotationBC(_logger, _repository, _userContext);
            return vendorQuoteBC.MarkVendorQuotationInactive(vendorQuoteId);
        }


        [HttpPost]
        [Route("getVendorQuotationFilter")]
        [Authorize]
        public List<VendorQuotation> GetVendorQuotationSerachFilter(VendorQuotation vendaorData)
        {
            VendorQuotationBC vendorQuoteBC = new VendorQuotationBC(_logger, _repository,_userContext);
            return vendorQuoteBC.GetVendorQuotationSerachFilter(vendaorData);
        }

        [HttpPost]
        [Route("getVendorQuotationListByActive")]
        [Authorize]
        public List<VendorQuotation> GetVendorQuotationListByActive(VendorQuotation vendaorData, bool isExport = false)
        {
            VendorQuotationBC vendorQuoteBC = new VendorQuotationBC(_logger, _repository, _userContext);
            return vendorQuoteBC.GetVendorQuotationListByActive(vendaorData, isExport);
        }

        [HttpGet]
        [Route("getVendorQuotationDetById/{quotationId}")]
        [Authorize]
        public List<VendorQuotationDet> GetVendorQuotationDetById(Guid quotationId)
        {
            VendorQuotationBC quotationBC = new VendorQuotationBC(_logger, _repository, _userContext);
            return quotationBC.GetVendorQuotationDetById(quotationId);
        }

        [HttpPost]
        [Route("saveVendorQuotationComment")]
        [Authorize]
        public AppResponse SaveVendorQuotationComment(VendorQuotationComment vendorQuotationComment)
        {
            VendorQuotationBC quotationBC = new VendorQuotationBC(_logger, _repository, _userContext);
            return quotationBC.SaveVendorQuotationComment(vendorQuotationComment);
        }
        [HttpGet]
        [Route("getVendorQuotationComment/{id}")]
        [Authorize]
        public List<VendorQuotationComment> GetVendorQuotationComment(Guid id)
        {
            VendorQuotationBC quotationBC = new VendorQuotationBC(_logger, _repository, _userContext);
            return quotationBC.GetVendorQuotationComment(id);
        }
        [HttpGet]
        [Route("getVendorQuotationAttachments/{id}")]
        [Authorize]
        public List<AppDocument> GetVendorQuotationAttachments(Guid id)
        {
            VendorQuotationBC quotationBC = new VendorQuotationBC(_logger, _repository, _userContext);
            return quotationBC.GetVendorQuotationAttachments(id);
        }

        [HttpGet]
        [Route("getVendorQuotationStatusHistory/{id}")]
        [Authorize]
        public List<VendorQuotationStatusHist> GetVendorQuotationStatusHistory(Guid id)
        {
            VendorQuotationBC quotationBC = new VendorQuotationBC(_logger, _repository, _userContext);
            return quotationBC.GetVendorQuotationStatusHistory(id);
        }
        [HttpGet]
        [Route("getVendorQuotationHistory/{id}")]
        [Authorize]
        public List<VendorQuotationHist> GetVendorQuotationHistory(Guid id)
        {
            VendorQuotationBC quotationBC = new VendorQuotationBC(_logger, _repository, _userContext);
            return quotationBC.GetVendorQuotationHistory(id);
        }
        [HttpPost]
        [Route("saveVendorQuotationDetComment")]
        [Authorize]
        public AppResponse SaveVendorQuotationDetComment(VendorQuotationDetComment vendorQuotationDetComment)
        {
            VendorQuotationBC quotationBC = new VendorQuotationBC(_logger, _repository, _userContext);
            return quotationBC.SaveVendorQuotationDetComment(vendorQuotationDetComment);
        }
        [HttpGet]
        [Route("getVendorQuoDetComment/{id}")]
        [Authorize]
        public List<VendorQuotationDetComment> GetVendorQuoDetComment(Guid id)
        {
            VendorQuotationBC quotationBC = new VendorQuotationBC(_logger, _repository, _userContext);
            return quotationBC.GetVendorQuoDetComment(id);
        }
        [HttpGet]
        [Route("getVendorQuoDetAttachments/{id}")]
        [Authorize]
        public List<AppDocument> GetVendorQuoDetAttachments(Guid id)
        {
            VendorQuotationBC quotationBC = new VendorQuotationBC(_logger, _repository, _userContext);
            return quotationBC.GetVendorQuoDetAttachments(id);
        }
        [HttpGet]
        [Route("getVendorQuoDetHistory/{id}")]
        [Authorize]
        public List<VendorQuotationDetHist> GetVendorQuoDetHistory(Guid id)
        {
            VendorQuotationBC quotationBC = new VendorQuotationBC(_logger, _repository, _userContext);
            return quotationBC.GetVendorQuoDetHistory(id);
        }
        [HttpPost]
        [Route("approveVendorQuotation")]
        [Authorize]
        public AppResponse ApproveVendorQuotation(VendorQuotation vendorQuotation)
        {
            VendorQuotationBC quotationBC = new VendorQuotationBC(_logger, _repository, _userContext);
            return quotationBC.ApproveVendorQuotation(vendorQuotation);
        }
        [HttpPost]
        [Route("downloadVendorQuotation")]
        [Authorize]
        public HttpResponseMessage DownloadVendorQuotation(VendorQuotation search)
        {
            HttpResponseMessage httpResponse = Request.CreateResponse(System.Net.HttpStatusCode.OK);
            GetFileResponse(search, httpResponse);
            return httpResponse;
        }
        private void GetFileResponse(VendorQuotation search, HttpResponseMessage httpResponse)
        {
            List<VendorQuotation> transfers = GetVendorQuotationListByActive(search, true);
            byte[] buff = null;
            if (transfers.Count > 0)
            {
                if (search.ExportType == ExportType.PDF)
                {
                    var file = new BudgetAllocationPDFGenerator<VendorQuotation>
                    {
                        gridData = transfers,
                        _headerText = search.ExportHeaderText,
                        _repository = _repository,
                        _userContext = _userContext,
                        _logger = _logger,

                    };
                    buff = file.getByte();
                }
                else if (search.ExportType == ExportType.EXCEL)
                {
                    var file = new BudgetAllocationExcelGeneratore<VendorQuotation>
                    {
                        data = transfers,
                        _headerText = search.ExportHeaderText,
                        _repository = _repository,
                        _userContext = _userContext,
                        _logger = _logger,

                    };
                    buff = file.getByte();
                }
            }
            base.CreateFileResponse(httpResponse, buff);
        }
    }
}
