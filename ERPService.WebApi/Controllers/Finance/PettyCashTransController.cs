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
using System.IO;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace ERPService.WebApi
{
    [Route("api/pettyCashTrans")]
    [Authorize]
    public class PettyCashTransController : AppApiBaseController
    {

        public PettyCashTransController(ILogger logger, IRepository repository, IOwinContext context) :base(logger, repository, context)
        {
        }

        [HttpPost]
        [Route("getPettyCashTransactions")]
        [Authorize]
        public List<DataModel.DTO.PettyCashTransfer> GetPettyCashTransaction(DataModel.DTO.PettyCashTransfer search)
        {
            List<PettyCashTransfer> transfers = GetCashTransfers(search);
            return transfers;
        }

        [HttpPost]
        [Route("downloadTransfers")]
        [Authorize]
        public HttpResponseMessage DownloadTransfers(PettyCashTransfer search)
        {
            HttpResponseMessage httpResponse = Request.CreateResponse(System.Net.HttpStatusCode.OK);
            List<PettyCashTransfer> transfers = GetCashTransfers(search);
            byte[] buff = null;
            if (transfers.Count > 0)
            {
                if (search.ExportType == ExportType.PDF)
                {
                    var file = new PDFGenerator<PettyCashTransfer>
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
                    var file = new ExcelGenerator<PettyCashTransfer>
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
            return httpResponse;
        }

        private List<PettyCashTransfer> GetCashTransfers(PettyCashTransfer search)
        {
            var pettyCashTransBC = new PettyCashTransBC(_logger, _repository);
            var transfers = pettyCashTransBC.GetPettyCashTransactions(search);
            return transfers;
        }

        [HttpGet]
        [Route("getPettyCashTransactionById/{transId}")]
        [Authorize]
        public DataModel.DTO.PettyCashTransfer GetPettyCashTransactionById(Guid transId)
        {
            var pettyCashTransBC = new PettyCashTransBC(_logger, _repository);
            return pettyCashTransBC.GetPettyCashTransactionById(transId);
        }

        [HttpPost]
        [Route("savePettyCashTrans")]
        [Authorize]
        public AppResponse SavePettyCashTrans(DataModel.DTO.PettyCashTransfer pettyCashTrans)
        {
            var pettyCashTransBC = new PettyCashTransBC(_logger, _repository);
            return pettyCashTransBC.SavePettyCashTrans(pettyCashTrans);
        }
    }
}