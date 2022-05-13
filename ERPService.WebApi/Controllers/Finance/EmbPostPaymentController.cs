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
    [Route("api/embPrePayment")]
    [Authorize]
    public class EmbPostPaymentController : AppApiBaseController
    {
 
        public EmbPostPaymentController(ILogger logger, IRepository repository, IOwinContext context) : base(logger, repository, context)
        {
        }

        [HttpPost]
        [Route("getEmbPostPayments")]
        [Authorize]
        public List<EmbPostPayment> GetEmbPostPayments(EmbPostPaymenteSearch search)
        {
            var embPostPaymentBC = new EmbPostPaymentBC(_logger, _repository);
            return embPostPaymentBC.GetEmbPostPaymentList(search);
        }

        [HttpGet]
        [Route("getEmbPostPaymentById/{id}")]
        [Authorize]
        public EmbPostPayment GetEmbPostPaymentById(Guid id)
        {
            var embPostPaymentBC = new EmbPostPaymentBC(_logger, _repository);
            return embPostPaymentBC.GetEmbPostPaymentById(id);
        }

        [HttpPost]
        [Route("saveEmbPostPayment")]
        [Authorize]
        public AppResponse SaveEmbPostPayment(EmbPostPayment embPostPayment)
        {
            var embPostPaymentBC = new EmbPostPaymentBC(_logger, _repository);
            return embPostPaymentBC.SaveEmbPostPayment(embPostPayment);
        }

        [HttpPost]
        [Route("approveReturnEmbPostPay")]
        [Authorize]
        public AppResponse ApproveReturnEmbPostPay(EmbPostPayment embPostPayment)
        {
            var embPostPaymentBC = new EmbPostPaymentBC(_logger, _repository);
            embPostPayment.ApprovedBy = _userContext.Id;
            embPostPayment.ApprovedDate = DateTime.Now;

            return embPostPaymentBC.ApproveRejectEmbPostPayment(embPostPayment);
        }

        [HttpPost]
        [Route("downloadEmbassyPostPayment")]
        [Authorize]
        public HttpResponseMessage DownloadEmbassyPostPayment(EmbPostPaymenteSearch search)
        {
            HttpResponseMessage httpResponse = Request.CreateResponse(System.Net.HttpStatusCode.OK);
            GetFileResponse(search, httpResponse);
            return httpResponse;
        }

        private void GetFileResponse(EmbPostPaymenteSearch search, HttpResponseMessage httpResponse)
        {
            List<EmbPostPayment> transfers = GetEmbPostPayments(search);
            byte[] buff = null;
            if (transfers.Count > 0)
            {
                if (search.ExportType == ExportType.PDF)
                {
                    var file = new PDFGenerator<EmbPostPayment>
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
                    var file = new ExcelGenerator<EmbPostPayment>
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

        ///////// History && Comments/////


        [HttpPost]
        [Route("saveEmbPostPaymentHdrHistComment")]
        [Authorize]
        public List<EmbPostPaymentComment> SaveEmbPostPaymentHdrHistComment(EmbPostPaymentComment histroy)
        {
            var embPostPaymentBC = new EmbPostPaymentBC(_logger, _repository);
            return embPostPaymentBC.SaveEmbPostPaymentHdrHistComment(histroy);
        }
        [HttpGet]
        [Route("getEmbPostPaymentHdrHistComment/{Id}")]
        [Authorize]
        public List<EmbPostPaymentComment> getEmbPostPaymentHdrHistComment(Guid Id)
        {
            var embPostPaymentBC = new EmbPostPaymentBC(_logger, _repository);
            return embPostPaymentBC.GetEmbPostPaymentHdrHistComment(Id);
        }
        [HttpGet]
        [Route("GetEmbPostPaymentHistoryStatus/{Id}")]
        [Authorize]
        public List<EmbPostPaymentStatusHist> GetEmbPostPaymentHistoryStatus(Guid Id)
        {
            var embPostPaymentBC = new EmbPostPaymentBC(_logger, _repository);
            return embPostPaymentBC.GetEmbPostPaymentHistoryStatus(Id);
        }
        [HttpGet]
        [Route("getEmbPostPaymentHistory/{Id}")]
        [Authorize]
        public List<EmbPostPaymentHist> GetEmbPostPaymentHistory(Guid Id)
        {
            var embPostPaymentBC = new EmbPostPaymentBC(_logger, _repository);
            return embPostPaymentBC.GetEmbPostPaymentHistory(Id);
        }


        [HttpGet]
        [Route("getEmbPostPaymentAttachments/{Id}")]
        [Authorize]
        public List<AppDocument> GetEmbPostPaymentAttachments(Guid Id)
        {
            var embPostPaymentBC = new EmbPostPaymentBC(_logger, _repository);
            return embPostPaymentBC.GetEmbPostPaymentAttachments(Id);
        }


    }
}