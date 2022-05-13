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

namespace ERPService.WebApi.Controllers
{
    public class GoodsReceiptNotesController : AppApiBaseController
    {
        public GoodsReceiptNotesController(ILogger logger, IRepository repository, IOwinContext context) : base(logger, repository, context)
        {
        }

        [HttpGet]
        [Route("getGoodsRecNoteById/{goodsRecNoteId}")]
        [Authorize]
        public GoodsRecNote GetGoodsRecNoteById(Guid goodsRecNoteId)
        {
            GoodsReceiptNotesBC goodsRecNoteBC = new GoodsReceiptNotesBC(_logger, _repository, _userContext);
            return goodsRecNoteBC.GetGoodsRecNoteById(goodsRecNoteId);
        }

        [HttpGet]
        [Route("getGoodsRecNoteDetByHdrId/{goodsRecNoteId}")]
        [Authorize]
        public List<GoodsRecNoteDet> GetGoodsRecNoteDetByHdrId(Guid goodsRecNoteId)
        {
            GoodsReceiptNotesBC goodsRecNoteBC = new GoodsReceiptNotesBC(_logger, _repository, _userContext);
            return goodsRecNoteBC.GetGoodsRecNoteDetByHdrId(goodsRecNoteId);
        }


        [HttpPost]
        [Route("saveGoodsRecNote")]
        [Authorize]
        public AppResponse SaveGoodsRecNote(GoodsRecNote goodsRecNote)
        {
            GoodsReceiptNotesBC goodsRecNoteBC = new GoodsReceiptNotesBC(_logger, _repository, _userContext);
            return goodsRecNoteBC.SaveGoodsRecNote(goodsRecNote);
        }

        [HttpPost]
        [Route("saveGoodsRecNoteComment")]
        [Authorize]
        public AppResponse SaveGoodsRecNoteComment(GoodsRecNoteComment goodsRecNote)
        {
            GoodsReceiptNotesBC goodsRecNoteBC = new GoodsReceiptNotesBC(_logger, _repository, _userContext);
            return goodsRecNoteBC.SaveGoodsRecNoteComment(goodsRecNote);
        }

        [HttpPost]
        [Route("saveGoodsRecNoteDetComment")]
        [Authorize]
        public AppResponse SaveGoodsRecNoteDetComment(GoodsRecNoteDetComment goodsRecNote)
        {
            GoodsReceiptNotesBC goodsRecNoteBC = new GoodsReceiptNotesBC(_logger, _repository, _userContext);
            return goodsRecNoteBC.SaveGoodsRecNoteDetComment(goodsRecNote);
        }

        [HttpPost]
        [Route("approveGoodsRecNote")]
        [Authorize]
        public AppResponse ApproveGoodsRecNote(GoodsRecNote goodsRecNote)
        {
            GoodsReceiptNotesBC goodsRecNoteBC = new GoodsReceiptNotesBC(_logger, _repository, _userContext);
            return goodsRecNoteBC.ApproveGoodsRecNote(goodsRecNote);
        }


        [HttpPost]
        [Route("getGoodsRecNoteList")]
        [Authorize]
        public List<GoodsRecNote> GetGoodsRecNoteList(GoodsRecNoteSearch input, bool isExport = false)
        {
            GoodsReceiptNotesBC goodsRecNoteBC = new GoodsReceiptNotesBC(_logger, _repository, _userContext);
            return goodsRecNoteBC.GetGoodsRecNotes(input, isExport);
        }

        [HttpGet]
        [Route("getGoodsRecNoteHistory/{goodsRecNoteId}")]
        [Authorize]
        public List<GoodsRecNoteHist> GetGoodsRecNoteHistory(Guid goodsRecNoteId)
        {
            GoodsReceiptNotesBC goodsRecNoteBC = new GoodsReceiptNotesBC(_logger, _repository, _userContext);
            return goodsRecNoteBC.GetGoodsRecNoteHistory(goodsRecNoteId);
        }

        [HttpGet]
        [Route("getGoodsRecNoteDetHistory/{goodsRecNoteDetId}")]
        [Authorize]
        public List<GoodsRecNoteDetHist> GetGoodsRecNoteDetHistory(Guid goodsRecNoteDetId)
        {
            GoodsReceiptNotesBC goodsRecNoteBC = new GoodsReceiptNotesBC(_logger, _repository, _userContext);
            return goodsRecNoteBC.GetGoodsRecNoteDetHistory(goodsRecNoteDetId);
        }

        [HttpGet]
        [Route("getGoodsRecNoteComment/{goodsRecNoteId}")]
        [Authorize]
        public List<GoodsRecNoteComment> GetGoodsRecNoteComment(Guid goodsRecNoteId)
        {
            GoodsReceiptNotesBC goodsRecNoteBC = new GoodsReceiptNotesBC(_logger, _repository, _userContext);
            return goodsRecNoteBC.GetGoodsRecNoteComment(goodsRecNoteId);
        }

        [HttpGet]
        [Route("getGoodsRecNoteDetComment/{goodsRecNoteDetId}")]
        [Authorize]
        public List<GoodsRecNoteDetComment> GetGoodsRecNoteDetComment(Guid goodsRecNoteDetId)
        {
            GoodsReceiptNotesBC goodsRecNoteBC = new GoodsReceiptNotesBC(_logger, _repository, _userContext);
            return goodsRecNoteBC.GetGoodsRecNoteDetComment(goodsRecNoteDetId);
        }

        [HttpGet]
        [Route("getGoodsRecNoteStatusHistory/{goodsRecNoteId}")]
        [Authorize]
        public List<GoodsRecNoteStatusHist> GetGoodsRecNoteStatusHistory(Guid goodsRecNoteId)
        {
            GoodsReceiptNotesBC goodsRecNoteBC = new GoodsReceiptNotesBC(_logger, _repository, _userContext);
            return goodsRecNoteBC.GetGoodsRecNoteStatusHistory(goodsRecNoteId);
        }

        [HttpGet]
        [Route("getGoodsRecNotesAttachments/{goodsRecNoteId}")]
        [Authorize]
        public List<AppDocument> GetGoodsRecNotesAttachments(Guid goodsRecNoteId)
        {
            GoodsReceiptNotesBC goodsRecNoteBC = new GoodsReceiptNotesBC(_logger, _repository, _userContext);
            return goodsRecNoteBC.GetGoodsRecNotesAttachments(goodsRecNoteId);
        }
        [HttpPost]
        [Route("downloadGrn")]
        [Authorize]
        public HttpResponseMessage DownloadGrn(GoodsRecNoteSearch search)
        {
            HttpResponseMessage httpResponse = Request.CreateResponse(System.Net.HttpStatusCode.OK);
            GetFileResponse(search, httpResponse);
            return httpResponse;
        }
        private void GetFileResponse(GoodsRecNoteSearch search, HttpResponseMessage httpResponse)
        {
            List<GoodsRecNote> transfers = GetGoodsRecNoteList(search, true);
            byte[] buff = null;
            if (transfers.Count > 0)
            {
                if (search.ExportType == ExportType.PDF)
                {
                    var file = new BudgetAllocationPDFGenerator<GoodsRecNote>
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
                    var file = new BudgetAllocationExcelGeneratore<GoodsRecNote>
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
        [HttpGet]
        [Route("sendGrnSendMail/{id}")]
        public AppResponse SendGrnSendMail(Guid id)
        {
            GoodsReceiptNotesBC goodsRecNoteBC = new GoodsReceiptNotesBC(_logger, _repository, _userContext);
            return goodsRecNoteBC.SendGrnSendMail(id);
        }
    }
}