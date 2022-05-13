using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using ERPService.BC;
using ERPService.Common.Interfaces;
using ERPService.DataModel.DTO;

namespace ERPService.WebApi.Handlers
{
    public class RequestResponseHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage>
        SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
        {
            var requestedMethod = request.Method;
            var userHostAddress = HttpContext.Current != null ?
                HttpContext.Current.Request.UserHostAddress : "0.0.0.0";
            var useragent = request.Headers.UserAgent.ToString();
            var requestMessage = await request.Content.ReadAsByteArrayAsync();
            var uriAccessed = request.RequestUri.AbsoluteUri;

            var responseHeadersString = new StringBuilder();
            foreach (var header in request.Headers)
            {

                responseHeadersString.Append(header.Key + ":");
                responseHeadersString.Append(String.Join(", ", header.Value));
                responseHeadersString.Append("~");
            }
            var RequestBody = Encoding.UTF8.GetString(requestMessage);

            var requestLog = new ActivityLog()
            {
                Headers = responseHeadersString.ToString(),
                AbsoluteURI = uriAccessed,
                Host = userHostAddress,
                RequestBody = Encoding.UTF8.GetString(requestMessage),
                UserHostAddress = userHostAddress,
                UserAgent = useragent,
                RequestMethod = requestedMethod.ToString(),
                RequestedOn = DateTime.Now
            };

            ActivityLogBC activityLogBC = new ActivityLogBC();
            activityLogBC.InsertActvityLog(requestLog);

            var response = await base.SendAsync(request, cancellationToken);
            return response;
        }
    }
}