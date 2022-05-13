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
    public class PettyCashTransferHistCommentsController : AppApiBaseController
    {
        public PettyCashTransferHistCommentsController(ILogger logger, IRepository repository, IOwinContext context) : base(logger, repository, context)
        {
        }
        [HttpPost]
        [Route("savePettyCashTransHdrComment")]
        [Authorize]
        public List<PettyCashTransferComment> SavePettyCashTransHdrComment(PettyCashTransferComment histroy)
        {
            var pettyCashTransHistroyCommentsBC = new PettyCashTransferHistCommentsBC(_logger, _repository);
            return pettyCashTransHistroyCommentsBC.SavePettyCashTransHdrComment(histroy);
        }
        [HttpGet]
        [Route("getPettyCashTransComments/{Id}")]
        [Authorize]
        public List<PettyCashTransferComment> GetPettyCashTransComments(Guid Id)
        {
            var pettyCashTransCommentsBC = new PettyCashTransferHistCommentsBC(_logger, _repository);
            return pettyCashTransCommentsBC.GetPettyCashTransComments(Id);
        }

       
       
        [HttpGet]
        [Route("getpettycashtransHist/{Id}")]
        [Authorize]
        public List<PettyCashTransferHist> GetpettycashtransHist(Guid Id)
        {
            var budgetHistroyCommentsBC = new PettyCashTransferHistCommentsBC(_logger, _repository);
            return budgetHistroyCommentsBC.GetpettycashtransHist(Id);
        }

        [HttpGet]
        [Route("getPettyCashTransAttachments/{Id}")]
        [Authorize]
        public List<AppDocument> GetPettyCashTransAttachments(Guid Id)
        {
            var pettyCashTransCommentsBC = new PettyCashTransferHistCommentsBC(_logger, _repository);
            return pettyCashTransCommentsBC.GetPettyCashTransAttachments(Id);
        }
        

       
       
    }
}