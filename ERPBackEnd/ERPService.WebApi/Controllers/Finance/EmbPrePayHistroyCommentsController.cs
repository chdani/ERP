using ERPService.BC;
using ERPService.BC.Finance;
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
   public class EmbPrePayHistroyCommentsController : AppApiBaseController
   {
        public EmbPrePayHistroyCommentsController(ILogger logger, IRepository repository, IOwinContext context) : base(logger, repository, context)
        {
        }
        [HttpPost]
        [Route("saveEmbPrePaymentHdrHistComment")]
        [Authorize]
        public List<EmbPrePaymentHdrComment> SaveEmbPrePaymentHdrHistComment(EmbPrePaymentHdrComment histroy)
        {
            var embPrePayHistroyComentsBC = new EmbPrePayHistroyComentsBC(_logger, _repository);
            return embPrePayHistroyComentsBC.SaveEmbPrePaymentHdrHistComment(histroy);
        }
        [HttpGet]
        [Route("getEmbPrePaymentHdrHistComment/{Id}")]
        [Authorize]
        public List<EmbPrePaymentHdrComment> GetEmbPrePaymentHdrHistComment(Guid Id)
        {
            var embPrePayHistroyComentsBC = new EmbPrePayHistroyComentsBC(_logger, _repository);
            return embPrePayHistroyComentsBC.GetEmbPrePaymentHdrHistComment(Id);
        }
        [HttpGet]
        [Route("getEmbPrePaymentHistoryStatus/{Id}")]
        [Authorize]
        public List<EmbPrePaymentHdrStatusHist> GetEmbPrePaymentHistoryStatus(Guid Id)
        {
            var embPrePayHistroyComentsBC = new EmbPrePayHistroyComentsBC(_logger, _repository);
            return embPrePayHistroyComentsBC.GetEmbPrePaymentHistoryStatus(Id);
        }
        [HttpGet]
        [Route("getEmbPrePaymentHistory/{Id}")]
        [Authorize]
        public List<EmbPrePaymentHdrHist> GetEmbPrePaymentHistory(Guid Id)
        {
            var embPrePayHistroyComentsBC = new EmbPrePayHistroyComentsBC(_logger, _repository);
            return embPrePayHistroyComentsBC.GetEmbPrePaymentHistory(Id);
        }


        [HttpGet]
        [Route("getEmbPrePaymentAttachments/{Id}")]
        [Authorize]
        public List<AppDocument> GetEmbPrePaymentAttachments(Guid Id)
        {
            var embPrePayHistroyComentsBC = new EmbPrePayHistroyComentsBC(_logger, _repository);
            return embPrePayHistroyComentsBC.GetEmbPrePaymentAttachments(Id);
        }
        /////////////////////////////////////////////////////////////////////////

        [HttpPost]
        [Route("saveEmbPrePaymentDetHistComment")]
        [Authorize]
        public List<EmbPrePaymentEmbDetComment> SaveEmbPrePaymentDetHistComment(EmbPrePaymentEmbDetComment histroy)
        {
            var embPrePayHistroyComentsBC = new EmbPrePayHistroyComentsBC(_logger, _repository);
            return embPrePayHistroyComentsBC.SaveEmbPrePaymentDetHistComment(histroy);
        }
        [HttpGet]
        [Route("getEmbPrePaymentDetHistComment/{Id}")]
        [Authorize]
        public List<EmbPrePaymentEmbDetComment> GetEmbPrePaymentDetHistComment(Guid Id)
        {
            var embPrePayHistroyComentsBC = new EmbPrePayHistroyComentsBC(_logger, _repository);
            return embPrePayHistroyComentsBC.GetEmbPrePaymentDetHistComment(Id);
        }
       
        [HttpGet]
        [Route("getEmbPrePaymentDetHistory/{Id}")]
        [Authorize]
        public List<EmbPrePaymentEmbDetHist> GetEmbPrePaymentDetHistory(Guid Id)
        {
            var embPrePayHistroyComentsBC = new EmbPrePayHistroyComentsBC(_logger, _repository);
            return embPrePayHistroyComentsBC.GetEmbPrePaymentDetHistory(Id);
        }

        [HttpGet]
        [Route("getEmbPrePaymentdetAttachments/{Id}")]
        [Authorize]
        public List<AppDocument> GetEmbPrePaymentdetAttachments(Guid Id)
        {
            var embPrePayHistroyComentsBC = new EmbPrePayHistroyComentsBC(_logger, _repository);
            return embPrePayHistroyComentsBC.GetEmbPrePaymentAttachments(Id);
        }
        ////////////////////////////////////////////////////////////////////
        ///
        [HttpPost]
        [Route("saveEmbPreInvPaymentDetHistComment")]
        [Authorize]
        public List<EmbPrePaymentInvDetComment> SaveEmbPreInvPaymentDetHistComment(EmbPrePaymentInvDetComment histroy)
        {
            var embPrePayHistroyComentsBC = new EmbPrePayHistroyComentsBC(_logger, _repository);
            return embPrePayHistroyComentsBC.SaveEmbPreInvPaymentDetHistComment(histroy);
        }
        [HttpGet]
        [Route("getEmbPrePaymentInvDetHistComment/{Id}")]
        [Authorize]
        public List<EmbPrePaymentInvDetComment> GetEmbPrePaymentInvDetHistComment(Guid Id)
        {
            var embPrePayHistroyComentsBC = new EmbPrePayHistroyComentsBC(_logger, _repository);
            return embPrePayHistroyComentsBC.GetEmbPrePaymentInvDetHistComment(Id);
        }

        [HttpGet]
        [Route("getEmbPrePaymentInvDetHistory/{Id}")]
        [Authorize]
        public List<EmbPrePaymentInvDetHist> GetEmbPrePaymentInvDetHistory(Guid Id)
        {
            var embPrePayHistroyComentsBC = new EmbPrePayHistroyComentsBC(_logger, _repository);
            return embPrePayHistroyComentsBC.GetEmbPrePaymentInvDetHistory(Id);
        }

        [HttpGet]
        [Route("GetEmbPrePaymentInvdetAttachments/{Id}")]
        [Authorize]
        public List<AppDocument> GetEmbPrePaymentInvdetAttachments(Guid Id)
        {
            var embPrePayHistroyComentsBC = new EmbPrePayHistroyComentsBC(_logger, _repository);
            return embPrePayHistroyComentsBC.GetEmbPrePaymentInvdetAttachments(Id);
        }
    }
}