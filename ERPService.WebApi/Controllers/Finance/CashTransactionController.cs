using ERPService.BC;
using ERPService.Common.Interfaces;
using ERPService.Common.Shared;
using ERPService.DataModel.CTO;
using ERPService.DataModel.DTO;
using Serilog;
using System;
using System.Collections.Generic;
using System.Web.Http;
using Microsoft.Owin;
using System.Net.Http;
using ERPService.BC.Utility;
using ERPService.Common;

namespace ERPService.WebApi
{
    [Route("api/[controller]")]
    [Authorize]
    public class CashTransactionController : AppApiBaseController
    {

        public CashTransactionController(ILogger logger, IRepository repository, IOwinContext context) : base(logger, repository, context)
        {
        }
        [HttpGet]
        [Route("getCashTransactionById/{transId}")]
        [Authorize]
        public CashTransaction GetCashTransactionById(Guid transId)
        {
            var cashTransBC = new CashTransactionBC(_logger, _repository);
            return cashTransBC.GetCashTransactionById(transId);
        }
        [HttpPost]
        [Route("getCashTransactions")]
        [Authorize]
        public List<CashTransaction> GetCashTransactions(CashTransaction search)
        {
            return GetCashTrasactionsByCriteria(search);
        }

        private List<CashTransaction> GetCashTrasactionsByCriteria(CashTransaction search, bool isExport = false)
        {
            var cashTransBC = new CashTransactionBC(_logger, _repository);
            return cashTransBC.GetCashTransactions(search, isExport);
        }

        [HttpPost]
        [Route("downloadReceipts")]
        [Authorize]
        public HttpResponseMessage DownloadReceipts(CashTransaction search)
        {
            HttpResponseMessage httpResponse = Request.CreateResponse(System.Net.HttpStatusCode.OK);
            GetFileResponse(search, httpResponse);
            return httpResponse;
        }

        [HttpPost]
        [Route("downloadExpenses")]
        [Authorize]
        public HttpResponseMessage DownloadExpenses(CashTransaction search)
        {
            HttpResponseMessage httpResponse = Request.CreateResponse(System.Net.HttpStatusCode.OK);
            GetFileResponse(search, httpResponse);
            return httpResponse;
        }

        private void GetFileResponse(CashTransaction search, HttpResponseMessage httpResponse)
        {
            List<CashTransaction> transfers = GetCashTrasactionsByCriteria(search, true);
            byte[] buff = null;
            if (transfers.Count > 0)
            {
                if (search.ExportType == ExportType.PDF)
                {
                    var file = new PDFGenerator<CashTransaction>
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
                    var file = new ExcelGenerator<CashTransaction>
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
        [Route("getPettyCashBalance")]
        [Authorize]
        public List<PettyCashBalanceResponse> GetPettyCashBalance(PettyCashBalanceSearch search)
        {
            var cashTransBC = new CashTransactionBC(_logger, _repository);
            return cashTransBC.GetPettyCashBalance(search);
        }

        [HttpPost]
        [Route("saveCashTransaction")]
        [Authorize]
        public AppResponse SaveCashTransaction(CashTransaction transaction)
        {
            var cashTransBC = new CashTransactionBC(_logger, _repository);
            return cashTransBC.SaveCashTransaction(transaction);
        }

        [HttpPost]
        [Route("transferPettyCash")]
        [Authorize]
        public AppResponse TransferPettyCash(PettyCashTransfer pettyCashTransfer)
        {
            var cashTrasanctionBC = new CashTransactionBC(_logger, _repository);
            return cashTrasanctionBC.TransferPettyCash(pettyCashTransfer, _userContext);
        }

        [HttpGet]
        [Route("tellerNameList")]
        [Authorize]
        public List<PettyCashTeller> tellerNameList()
        {
            CashTransactionBC tellerListBC = new CashTransactionBC(_logger, _repository);
            return tellerListBC.tellerNameList();
        }

        [HttpGet]
        [Route("accountNameList")]
        [Authorize]
        public List<PettyCashAccount> accountNameList()
        {
            CashTransactionBC accountListBC = new CashTransactionBC(_logger, _repository);
            return accountListBC.accountNameList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("processDayClosure")]
        [Authorize]
        public AppResponse ProcessDayClosure(DayClosureRequest request)
        {
            var cashTrasanctionBC = new CashTransactionBC(_logger, _repository);
            return cashTrasanctionBC.ProcessDayClosure(request);
        }

        /// <summary>
        /// Check whether day is closed for the logged in Teller
        /// </summary>
        /// <param name="request"></param>
        /// <returns>returns True if closed and False it is not</returns>
        [HttpPost]
        [Route("checkDayClosureStatus")]
        [Authorize]
        public bool CheckDayClosureStatus(DayClosureRequest request)
        {
            var cashTrasanctionBC = new CashTransactionBC(_logger, _repository);
            return cashTrasanctionBC.CheckDayClosureStatus(request);
        }
        [HttpGet]
        [Route ("getDefaultOrgId")]
        [Authorize]
        public Organization GetDefaultOrgId()
        {
            var cashTrasanctionBC = new CashTransactionBC(_logger, _repository);
            return cashTrasanctionBC.GetDefaultOrgId();
        }
    }
}