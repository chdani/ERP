using ERPService.BC;
using ERPService.Common.Interfaces;
using ERPService.Common.Shared;
using ERPService.DataModel.DTO;
using Microsoft.Owin;
using Serilog;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace ERPService.WebApi
{
    [Route("api/[controller]")]
    [Authorize]
    public class CashTransactionHistoryCommentsController : AppApiBaseController
    {
        public CashTransactionHistoryCommentsController(ILogger logger, IRepository repository, IOwinContext context) : base(logger, repository, context)
        {
        }
        [HttpPost]
        [Route("saveCashTransactionHdrComment")]
        [Authorize]
        public List<CashTransacionComment> SaveCashTransactionHdrComment(CashTransacionComment histroy)
        {
            var cashTransactionHistroyCommentsBC = new CashTransactionHistoryCommentsBC(_logger, _repository);
            return cashTransactionHistroyCommentsBC.SaveCashTransactionHdrComment(histroy);
        }
        [HttpGet]
        [Route("getCashTransAcionComments/{Id}")]
        [Authorize]
        public List<CashTransacionComment> GetCashTransAcionComments(Guid Id)
        {
            var cashTransactionHistroyCommentsBC = new CashTransactionHistoryCommentsBC(_logger, _repository);
            return cashTransactionHistroyCommentsBC.GetCashTransAcionComments(Id);
        }

       
       
        [HttpGet]
        [Route("getCashTransAcionHist/{Id}")]
        [Authorize]
        public List<CashTransacionHist> GetCashTransAcionHist(Guid Id)
        {
            var cashTransactionHistroyCommentsBC = new CashTransactionHistoryCommentsBC(_logger, _repository);
            return cashTransactionHistroyCommentsBC.GetCashTransactionHist(Id);
        }

        [HttpGet]
        [Route("getCashTransacionAttachments/{Id}")]
        [Authorize]
        public List<AppDocument> GetCashTransacionAttachments(Guid Id)
        {
            var cashTransactionHistroyCommentsBC = new CashTransactionHistoryCommentsBC(_logger, _repository);
            return cashTransactionHistroyCommentsBC.GetCashTransacionAttachments(Id);
        }
        

       
       
    }
}