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
    public class BudgetDetHistoryCommentsController : AppApiBaseController
    {
        public BudgetDetHistoryCommentsController(ILogger logger, IRepository repository, IOwinContext context) : base(logger, repository, context)
        {
        }
        [HttpPost]
        [Route("saveBudgtAllocDetComment")]
        [Authorize]
        public List<BudgAllocDetComment> SaveBudgtAllocDetComment(BudgAllocDetComment histroy)
        {
            var budgetDetHistroyCommentsBC = new BudgetDetHistoryCommentsBC(_logger, _repository);
            return budgetDetHistroyCommentsBC.SaveBudgtAllocDetComment(histroy);
        }
        [HttpGet]
        [Route("getBudgtAllocDetComments/{Id}")]
        [Authorize]
        public List<BudgAllocDetComment> GetBudgtAllocDetComments(Guid Id)
        {
            var budgetDetHistroyCommentsBC = new BudgetDetHistoryCommentsBC(_logger, _repository);
            return budgetDetHistroyCommentsBC.GetBudgtAllocDetComments(Id);
        }

        [HttpPost]
        [Route("SaveBudgAllocHdrHist")]
        [Authorize]
        public bool SaveBudgAllocHdrHist(BudgAllocDetHist histroy)
        {
            var budgetHistroyCommentsBC = new BudgetHistroyCommentsBC(_logger, _repository);
            budgetHistroyCommentsBC.SaveBudgAllocDetHist(histroy);
            return true;
        }
        [HttpGet]
        [Route("getBudgAlocDetHist/{Id}")]
        [Authorize]
        public List<BudgAllocDetHist> GetBudgAlocDetHist(Guid Id)
        {
            var budgetHistroyCommentsBC = new BudgetHistroyCommentsBC(_logger, _repository);
            return budgetHistroyCommentsBC.getBudgAlocDetHist(Id);
        }

        [HttpGet]
        [Route("getBudgAlocDetAttachments/{Id}")]
        [Authorize]
        public List<AppDocument> GetBudgAlocDetAttachments(Guid Id)
        {
            var budgetDetHistroyCommentsBC = new BudgetDetHistoryCommentsBC(_logger, _repository);
            return budgetDetHistroyCommentsBC.GetBudgAlocDetAttachments(Id);
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