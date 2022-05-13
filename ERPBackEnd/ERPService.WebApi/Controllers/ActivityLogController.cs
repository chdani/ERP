using ERPService.BC;
using ERPService.Common.Interfaces;
using ERPService.Common.Shared;
using ERPService.DataModel.CTO;
using ERPService.DataModel.DTO;
using Serilog;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace ERPService.WebApi
{
    [Route("api/[controller]")]
    //[Authorize]
    public class ActivityLogController : ApiController
    {
        private IRepository _repository { get; }

        private UserContext _userContext;

        public ActivityLogController(IRepository repository, UserContext userContext) //: base(logger, repository, userContext)
        {
            _repository = repository;
            _userContext = userContext;
        }

        [HttpPost]
        [Route("getActivityLog")]
        //[Authorize]
        public ActivityLogRes GetActivityLog(ActivityLogVM activityLog)
        {
            ActivityLogBC activityLogBC = new ActivityLogBC();
            return activityLogBC.GetActvityLog(activityLog);
        }

        [HttpPost]
        [Route("getExceptionLog")]
        //[Authorize]
        public ExceptionLogRes GetExceptionLog(ActivityLogVM activityLog)
        {
            ActivityLogBC activityLogBC = new ActivityLogBC();
            return activityLogBC.GetExceptionLog(activityLog);
        }
    }
}