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
    public class DirInvPostPayHistCommentsController : AppApiBaseController
    {
        public DirInvPostPayHistCommentsController(ILogger logger, IRepository repository, IOwinContext context) : base(logger, repository, context)
        {
        }

        [HttpPost]
        [Route("saveDirInvPostPayComment")]
        [Authorize]
        public List<DirInvPostPayComment> saveDirInvPostPayComment(DirInvPostPayComment comment)
        {
            var dirInvPostPayHistCommentsBCBC = new DirInvPostPayHistCommentsBC(_logger, _repository);
            return dirInvPostPayHistCommentsBCBC.saveDirInvPostPayComment(comment);
        }
        [HttpGet]
        [Route("getDirInvPostPayComment/{Id}")]
        [Authorize]
        public List<DirInvPostPayComment> getDirInvPostPayComment(Guid Id)
        {
            var dirInvPostPayHistCommentsBCBC = new DirInvPostPayHistCommentsBC(_logger, _repository);
            return dirInvPostPayHistCommentsBCBC.getDirInvPostPayComment(Id);
        }

        [HttpGet]
        [Route("getDirInvPostPayHistroy/{Id}")]
        [Authorize]
        public List<DirInvPostPayHist> getDirInvPostPayHistroy(Guid Id)
        {
            var dirInvPostPayHistCommentsBCBC = new DirInvPostPayHistCommentsBC(_logger, _repository);
            return dirInvPostPayHistCommentsBCBC.getDirInvPostPayHistroy(Id);
        }

        [HttpGet]
        [Route("getDirInvPostPayStatusHist/{Id}")]
        [Authorize]
        public List<DirInvPostPayStatusHist> getDirInvPostPayStatusHist(Guid Id)
        {
            var dirInvPostPayHistCommentsBCBC = new DirInvPostPayHistCommentsBC(_logger, _repository);
            return dirInvPostPayHistCommentsBCBC.getDirInvPostPayStatusHist(Id);
        }
        [HttpGet]
        [Route("getDirInvPostPayAttachments/{Id}")]
        [Authorize]
        public List<AppDocument> getDirInvPostPayAttachments(Guid Id)
        {
            var dirInvPostPayHistCommentsBCBC = new DirInvPostPayHistCommentsBC(_logger, _repository);
            return dirInvPostPayHistCommentsBCBC.getDirInvPostPayAttachments(Id);
        }



    }
}




