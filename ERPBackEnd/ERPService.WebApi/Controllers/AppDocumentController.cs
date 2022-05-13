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
    public class AppDocumentController : ApiController
    {
        private ILogger _logger { get; }
        private IRepository _repository { get; }

        public AppDocumentController(ILogger logger, IRepository repository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository;
        }

        [HttpPost]
        [Route("saveAppDocument")]
        [Authorize]
        public AppResponse saveAppDocument(List<AppDocument> allocHdr)
        {
            var AppDocumentBC = new AppDocumentBC(_logger, _repository);
            return AppDocumentBC.saveAppDocument(allocHdr, true);
        }

        [HttpPost]
        [Route("saveAppDocumentNew")]
        [Authorize]
        public AppResponse saveAppDocumentNew(List<AppDocument> allocHdr)
        {
            var AppDocumentBC = new AppDocumentBC(_logger, _repository);
            return AppDocumentBC.saveAppDocumentNew(allocHdr);
        }

        [HttpGet]
        [Route("getFileAttachment/{Id}")]
        [Authorize]
        public List<AppDocument> getFileAttachment(Guid Id)
        {
            var AppDocumentBC = new AppDocumentBC(_logger, _repository);
            return AppDocumentBC.getFileAttachment(Id);
        }
        [HttpGet]
        [Route("getFileDowenload/{Id}")]
        [Authorize]
        public AppDocument getFileDowenload(Guid id)
        {
            var AppDocumentBC = new AppDocumentBC(_logger, _repository);
            return AppDocumentBC.getFileDowenload(id);
        }
    }
}