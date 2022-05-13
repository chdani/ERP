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
namespace ERPService.WebApi.Controllers
{
    public class QuotationRequestController : AppApiBaseController
    {
        public QuotationRequestController(ILogger logger, IRepository repository, IOwinContext context) : base(logger, repository, context)
        {
        }

        [HttpGet]
        [Route("getQuotationRequestById/{quotationRequestId}")]
        [Authorize]
        public QuotationRequest GetQuotationRequestById(Guid quotationRequestId)
        {
            QuotationRequestBC quotationReqBC = new QuotationRequestBC(_logger, _repository, _userContext);
            return quotationReqBC.GetQuotationRequestById(quotationRequestId);
        }

        [HttpPost]
        [Route("saveQuotationRequest")]
        [Authorize]
        public AppResponse SaveQuotationRequest(QuotationRequest serviceRequest)
        {
            QuotationRequestBC quotationReqBC = new QuotationRequestBC(_logger, _repository, _userContext);
            return quotationReqBC.SaveQuotationRequest(serviceRequest);
        }

        [HttpPost]
        [Route("getQuotationRequestList")]
        [Authorize]
        public List<QuotationRequest> GetQuotationRequestList(ServiceRequestSearch input, bool isExport = false)
        {
            QuotationRequestBC quotationReqBC = new QuotationRequestBC(_logger, _repository, _userContext);
            return quotationReqBC.GetQuotationRequestList(input, isExport);
        }
        [HttpGet]
        [Route("getQuotationRequestByTrans/{input}")]
        [Authorize]
        public QuotationRequest GetQuotationRequestByTrans(string input)
        {
            QuotationRequestBC quotationReqBC = new QuotationRequestBC(_logger, _repository, _userContext);
            return quotationReqBC.GetQuotationRequestByTrans(input);
        }

        [HttpGet]
        [Route("getVendorproductList")]
        [Authorize]
        public List<VendorProduct> GetVendorproductList()
        {
            QuotationRequestBC quotationReqBC = new QuotationRequestBC(_logger, _repository, _userContext);
            return quotationReqBC.GetVendorproductList();
        }
        [HttpGet]
        [Route("sendVendorDetailsMail/{id}")]
        public AppResponse SendVendorDetailsMail(Guid id)
        {
            QuotationRequestBC quotationReqBC = new QuotationRequestBC(_logger, _repository, _userContext);
            return quotationReqBC.SendVendorDetailsMail(id);
        }

        [HttpPost]
        [Route("approveQuotationRequest")]
        [Authorize]
        public AppResponse ApproveQuotationRequest(QuotationRequest quotationRequest)
        {
            QuotationRequestBC quotationReqBC = new QuotationRequestBC(_logger, _repository, _userContext);
            return quotationReqBC.ApproveQuotationRequest(quotationRequest);
        }
        [HttpGet]
        [Route("getQuotationReqDetComment/{id}")]
        [Authorize]
        public List<QuotationReqDetComment> GetQuotationReqDetComment(Guid id)
        {
            QuotationRequestBC quotationReqBC = new QuotationRequestBC(_logger, _repository, _userContext);
            return quotationReqBC.GetQuotationReqDetComment(id);
        }
        [HttpGet]
        [Route("getQuotationReqDetHistory/{id}")]
        [Authorize]
        public List<QuotationReqDetHist> GetQuotationReqDetHistory(Guid id)
        {
            QuotationRequestBC quotationReqBC = new QuotationRequestBC(_logger, _repository, _userContext);
            return quotationReqBC.GetQuotationReqDetHistory(id);
        }
        [HttpPost]
        [Route("saveQuotationReqDetComment")]
        [Authorize]
        public AppResponse SaveQuotationReqDetComment(QuotationReqDetComment quotationReqDetComment)
        {
            QuotationRequestBC quotationReqBC = new QuotationRequestBC(_logger, _repository, _userContext);
            return quotationReqBC.SaveQuotationReqDetComment(quotationReqDetComment);
        }
        [HttpPost]
        [Route("saveQuotationRequestComment")]
        [Authorize]
        public AppResponse SaveQuotationRequestComment(QuotationRequestComment purchaseOrd)
        {
            QuotationRequestBC quotationReqBC = new QuotationRequestBC(_logger, _repository, _userContext);
            return quotationReqBC.SaveQuotationRequestComment(purchaseOrd);
        }
        [HttpGet]
        [Route("getQuotationReqComment/{id}")]
        [Authorize]
        public List<QuotationRequestComment> GetQuotationReqComment(Guid id)
        {
            QuotationRequestBC quotationReqBC = new QuotationRequestBC(_logger, _repository, _userContext);
            return quotationReqBC.GetQuotationReqComment(id);
        }
        [HttpGet]
        [Route("getQuotationReqStatusHistory/{id}")]
        [Authorize]
        public List<QuotationReqStatusHist> GetQuotationReqStatusHistory(Guid id)
        {
            QuotationRequestBC quotationReqBC = new QuotationRequestBC(_logger, _repository, _userContext);
            return quotationReqBC.GetQuotationReqStatusHistory(id);
        }
        [HttpGet]
        [Route("getQuotationReqHistory/{id}")]
        [Authorize]
        public List<QuotationRequestHist> GetQuotationReqHistory(Guid id)
        {
            QuotationRequestBC quotationReqBC = new QuotationRequestBC(_logger, _repository, _userContext);
            return quotationReqBC.GetQuotationReqHistory(id);
        }
        [HttpGet]
        [Route("getQuotationReqAttachments/{id}")]
        [Authorize]
        public List<AppDocument> GetQuotationReqAttachments(Guid id)
        {
            QuotationRequestBC quotationReqBC = new QuotationRequestBC(_logger, _repository, _userContext);
            return quotationReqBC.GetQuotationReqAttachments(id);
        }
        [HttpGet]
        [Route("getQuotationReqDetAttachments/{id}")]
        [Authorize]
        public List<AppDocument> GetQuotationReqDetAttachments(Guid id)
        {
            QuotationRequestBC quotationReqBC = new QuotationRequestBC(_logger, _repository, _userContext);
            return quotationReqBC.GetQuotationReqDetAttachments(id);
        }
        [HttpPost]
        [Route("downloadQuotationRequest")]
        [Authorize]
        public HttpResponseMessage DownloadQuotationRequest(ServiceRequestSearch search)
        {
            HttpResponseMessage httpResponse = Request.CreateResponse(System.Net.HttpStatusCode.OK);
            GetFileResponse(search, httpResponse);
            return httpResponse;
        }
        private void GetFileResponse(ServiceRequestSearch search, HttpResponseMessage httpResponse)
        {
            List<QuotationRequest> transfers = GetQuotationRequestList(search, true);
            byte[] buff = null;
            if (transfers.Count > 0)
            {
                if (search.ExportType == ExportType.PDF)
                {
                    var file = new BudgetAllocationPDFGenerator<QuotationRequest>
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
                    var file = new BudgetAllocationExcelGeneratore<QuotationRequest>
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