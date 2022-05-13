using Serilog;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;

namespace ERPService.WebApi
{
    public class ExceptionLogger: IExceptionLogger
    {
        public Task LogAsync(ExceptionLoggerContext context, CancellationToken cancellationToken)
        {
            Log.Error(context.Exception, context.ExceptionContext.Request.RequestUri.ToString());
            return Task.CompletedTask;
        }
    }
}