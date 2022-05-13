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

namespace ERPService.WebApi
{
    [Route("api/[controller]")]
    [Authorize]
    public class DirectInvPostPaymentController : AppApiBaseController
    {
        public DirectInvPostPaymentController(ILogger logger, IRepository repository, IOwinContext context) : base(logger, repository, context)
        {
        }

        [HttpGet]
        [Route("getDirectInvPostPayById/{invoiceId}")]
        [Authorize]
        public DirectInvPostPayment GetDirectInvPostPayById(Guid invoiceId)
        {
            DirectInvPostPaymentBC directInoiceBC = new DirectInvPostPaymentBC(_logger, _repository);
            return directInoiceBC.GetDirectInvPostPaymentById(invoiceId);
        }
        [HttpPost]
        [Route("downloadPostpayment")]
        [Authorize]
        public HttpResponseMessage DownloadPostpayment(DirectInvSearch search)
        {
            HttpResponseMessage httpResponse = Request.CreateResponse(System.Net.HttpStatusCode.OK);
            GetFileResponse(search, httpResponse);
            return httpResponse;
        }
        private void GetFileResponse(DirectInvSearch search, HttpResponseMessage httpResponse)
        {
            List<DirectInvPostPayment> transfers = GetDirectInvPostPayList(search, true);
            byte[] buff = null;
            if (transfers.Count > 0)
            {
                if (search.ExportType == ExportType.PDF)
                {
                    var file = new PDFGenerator<DirectInvPostPayment>
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
                    var file = new ExcelGenerator<DirectInvPostPayment>
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



        [HttpPost]
        [Route("getDirectInvPostPayList")]
        [Authorize]
        public List<DirectInvPostPayment> GetDirectInvPostPayList(DirectInvSearch search, bool isExport = false)
        {
            DirectInvPostPaymentBC directInoiceBC = new DirectInvPostPaymentBC(_logger, _repository);
            return directInoiceBC.GetDirectInvPostPaymentList(search, isExport);
        }

        [HttpPost]
        [Route("saveDirectInvPostPay")]
        [Authorize]
        public AppResponse SaveDirectInvPostPay(DirectInvPostPayment invoice)
        {
            DirectInvPostPaymentBC directInoiceBC = new DirectInvPostPaymentBC(_logger, _repository);
            return directInoiceBC.SaveDirectInvPostPayment(invoice);
        }

        [HttpPost]
        [Route("approveReturnDirInvoicePostPay")]
        [Authorize]
        public AppResponse ApproveReturnDirInvoicePostPay(DirectInvPostPayment invoice)
        {
            DirectInvPostPaymentBC directInoiceBC = new DirectInvPostPaymentBC(_logger, _repository);
            invoice.ApprovedBy = _userContext.Id;
            invoice.ApprovedDate = DateTime.Now;


            return directInoiceBC.SaveDirectInvPostPayment(invoice);
        }
    }
}