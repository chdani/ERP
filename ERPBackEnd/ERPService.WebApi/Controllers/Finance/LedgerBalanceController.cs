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
    public class LedgerBalanceController : AppApiBaseController
    {


        public LedgerBalanceController(ILogger logger, IRepository repository, IOwinContext context) : base(logger, repository, context)
        {

        }
        [HttpGet]
        [Route("getLedgerBalanceById/{ledgerBalId}")]
        [Authorize]
        public LedgerBalance GetLedgerBalanceById(Guid ledgerBalId)
        {
            var ledgerBalanceBC = new LedgerBalanceBC(_logger, _repository);
            return ledgerBalanceBC.GetLedgerBalanceById(ledgerBalId);
        }
        [HttpPost]
        [Route("getLedgerHistroy")]
        [Authorize]
        public List<LedgerBalanceReport> GetLedgerHistroy(LedgerBalanceSearch ledgerBalance, bool isExport = false)
        {
            var ledgerBalanceBC = new LedgerBalanceBC(_logger, _repository);
            return ledgerBalanceBC.GetLedgerHistroy(ledgerBalance, isExport);
        }
        [HttpPost]
        [Route("getLedgerBalances")]
        [Authorize]
        public List<LedgerBalanceSummary> GetLedgerBalances(LedgerBalanceSearch ledgerBalance, bool isExport = false)
        {
            var ledgerBalanceBC = new LedgerBalanceBC(_logger, _repository);
            return ledgerBalanceBC.GetLedgerBalances(ledgerBalance, isExport);
        }

        [HttpPost]
        [Route("getLedgerAccWiseCurrentBalance")]
        [Authorize]
        public List<LedgerBalanceSummary> GetLedgerAccWiseCurrentBalance(LedgerBalanceSearch ledgerBalance)
        {
            var ledgerBalanceBC = new LedgerBalanceBC(_logger, _repository);
            return ledgerBalanceBC.GetLedgerAccWiseCurrentBalance(ledgerBalance);
        }

        [HttpPost]
        [Route("saveLedgerBalance")]
        [Authorize]
        public AppResponse SaveLedgerBalance(LedgerBalance ledgerBalance)
        {
            var ledgerBalanceBC = new LedgerBalanceBC(_logger, _repository);
            return ledgerBalanceBC.SaveLedgerBalance(ledgerBalance);
        }

        [HttpPost]
        [Route("downloadLedgerHistroy")]
        [Authorize]
        public HttpResponseMessage DownloadLedgerHistroy(LedgerBalanceSearch search)
        {
            HttpResponseMessage httpResponse = Request.CreateResponse(System.Net.HttpStatusCode.OK);
            GetFileResponse(search, httpResponse);
            return httpResponse;
        }
        private void GetFileResponse(LedgerBalanceSearch search, HttpResponseMessage httpResponse)
        {
            List<LedgerBalanceReport> transfers = GetLedgerHistroy(search, true);
            byte[] buff = null;
            if (transfers.Count > 0)
            {
                if (search.ExportType == ExportType.PDF)
                {
                    var file = new PDFGenerator<LedgerBalanceReport>
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
                    var file = new ExcelGenerator<LedgerBalanceReport>
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
        [Route("downloadLedgerBalance")]
        [Authorize]
        public HttpResponseMessage DownloadLedgerBalance(LedgerBalanceSearch search)
        {
            HttpResponseMessage httpResponse = Request.CreateResponse(System.Net.HttpStatusCode.OK);
            GetFileResponse1(search, httpResponse);
            return httpResponse;
        }
        private void GetFileResponse1(LedgerBalanceSearch search, HttpResponseMessage httpResponse)
        {
            List<LedgerBalanceSummary> transfers = GetLedgerBalances(search, true);
            byte[] buff = null;
            if (transfers.Count > 0)
            {
                if (search.ExportType == ExportType.PDF)
                {
                    var file = new PDFGenerator<LedgerBalanceSummary>
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
                    var file = new ExcelGenerator<LedgerBalanceSummary>
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