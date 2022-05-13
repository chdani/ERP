using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using ERPService.BC;
using ERPService.Common.Interfaces;
using ERPService.DataModel.DTO;
using Newtonsoft.Json;
using Serilog;

namespace ERPService.WebApi
{
    public class ActivityLogActionFilter : ActionFilterAttribute
    {
        private ILogger _logger { get; }
        private IRepository _repository { get; }

        string activityType = "";
        public ActivityLogActionFilter(string ActivityType)
        {
            //_logger = logger ?? throw new ArgumentNullException(nameof(logger));
            //_repository = repository;
            activityType = ActivityType;
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            ////Fill Log Details
            //ActivityLog activityLog = new ActivityLog();
            //activityLog.ControllerName = actionContext.ControllerContext.ControllerDescriptor.ControllerName;
            //activityLog.ActionName = actionContext.ActionDescriptor.ActionName;
            //activityLog.RequestURL = actionContext.Request.RequestUri.ToString();
            //var args = actionContext.ActionArguments;
            //activityLog.Parameters = JsonConvert.SerializeObject(args);
            //activityLog.ActivityType = new Guid();

            //var handler = new ActivityFilterHandler();
            //var activityLogBC = new ActivityLogBC(_logger, _repository);
            //activityLogBC.InsertActvity(activityLog);
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            //Igored;
        }
    }
}