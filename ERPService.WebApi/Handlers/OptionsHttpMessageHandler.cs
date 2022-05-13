using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ERPService.WebApi
{
    /// <summary>
    /// OPTIONS HTTP query handler
    /// </summary>
    /// <remarks>The code based on this answer https://stackoverflow.com/a/36454275/8127016</remarks>
    public class OptionsHttpMessageHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Method == HttpMethod.Options)
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Headers.Add("Access-Control-Allow-Methods", "*");
                return Task.FromResult(response);
            }
            return base.SendAsync(request, cancellationToken);
        }
    }
}
