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
    public class BudgetHistoryCommentsController : AppApiBaseController
    {
        public BudgetHistoryCommentsController(ILogger logger, IRepository repository, IOwinContext context) : base(logger, repository, context)
        {
        }
        [HttpPost]
        [Route("saveBudgtAllocHdrComment")]
        [Authorize]
        public List<BudgAllocHdrComment> SaveBudgtAllocHdrComment(BudgAllocHdrComment histroy)
        {
            var budgetHistroyCommentsBC = new BudgetHistroyCommentsBC(_logger, _repository);
            return budgetHistroyCommentsBC.SaveBudgtAllocHdrComment(histroy);
        }
        [HttpGet]
        [Route("getBudgtAllocHdrComments/{Id}")]
        [Authorize]
        public List<BudgAllocHdrComment> GetBudgtAllocHdrComments(Guid Id)
        {
            var budgetHistroyCommentsBC = new BudgetHistroyCommentsBC(_logger, _repository);
            return budgetHistroyCommentsBC.GetBudgtAllocHdrComments(Id);
        }

        [HttpPost]
        [Route("SaveStatusHistory")]
        [Authorize]
        public bool SaveStatusHistory(BudgAllocHdrStatusHist histroy)
        {
            var budgetHistroyCommentsBC = new BudgetHistroyCommentsBC(_logger, _repository);
            budgetHistroyCommentsBC.SaveStatusHistory(histroy);
            return true;
        }
        [HttpGet]
        [Route("getHistoryStatus/{Id}")]
        [Authorize]
        public List<BudgAllocHdrStatusHist> getHistoryStatus(Guid Id)
        {
            var budgetHistroyCommentsBC = new BudgetHistroyCommentsBC(_logger, _repository);
            return budgetHistroyCommentsBC.getHistoryStatus(Id);
        }

        [HttpPost]
        [Route("SaveBudgAllocHdrHist")]
        [Authorize]
        public  bool SaveBudgAllocHdrHist(BudgAllocHdrHist histroy)
        {
            var budgetHistroyCommentsBC = new BudgetHistroyCommentsBC(_logger, _repository);
            var histroies = new List<BudgAllocHdrHist>();
            histroies.Add(histroy);
            budgetHistroyCommentsBC.SaveBudgAllocHdrHist(histroies, true);
            return true;
        }
        [HttpGet]
        [Route("getBudgAlocHdrHist/{Id}")]
        [Authorize]
        public List<BudgAllocHdrHist> getBudgAlocHdrHist(Guid Id)
        {
            var budgetHistroyCommentsBC = new BudgetHistroyCommentsBC(_logger, _repository);
            return budgetHistroyCommentsBC.GetBudgAlocHdrHist(Id);
        }

        [HttpGet]
        [Route("getBudgAlocHdrAttachments/{Id}")]
        [Authorize]
        public List<AppDocument> GetBudgAlocHdrAttachments(Guid Id)
        {
            var budgetHistroyCommentsBC = new BudgetHistroyCommentsBC(_logger, _repository);
            return budgetHistroyCommentsBC.GetBudgAlocHdrAttachments(Id);
        }
        

        [HttpPost]
        [Route("SaveBudgAllocDetHist")]
        [Authorize]
        public List<BudgAllocDetHist> SaveBudgAllocDetHist(BudgAllocDetHist histroy)
        {
            var budgetHistroyCommentsBC = new BudgetHistroyCommentsBC(_logger, _repository);
            return budgetHistroyCommentsBC.SaveBudgAllocDetHist(histroy);
        }
        [HttpGet]
        [Route("getBudgAloocDetHist/{Id}")]
        [Authorize]
        public List<BudgAllocDetHist> getBudgAlocDetHist(Guid Id)
        {
            var budgetHistroyCommentsBC = new BudgetHistroyCommentsBC(_logger, _repository);
            return budgetHistroyCommentsBC.getBudgAlocDetHist(Id);
        }
    }
}