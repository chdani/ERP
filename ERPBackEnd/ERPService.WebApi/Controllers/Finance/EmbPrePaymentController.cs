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
    public class EmbPrePaymentController : AppApiBaseController
    {
        public EmbPrePaymentController(ILogger logger, IRepository repository, IOwinContext context) : base(logger, repository, context)
        {
        }

        [HttpPost]
        [Route("getEmbPrePayments")]
        [Authorize]
        public List<EmbPrePaymentHdr> GetEmbPrePayments(EmbPrePaymenteSearch search , bool isExport = false)
        {
            var embPrePaymentBC = new EmbassyPrePaymentBC(_logger, _repository);
            return embPrePaymentBC.GetEmbPrePaymentList(search , isExport);
        }

        [HttpPost]
        [Route("getEmbPrePaymentDues")]
        [Authorize]
        public List<EmbPrePaymentDue> GetPrePaymentDues(EmbPrePaymentDueSearch search)
        {
            var embPrePaymentBC = new EmbassyPrePaymentBC(_logger, _repository);
            return embPrePaymentBC.GetPrePaymentDues(search);
        }

        [HttpGet]
        [Route("getEmbPrePaymentById/{id}")]
        [Authorize]
        public EmbPrePaymentHdr GetEmbPrePaymentById(Guid id)
        {
            var embPrePaymentBC = new EmbassyPrePaymentBC(_logger, _repository);
            return embPrePaymentBC.GetEmbPrePaymentById(id);
        }

        [HttpGet]
        [Route("getEmbPrePaymentEmbDetByHeaderId/{id}")]
        [Authorize]
        public List<EmbPrePaymentEmbDet> GetEmbPrePaymentDetByHeaderId(Guid id)
        {
            var embPrePaymentBC = new EmbassyPrePaymentBC(_logger, _repository);
            return embPrePaymentBC.GetEmbPrePaymentDetByHdrId(id);
        }

        [HttpGet]
        [Route("getEmbPrePayInvbDetByEmbDetId/{id}")]
        [Authorize]
        public List<EmbPrePaymentInvDet> GetEmbPrePayInvbDetByEmbDetId(Guid id)
        {
            var embPrePaymentBC = new EmbassyPrePaymentBC(_logger, _repository);
            return embPrePaymentBC.GetEmbPrePayInvDetByEmbDetId(id);
        }

        [HttpPost]
        [Route("saveEmbPrePayment")]
        [Authorize]
        public AppResponse SaveEmbPrePayment(EmbPrePaymentHdr embPrePayment)
        {
            var embPrePaymentBC = new EmbassyPrePaymentBC(_logger, _repository);
            return embPrePaymentBC.SaveEmbPrePayment(embPrePayment);
        }

        [HttpPost]
        [Route("saveMultipleEmbPrePayment")]
        [Authorize]
        public AppResponse SaveMultipleEmbPrePayment(List<EmbPrePaymentHdr> embPrePayment)
        {
            var embPrePaymentBC = new EmbassyPrePaymentBC(_logger, _repository);
            return embPrePaymentBC.SaveEmbPrePaymentList(embPrePayment);
        }

        [HttpPost]
        [Route("approveReturnEmbPrePay")]
        [Authorize]
        public AppResponse ApproveReturnEmbPrePay(EmbPrePaymentHdr embPrePayment)
        {
            var embPrePaymentBC = new EmbassyPrePaymentBC(_logger, _repository);
            embPrePayment.ApprovedBy = _userContext.Id;
            embPrePayment.ApprovedDate = DateTime.Now;

            return embPrePaymentBC.ApproveRejectEmbPrePayment(embPrePayment);
        }
        [HttpPost]
        [Route("downloadEmbassyPrePayment")]
        [Authorize]
        public HttpResponseMessage downloadEmbassyPrePayment(EmbPrePaymenteSearch search)
        {
            HttpResponseMessage httpResponse = Request.CreateResponse(System.Net.HttpStatusCode.OK);
            GetFileResponse(search, httpResponse);
            return httpResponse;
        }
        private void GetFileResponse(EmbPrePaymenteSearch search, HttpResponseMessage httpResponse)
        {
            List<EmbPrePaymentHdr> transfers = GetEmbPrePayments(search, true);
            byte[] buff = null;
            if (transfers.Count > 0)
            {
                if (search.ExportType == ExportType.PDF)
                {
                    var file = new EmbPrePaymentPDFGenerator<EmbPrePaymentHdr>
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
                    var file = new EmbPrePaymentExcelGnenertor<EmbPrePaymentHdr>
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