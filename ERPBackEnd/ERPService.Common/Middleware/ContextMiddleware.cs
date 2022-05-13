using ERPService.Common.Shared;
using Microsoft.Owin;
using Serilog;
using System.Threading.Tasks;

namespace ERPService.Common.Middleware
{
    public class ContextMiddleware : OwinMiddleware
    {
        private readonly ILogger _logger;
        private UserContext _userContext;

        public ContextMiddleware(OwinMiddleware next, ILogger logger, UserContext userContext) : base(next)
        {
            _logger = logger;
            _userContext = userContext;
        }

        public override async Task Invoke(IOwinContext context)
        {
            if(context.Request.Headers.ContainsKey("Authorization"))
            {
                var token = context.Request.Headers["Authorization"].ToString();
                token = token.Replace("Bearer ", "");
                _userContext = JWTToken.ValidateToken(token);
                 
                context.Set("USRCTX", _userContext);
            }
            else
                _logger.Debug("Requester did not send a token!!");

            //var claimsIdentity = context.Request.User.Identity as System.Security.Claims.ClaimsIdentity;
            //var userData = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.UserData);

            //UserContext ctx = null;
            //if (userData != null)
            //{
            //    ctx = JsonConvert.DeserializeObject<UserContext>(userData.Value);
            //    string token = string.Empty;
            //    if (context.Request.Headers.ContainsKey("Authorization"))
            //    {
            //        token = context.Request.Headers["Authorization"].ToString();
            //        ctx.Token = token;
            //    }
            //    //userContext.CopyLocal(ctx);
            //    context.Set("USRCTX", ctx);
            //}
            //else
            //{
            //    _logger.Debug("Requester did not send a token!!");
            //}
            // Call the next delegate/middleware in the pipeline
            await Next.Invoke(context);
        }
    }
}
