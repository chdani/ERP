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
    public class PurchaseRequestController : AppApiBaseController
    {
        public PurchaseRequestController(ILogger logger, IRepository repository, IOwinContext context) : base(logger, repository, context)
        {
        }

        [HttpGet]
        [Route("getPurchaseRequestById/{purchaseRequestId}")]
        [Authorize]
        public PurchaseRequest GetPurchaseRequestById(Guid purchaseRequestId)
        {
            PurchaseRequestBC purchaseReqBC = new PurchaseRequestBC(_logger, _repository, _userContext);
            return purchaseReqBC.GetPurchaseRequestById(purchaseRequestId);
        }

        [HttpGet]
        [Route("getPurchaseReqDetByHdrId/{purchaseRequestId}")]
        [Authorize]
        public List<PurchaseRequestDet> GetPurchaseReqDetByHdrId(Guid purchaseRequestId)
        {
            PurchaseRequestBC purchaseReqBC = new PurchaseRequestBC(_logger, _repository, _userContext);
            return purchaseReqBC.GetPurchaseReqDetByHdrId(purchaseRequestId);
        }


        [HttpPost]
        [Route("savePurchaseRequest")]
        [Authorize]
        public AppResponse SavePurchaseRequest(PurchaseRequest purchaseRequest)
        {
            PurchaseRequestBC purchaseReqBC = new PurchaseRequestBC(_logger, _repository, _userContext);
            return purchaseReqBC.SavePurchaseRequest(purchaseRequest);
        }
        [HttpPost]
        [Route("savePurchaseRequestComment")]
        [Authorize]
        public AppResponse SavePurchaseRequestComment(PurchaseReqComment purchaseReqComment)
        {
            PurchaseRequestBC purchaseReqBC = new PurchaseRequestBC(_logger, _repository, _userContext);
            return purchaseReqBC.SavePurchaseRequestComment(purchaseReqComment);
        }
        [HttpPost]
        [Route("savePurchaseReqDetComment")]
        [Authorize]
        public AppResponse SavePurchaseReqDetComment(PurchaseReqDetComment PurchaseReqDetComment)
        {
            PurchaseRequestBC purchaseReqBC = new PurchaseRequestBC(_logger, _repository, _userContext);
            return purchaseReqBC.SavePurchaseReqDetComment(PurchaseReqDetComment);
        }

        [HttpPost]
        [Route("approvePurchaseRequest")]
        [Authorize]
        public AppResponse ApprovePurchaseRequest(PurchaseRequest purchaseRequest)
        {
            PurchaseRequestBC purchaseReqBC = new PurchaseRequestBC(_logger, _repository, _userContext);
            return purchaseReqBC.ApprovePurchaseRequest(purchaseRequest);
        }


        [HttpPost]
        [Route("getPurchaseRequestList")]
        [Authorize]
        public List<PurchaseRequest> GetPurchaseRequestList(PurchaseRequest purchaseRequest, bool isExport = false)
        {
            PurchaseRequestBC purchaseReqBC = new PurchaseRequestBC(_logger, _repository, _userContext);
            return purchaseReqBC.GetPurchaseRequestList(purchaseRequest, isExport);
        }
        [HttpGet]
        [Route("getPurchaseReqHistory/{purchaseReqId}")]
        [Authorize]
        public List<PurchaseReqHist> GetPurchaseReqHistory(Guid purchaseReqId)
        {
            PurchaseRequestBC purchaseReqBC = new PurchaseRequestBC(_logger, _repository, _userContext);
            return purchaseReqBC.GetPurchaseReqHistory(purchaseReqId);
        }

        [HttpGet]
        [Route("getPurchaseReqDetHistory/{purchaseReqDetId}")]
        [Authorize]
        public List<PurchaseReqDetHist> GetPurchaseReqDetHistory(Guid purchaseReqDetId)
        {
            PurchaseRequestBC purchaseReqBC = new PurchaseRequestBC(_logger, _repository, _userContext);
            return purchaseReqBC.GetPurchaseReqDetHistory(purchaseReqDetId);
        }

        [HttpGet]
        [Route("getPurchaseReqComment/{purchaseReqId}")]
        [Authorize]
        public List<PurchaseReqComment> GetPurchaseReqComment(Guid purchaseReqId)
        {
            PurchaseRequestBC purchaseReqBC = new PurchaseRequestBC(_logger, _repository, _userContext);
            return purchaseReqBC.GetPurchaseReqComment(purchaseReqId);
        }

        [HttpGet]
        [Route("getPurchaseReqDetComment/{purchaseReqDetId}")]
        [Authorize]
        public List<PurchaseReqDetComment> GetPurchaseReqDetComment(Guid purchaseReqDetId)
        {
            PurchaseRequestBC purchaseReqBC = new PurchaseRequestBC(_logger, _repository, _userContext);
            return purchaseReqBC.GetPurchaseReqDetComment(purchaseReqDetId);
        }

        [HttpGet]
        [Route("getPurchaseReqStatusHistory/{purchaseReqId}")]
        [Authorize]
        public List<PurchaseRequestStatusHist> GetPurchaseReqStatusHistory(Guid purchaseReqId)
        {
            PurchaseRequestBC purchaseReqBC = new PurchaseRequestBC(_logger, _repository, _userContext);
            return purchaseReqBC.GetPurchaseReqStatusHistory(purchaseReqId);
        }

        [HttpGet]
        [Route("getPurchaseReqAttachments/{purchaseReqId}")]
        [Authorize]
        public List<AppDocument> GetPurchaseReqAttachments(Guid purchaseReqId)
        {
            PurchaseRequestBC purchaseReqBC = new PurchaseRequestBC(_logger, _repository, _userContext);
            return purchaseReqBC.GetPurchaseReqAttachments(purchaseReqId);
        }

        [HttpGet]
        [Route("getPurchaseReqDetAttachments/{purchaseReqDetId}")]
        [Authorize]
        public List<AppDocument> GetPurchaseReqDetAttachments(Guid purchaseReqDetId)
        {
            PurchaseRequestBC purchaseReqBC = new PurchaseRequestBC(_logger, _repository, _userContext);
            return purchaseReqBC.GetPurchaseReqDetAttachments(purchaseReqDetId);
        }
        [HttpPost]
        [Route("downloadPurchaseReq")]
        [Authorize]
        public HttpResponseMessage DownloadPurchaseReq(PurchaseRequest search)
        {
            HttpResponseMessage httpResponse = Request.CreateResponse(System.Net.HttpStatusCode.OK);
            GetFileResponse(search, httpResponse);
            return httpResponse;
        }
        private void GetFileResponse(PurchaseRequest search, HttpResponseMessage httpResponse)
        {
            List<PurchaseRequest> transfers = GetPurchaseRequestList(search, true);
            byte[] buff = null;
            if (transfers.Count > 0)
            {
                if (search.ExportType == ExportType.PDF)
                {
                    var file = new BudgetAllocationPDFGenerator<PurchaseRequest>
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
                    var file = new BudgetAllocationExcelGeneratore<PurchaseRequest>
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
