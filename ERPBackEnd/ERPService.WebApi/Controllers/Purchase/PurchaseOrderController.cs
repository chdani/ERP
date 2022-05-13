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
    public class PurchaseOrderController : AppApiBaseController
    {
        public PurchaseOrderController(ILogger logger, IRepository repository, IOwinContext context) : base(logger, repository, context)
        {
        }

        [HttpGet]
        [Route("getPurchaseOrderById/{purchaseOrderId}")]
        [Authorize]
        public PurchaseOrder GetPurchaseOrderById(Guid purchaseOrderId)
        {
            PurchaseOrderBC purchaseOrderBC = new PurchaseOrderBC(_logger, _repository, _userContext);
            return purchaseOrderBC.GetPurchaseOrderById(purchaseOrderId);
        }

        [HttpGet]
        [Route("getPurchaseOrderDetByHdrId/{purchaseOrderId}")]
        [Authorize]
        public List<PurchaseOrderDet> GetPurchaseOrderDetByHdrId(Guid purchaseOrderId)
        {
            PurchaseOrderBC purchaseOrderBC = new PurchaseOrderBC(_logger, _repository, _userContext);
            return purchaseOrderBC.GetPurchaseOrderDetByHdrId(purchaseOrderId);
        }

        [HttpPost]
        [Route("getPurchaseOrderList")]
        [Authorize]
        public List<PurchaseOrder> GetPurchaseOrders(PurchaseOrderSearch input, bool isExport = false)
        {
            PurchaseOrderBC purchaseOrderBC = new PurchaseOrderBC(_logger, _repository, _userContext);
            return purchaseOrderBC.GetPurchaseOrders(input, isExport);
        }
        [HttpGet]
        [Route("getPurchaseRequestId/{purchaseOrderId}")]
        [Authorize]
        public PurchaseRequest GetPurchaseRequestId(Guid purchaseOrderId)
        {
            PurchaseOrderBC purchaseOrderBC = new PurchaseOrderBC(_logger, _repository, _userContext);
            return purchaseOrderBC.GetPurchaseRequestId(purchaseOrderId);
        }
        [HttpGet]
        [Route("getPurchaseRequestDetByHdrId/{purchaseOrderId}")]
        [Authorize]
        public List<PurchaseRequestDet> GetPurchaseRequestDetByHdrId(Guid purchaseOrderId)
        {
            PurchaseOrderBC purchaseOrderBC = new PurchaseOrderBC(_logger, _repository, _userContext);
            return purchaseOrderBC.GetPurchaseRequestDetByHdrId(purchaseOrderId);
        }
        [HttpPost]
        [Route("getPurchaseRequest")]
        [Authorize]
        public List<PurchaseRequest> GetPurchaseRequest(PurchaseRequest input)
        {
            PurchaseOrderBC purchaseOrderBC = new PurchaseOrderBC(_logger, _repository, _userContext);
            return purchaseOrderBC.GetPurchaseRequest(input);
        }
        [HttpPost]
        [Route("savePurchaseOrder")]
        [Authorize]
        public AppResponse SavePurchaseOrder(PurchaseOrder purchaseOrder)
        {
            PurchaseOrderBC purchaseOrderBC = new PurchaseOrderBC(_logger, _repository, _userContext);
            return purchaseOrderBC.SavePurchaseOrder(purchaseOrder);
        }
        [HttpPost]
        [Route("approvePurchaseOrder")]
        [Authorize]
        public AppResponse ApprovePurchaseOrder(PurchaseOrder purchaseOrder)
        {
            PurchaseOrderBC purchaseOrderBC = new PurchaseOrderBC(_logger, _repository, _userContext);
            return purchaseOrderBC.ApprovePurchaseOrder(purchaseOrder);
        }
        [HttpGet]
        [Route("getPurchaseOrderDetComment/{id}")]
        [Authorize]
        public List<PurchaseOrdDetComment> GetPurchaseOrderDetComment(Guid id)
        {
            PurchaseOrderBC purchaseOrderBC = new PurchaseOrderBC(_logger, _repository, _userContext);
            return purchaseOrderBC.GetPurchaseOrderDetComment(id);
        }
        [HttpGet]
        [Route("getPurchaseOrderDetHistory/{id}")]
        [Authorize]
        public List<PurchaseOrdDetHist> GetPurchaseOrderDetHistory(Guid id)
        {
            PurchaseOrderBC purchaseOrderBC = new PurchaseOrderBC(_logger, _repository, _userContext);
            return purchaseOrderBC.GetPurchaseOrderDetHistory(id);
        }
        [HttpPost]
        [Route("savePurchaseOrderDetComment")]
        [Authorize]
        public AppResponse SavePurchaseOrderDetComment(PurchaseOrdDetComment purchaseOrdDet)
        {
            PurchaseOrderBC purchaseOrderBC = new PurchaseOrderBC(_logger, _repository, _userContext);
            return purchaseOrderBC.SavePurchaseOrderDetComment(purchaseOrdDet);
        }
        [HttpPost]
        [Route("savePurchaseOrderComment")]
        [Authorize]
        public AppResponse SavePurchaseOrderComment(PurchaseOrdComment purchaseOrd)
        {
            PurchaseOrderBC purchaseOrderBC = new PurchaseOrderBC(_logger, _repository, _userContext);
            return purchaseOrderBC.SavePurchaseOrderComment(purchaseOrd);
        }
        [HttpGet]
        [Route("getPurchaseOrderComment/{id}")]
        [Authorize]
        public List<PurchaseOrdComment> GetPurchaseOrderComment(Guid id)
        {
            PurchaseOrderBC purchaseOrderBC = new PurchaseOrderBC(_logger, _repository, _userContext);
            return purchaseOrderBC.GetPurchaseOrderComment(id);
        }
        [HttpGet]
        [Route("getPurchaseOrderAttachments/{id}")]
        [Authorize]
        public List<AppDocument> GetPurchaseOrderAttachments(Guid id)
        {
            PurchaseOrderBC purchaseOrderBC = new PurchaseOrderBC(_logger, _repository, _userContext);
            return purchaseOrderBC.GetPurchaseOrderAttachments(id);
        }
        [HttpGet]
        [Route("getPurchaseOrderStatusHistory/{id}")]
        [Authorize]
        public List<PurchaseOrdStatusHist> GetPurchaseOrderStatusHistory(Guid id)
        {
            PurchaseOrderBC purchaseOrderBC = new PurchaseOrderBC(_logger, _repository, _userContext);
            return purchaseOrderBC.GetPurchaseOrderStatusHistory(id);
        }
        [HttpGet]
        [Route("getPurchaseOrderHistory/{id}")]
        [Authorize]
        public List<PurchaseOrdHist> GetPurchaseOrderHistory(Guid id)
        {
            PurchaseOrderBC purchaseOrderBC = new PurchaseOrderBC(_logger, _repository, _userContext);
            return purchaseOrderBC.GetPurchaseOrderHistory(id);
        }
        [HttpGet]
        [Route("getPurchaseOrderDetAttachments/{id}")]
        [Authorize]
        public List<AppDocument> GetPurchaseOrderDetAttachments(Guid id)
        {
            PurchaseOrderBC purchaseOrderBC = new PurchaseOrderBC(_logger, _repository, _userContext);
            return purchaseOrderBC.GetPurchaseOrderDetAttachments(id);
        }
        [HttpPost]
        [Route("downloadPurchaseOrder")]
        [Authorize]
        public HttpResponseMessage DownloadPurchaseOrder(PurchaseOrderSearch search)
        {
            HttpResponseMessage httpResponse = Request.CreateResponse(System.Net.HttpStatusCode.OK);
            GetFileResponse(search, httpResponse);
            return httpResponse;
        }
        private void GetFileResponse(PurchaseOrderSearch search, HttpResponseMessage httpResponse)
        {
            List<PurchaseOrder> transfers = GetPurchaseOrders(search, true);
            byte[] buff = null;
            if (transfers.Count > 0)
            {
                if (search.ExportType == ExportType.PDF)
                {
                    var file = new BudgetAllocationPDFGenerator<PurchaseOrder>
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
                    var file = new BudgetAllocationExcelGeneratore<PurchaseOrder>
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
        [HttpGet]
        [Route("sendPurchaseOrderMail/{id}")]
        public AppResponse SendPurchaseOrderMail(Guid id)
        {
            PurchaseOrderBC purchaseOrderBC = new PurchaseOrderBC(_logger, _repository, _userContext);
            return purchaseOrderBC.SendPurchaseOrderMail(id);
        }
    }
}