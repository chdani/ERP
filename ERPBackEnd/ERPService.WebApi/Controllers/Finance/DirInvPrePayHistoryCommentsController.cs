using ERPService.BC;
using ERPService.Common.Interfaces;
using ERPService.Common.Shared;
using ERPService.DataModel.DTO;
using Serilog;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace ERPService.WebApi
{
    [Route("api/[controller]")]
    [Authorize]
    public class DirInvPrePayStatusHistoryCommentsController : ApiController
    {
        private ILogger _logger { get; }
        private IRepository _repository { get; }

        public DirInvPrePayStatusHistoryCommentsController(ILogger logger, IRepository repository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository;
        }

        //DirInvPrePayStatusHistFunction(GetById,Save,Inactive)
        [HttpGet]
        [Route("getDirInvPrePayStatusHistListById/{Id}")]
        [Authorize]
        public List<DirInvPrePayStatusHist> GetDirInvPrePayStatusHistListById(Guid Id)
        {
            var dirInvPrePayStatusHistBC = new DirInvPrePayHistoryCommentsBC(_logger, _repository);
            return dirInvPrePayStatusHistBC.GetDirInvPrePayStatusHistListById(Id);
        }


        //DirInvPrePayHistFunction(GetById,Save,Inactive)
        [HttpGet]
        [Route("getDirInvPrePayHistListById/{directInvPrePaymentId}")]
        [Authorize]
        public List<DirInvPrePayHist> GetDirInvPrePayHistListById(Guid directInvPrePaymentId)
        {
            var dirInvPrePayHistBC = new DirInvPrePayHistoryCommentsBC(_logger, _repository);
            return dirInvPrePayHistBC.GetDirInvPrePayHistListById(directInvPrePaymentId);
        }

        //DirInvPrePayCommentFunction(GetById,Save,Inactive)
        [HttpGet]
        [Route("getDirInvPrePayCommentListById/{directInvPrePaymentId}")]
        [Authorize]
        public List<DirInvPrePayComment> GetDirInvPrePayCommentListById(Guid directInvPrePaymentId)
        {
            var dirInvPrePayCommentBC = new DirInvPrePayHistoryCommentsBC(_logger, _repository);
            return dirInvPrePayCommentBC.GetDirInvPrePayCommentListById(directInvPrePaymentId);
        }
        [HttpGet]
        [Route("getDirInvPrePayAttachmentsById/{Id}")]
        [Authorize]
        public List<AppDocument> GetDirInvPrePayAttachmentsById(Guid Id)
        {
            var dirInvPrePayHistroyCommentsBC = new DirInvPrePayHistoryCommentsBC(_logger, _repository);
            return dirInvPrePayHistroyCommentsBC.GetDirInvPrePayAttachmentsById(Id);
        }

        [HttpPost]
        [Route("saveDirInvPrePayComment")]
        [Authorize]
        public List<DirInvPrePayComment> SaveDirInvPrePayComment(DirInvPrePayComment dirInvPrePayComment)
        {
            var dirInvPrePayCommentBC = new DirInvPrePayHistoryCommentsBC(_logger, _repository);
            return dirInvPrePayCommentBC.SaveDirInvPrePayComment(dirInvPrePayComment);
        }
    }
}