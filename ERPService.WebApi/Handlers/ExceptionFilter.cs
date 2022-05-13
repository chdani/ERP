using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Filters;
using ERPService.BC;
using ERPService.Common;
using ERPService.Common.Shared;
using ERPService.DataModel.DTO;

namespace ERPService.WebApi
{
    public class ExceptionFilter : ExceptionFilterAttribute
    {
        private class ErrorResponse
        {
            public ErrorResponse(Exception ex)
            {
                Error = new ErrorDescription()
                {
                    Message = ex.Message,
                    InnerException = ex.InnerException != null ? ex.InnerException.Message : null
                };
            }

            public class ErrorDescription
            {
                public string Message { get; set; }
                public string InnerException { get; set; }
            }

            public ErrorDescription Error { get; set; }
        }

        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Exception is HttpResponseException)
            {
                actionExecutedContext.Response = (actionExecutedContext.Exception as HttpResponseException).Response;
                return;
            }

            if(actionExecutedContext.Exception is DbUpdateConcurrencyException)
            {
                var content = new AppResponse();
                content.Status =  APPMessageKey.ONEORMOREERR;
                content.Messages = new List<string>();
                content.Messages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.NOTLATEST]);
                actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(HttpStatusCode.OK, content);
                return;
            }
        

            var responseContent = new ErrorResponse(actionExecutedContext.Exception);
            var status = HttpStatusCode.InternalServerError;
            if (actionExecutedContext.Exception is KeyNotFoundException)
            {
                status = HttpStatusCode.NotFound;
            }

            ExceptionLog exLog = new ExceptionLog()
            {
                Id = new Guid(),
                ExceptionMessage = actionExecutedContext.Exception.Message,
                InnerException = actionExecutedContext.Exception.InnerException == null ?
                "" : actionExecutedContext.Exception.InnerException.ToString(),
                StackTrace = actionExecutedContext.Exception.StackTrace,
                ExceptionOccurredAt = DateTime.Now
            };

            ActivityLogBC activityLogBC = new ActivityLogBC();
            activityLogBC.InsertExceptionLog(exLog);

            actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(status, responseContent);
        }
    }
}