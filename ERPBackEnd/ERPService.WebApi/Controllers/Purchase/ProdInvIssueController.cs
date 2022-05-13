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
    public class ProdInvIssueController : AppApiBaseController
    {
        public ProdInvIssueController(ILogger logger, IRepository repository, IOwinContext context) : base(logger, repository, context)
        {
        }

        [HttpGet]
        [Route("getProdInvIssueById/{prodInvIssueId}")]
        [Authorize]
        public ProdInvIssue GetProdInvIssueById(Guid prodInvIssueId)
        {
            ProdInvIssueBC prodInvIssueBC = new ProdInvIssueBC(_logger, _repository, _userContext);
            return prodInvIssueBC.GetProdInvIssueById(prodInvIssueId);
        }

        [HttpGet]
        [Route("getProdInvIssueDetByHdrId/{prodInvIssueId}")]
        [Authorize]
        public List<ProdInvIssueDet> GetProdInvIssueDetByHdrId(Guid prodInvIssueId)
        {
            ProdInvIssueBC prodInvIssueBC = new ProdInvIssueBC(_logger, _repository, _userContext);
            return prodInvIssueBC.GetProdInvIssueDetByHdrId(prodInvIssueId);
        }

        [HttpPost]
        [Route("saveProdInvIssue")]
        [Authorize]
        public AppResponse SaveProdInvIssue(ProdInvIssue prodInvIssue)
        {
            ProdInvIssueBC prodInvIssueBC = new ProdInvIssueBC(_logger, _repository, _userContext);
            return prodInvIssueBC.SaveProdInvIssue(prodInvIssue);
        }

        [HttpPost]
        [Route("approveProdInvIssue")]
        [Authorize]
        public AppResponse ApproveProdInvIssue(ProdInvIssue prodInvIssue)
        {
            ProdInvIssueBC prodInvIssueBC = new ProdInvIssueBC(_logger, _repository, _userContext);
            return prodInvIssueBC.ApproveProdInvIssue(prodInvIssue);
        }

        [HttpPost]
        [Route("saveProdInvIssueComment")]
        [Authorize]
        public AppResponse SaveProdInvIssueComment(ProdInvIssueComment prodInvIssue)
        {
            ProdInvIssueBC prodInvIssueBC = new ProdInvIssueBC(_logger, _repository, _userContext);
            return prodInvIssueBC.SaveProdInvIssueComment(prodInvIssue);
        }

        [HttpPost]
        [Route("saveProdInvIssueDetComment")]
        [Authorize]
        public AppResponse SaveProdInvIssueDetComment(ProdInvIssueDetComment prodInvIssue)
        {
            ProdInvIssueBC prodInvIssueBC = new ProdInvIssueBC(_logger, _repository, _userContext);
            return prodInvIssueBC.SaveProdInvIssueDetComment(prodInvIssue);
        }

        [HttpPost]
        [Route("getProdInvIssueList")]
        [Authorize]
        public List<ProdInvIssue> GetProdInvIssueList(ProdInvIssueSearch input, bool isExport = false)
        {
            ProdInvIssueBC prodInvIssueBC = new ProdInvIssueBC(_logger, _repository, _userContext);
            return prodInvIssueBC.GetProdInvIssues(input, isExport);
        }

        [HttpGet]
        [Route("getProdInvIssueHistory/{prodInvIssueId}")]
        [Authorize]
        public List<ProdInvIssueHist> GetProdInvIssueHistory(Guid prodInvIssueId)
        {
            ProdInvIssueBC prodInvIssueBC = new ProdInvIssueBC(_logger, _repository, _userContext);
            return prodInvIssueBC.GetProdInvIssueHistory(prodInvIssueId);
        }

        [HttpGet]
        [Route("getProdInvIssueDetHistory/{prodInvIssueDetId}")]
        [Authorize]
        public List<ProdInvIssueDetHist> GetProdInvIssueDetHistory(Guid prodInvIssueDetId)
        {
            ProdInvIssueBC prodInvIssueBC = new ProdInvIssueBC(_logger, _repository, _userContext);
            return prodInvIssueBC.GetProdInvIssueDetHistory(prodInvIssueDetId);
        }

        [HttpGet]
        [Route("getProdInvIssueComment/{prodInvIssueId}")]
        [Authorize]
        public List<ProdInvIssueComment> GetProdInvIssueComment(Guid prodInvIssueId)
        {
            ProdInvIssueBC prodInvIssueBC = new ProdInvIssueBC(_logger, _repository, _userContext);
            return prodInvIssueBC.GetProdInvIssueComment(prodInvIssueId);
        }

        [HttpGet]
        [Route("getProdInvIssueDetComment/{prodInvIssueDetId}")]
        [Authorize]
        public List<ProdInvIssueDetComment> GetProdInvIssueDetComment(Guid prodInvIssueDetId)
        {
            ProdInvIssueBC prodInvIssueBC = new ProdInvIssueBC(_logger, _repository, _userContext);
            return prodInvIssueBC.GetProdInvIssueDetComment(prodInvIssueDetId);
        }

        [HttpGet]
        [Route("getProdInvIssueStatusHistory/{prodInvIssueId}")]
        [Authorize]
        public List<ProdInvIssueStatusHist> GetProdInvIssueStatusHistory(Guid prodInvIssueId)
        {
            ProdInvIssueBC prodInvBC = new ProdInvIssueBC(_logger, _repository, _userContext);
            return prodInvBC.GetProdInvIssueStatusHistory(prodInvIssueId);

        }

        [HttpGet]
        [Route("getProdInvIssuesAttachments/{prodInvIssueId}")]
        [Authorize]
        public List<AppDocument> GetProdInvIssuesAttachments(Guid prodInvIssueId)
        {
            ProdInvIssueBC prodInvIssueBC = new ProdInvIssueBC(_logger, _repository, _userContext);
            return prodInvIssueBC.GetProdInvIssuesAttachments(prodInvIssueId);
        }
        [HttpPost]
        [Route("downloadInvIssue")]
        [Authorize]
        public HttpResponseMessage DownloadInvIssue(ProdInvIssueSearch search)
        {
            HttpResponseMessage httpResponse = Request.CreateResponse(System.Net.HttpStatusCode.OK);
            GetFileResponse(search, httpResponse);
            return httpResponse;
        }
        private void GetFileResponse(ProdInvIssueSearch search, HttpResponseMessage httpResponse)
        {
            List<ProdInvIssue> transfers = GetProdInvIssueList(search, true);
            byte[] buff = null;
            if (transfers.Count > 0)
            {
                if (search.ExportType == ExportType.PDF)
                {
                    var file = new BudgetAllocationPDFGenerator<ProdInvIssue>
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
                    var file = new BudgetAllocationExcelGeneratore<ProdInvIssue>
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