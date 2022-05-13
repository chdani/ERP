using ERPService.BC;
using ERPService.BC.Utility;
using ERPService.Common;
using ERPService.Common.Interfaces;
using ERPService.Common.Shared;
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
    public class BudgetAllocationController : AppApiBaseController
    {
        public BudgetAllocationController(ILogger logger, IRepository repository, IOwinContext context) : base(logger, repository, context)
        {
        }
        [HttpGet]
        [Route("getBudgetAlloctionById/{allocHdrId}")]
        [Authorize]
        public BudgAllocHdr GetBudgetAlloctionById(Guid allocHdrId)
        {
            var budgetAllocBC = new BudgetAllocationBC(_logger,_repository);
            return budgetAllocBC.GetBudgetAlloctionById(allocHdrId);
        }

        [HttpGet]
        [Route("getBudgetAllocDetByHdrId/{allocHdrId}")]
        [Authorize]
        public List<BudgAllocDet> GetBudgetAllocDetByHdrId(Guid allocHdrId)
        {
            var budgetAllocBC = new BudgetAllocationBC(_logger, _repository);
            return budgetAllocBC.GetBudgetAllocDetByHdrId(allocHdrId);
        }

        [HttpPost]
        [Route("getBudgetAlloctions")]
        [Authorize]
        public List<BudgAllocHdr> GetBudgetAlloctions(BudgAllocHdr allocHdr)
        {
            var budgetAllocBC = new BudgetAllocationBC(_logger,_repository);
            return budgetAllocBC.GetBudgetAlloctions(allocHdr);
        }

        [HttpPost]
        [Route("saveBudgetAllocation")]
        [Authorize]
        public AppResponse SaveBudgetAllocation(BudgAllocHdr allocHdr)
        {
            var budgetAllocBC = new BudgetAllocationBC(_logger, _repository);
            return budgetAllocBC.SaveBudgetAllocation(allocHdr);
        }

        [HttpPost]
        [Route("approveReturnBudgtAlloc")]
        [Authorize]
        public AppResponse ApproveReturnBudgtAlloc(BudgAllocHdr allocHdr)
        {
            var budgetAllocBC = new BudgetAllocationBC(_logger, _repository);
            allocHdr.ApprovedBy = _userContext.Id;
            allocHdr.ApprovedDate = DateTime.Now;

            return budgetAllocBC.SaveBudgetAllocation(allocHdr);
        }
        [HttpPost]
        [Route("getBudgetAllocationSearch")]
        [Authorize]
        public List<BudgAllocHdr> getBudgetAllocationSearch(BudgAllocHdr search, bool isExport = false)
        {
            var budgetAllocBC = new BudgetAllocationBC(_logger, _repository);
            return budgetAllocBC.getBudgetAllocationSearch(search, isExport);
        }
        [HttpPost]
        [Route("downloadBudgetAllocation")]
        [Authorize]
        public HttpResponseMessage downloadBudgetAllocation(BudgAllocHdr search)
        {
            HttpResponseMessage httpResponse = Request.CreateResponse(System.Net.HttpStatusCode.OK);
            GetFileResponse(search, httpResponse);
            return httpResponse;
        }
        private void GetFileResponse(BudgAllocHdr search, HttpResponseMessage httpResponse)
        {
            List<BudgAllocHdr> transfers = getBudgetAllocationSearch(search, true);
            byte[] buff = null;
            if (transfers.Count > 0)
            {
                if (search.ExportType == ExportType.PDF)
                {
                    var file = new BudgetAllocationPDFGenerator<BudgAllocHdr>
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
                    var file = new BudgetAllocationExcelGeneratore<BudgAllocHdr>
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