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
    public class DirectInvPrePaymentController : AppApiBaseController
    {

        public DirectInvPrePaymentController(ILogger logger, IRepository repository, IOwinContext context) : base(logger, repository, context)
        {

        }

        [HttpGet]
        [Route("getDirectInvPrePayById/{invoiceId}")]
        [Authorize]
        public DirectInvPrePayment GetDirectInvPrePayById(Guid invoiceId)
        {
            DirectInvPrePaymentBC directInvoiceBC = new DirectInvPrePaymentBC(_logger, _repository);
            return directInvoiceBC.GetDirectInvPrePaymentById(invoiceId);
        }

        [HttpPost]
        [Route("getDirInvPrePaymentDues")]
        [Authorize]
        public List<DirectInvPrePaymentDue> GetDirInvPrePaymentDues(DirectInvPrePaydueSearch search)
        {
            var directInvoiceBC = new DirectInvPrePaymentBC(_logger, _repository);
            return directInvoiceBC.GetPrePaymentDues(search);
        }

        [HttpPost]
        [Route("downloadPrepayment")]
        [Authorize]
        public HttpResponseMessage DownloadPrepayment(DirectInvSearch search)
        {
            HttpResponseMessage httpResponse = Request.CreateResponse(System.Net.HttpStatusCode.OK);
            GetFileResponse(search, httpResponse);
            return httpResponse;
        }

        private void GetFileResponse(DirectInvSearch search, HttpResponseMessage httpResponse)
        {
            List<DirectInvPrePayment> transfers = GetDirectInvPrePayList(search, true);
            byte[] buff = null;
            if (transfers.Count > 0)
            {
                if (search.ExportType == ExportType.PDF)
                {
                    var file = new PDFGenerator<DirectInvPrePayment>
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
                    var file = new ExcelGenerator<DirectInvPrePayment>
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
        [Route("getDirectInvPrePayList")]
        [Authorize]
        public List<DirectInvPrePayment> GetDirectInvPrePayList(DirectInvSearch search, bool isExport = false)
        {
            DirectInvPrePaymentBC directInvoiceBC = new DirectInvPrePaymentBC(_logger, _repository);
            return directInvoiceBC.GetDirectInvPrePaymentList(search, isExport);
        }

        [HttpPost]
        [Route("saveDirectInvPrePay")]
        [Authorize]
        public AppResponse SaveDirectInvPrePay(DirectInvPrePayment invoice, bool SaveChanges = true)
        {
            DirectInvPrePaymentBC directInvoiceBC = new DirectInvPrePaymentBC(_logger, _repository);
            return directInvoiceBC.SaveDirectInvPrePayment(invoice, true);
        }

        [HttpPost]
        [Route("approveReturnDirInvoicePrePay")]
        [Authorize]
        public AppResponse ApproveReturnDirInvoicePrePay(DirectInvPostPayment invoice)
        {
            DirectInvPostPaymentBC directInvoiceBC = new DirectInvPostPaymentBC(_logger, _repository);
            invoice.ApprovedBy = _userContext.Id;
            invoice.ApprovedDate = DateTime.Now;

            return directInvoiceBC.SaveDirectInvPostPayment(invoice);
        }
    }
}