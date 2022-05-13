using System.IO;
using System.Web.Http;
using ERPService.Common;
using ERPService.DataModel.CTO;
using ERPService.WebApi.Handlers;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;

[assembly: OwinStartup(typeof(ERPService.WebApi.Startup))]

namespace ERPService.WebApi
{
    // More information about https://github.com/drwatson1/AspNet-WebApi/wiki
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Use DotNetEnv v1.1.0 due to it is the only version with out dependencies
            var envFilePath = System.IO.Path.Combine(System.Web.HttpRuntime.AppDomainAppPath, ".env");
            if (File.Exists(envFilePath))
            {
                DotNetEnv.Env.Load(envFilePath);
            }

            var corsOptions = CorsConfig.ConfigureCors(Settings.AllowedOrigins);
            app.UseCors(corsOptions);

            HttpConfiguration config = new HttpConfiguration();

            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));
            config.Filters.Add(new ExceptionFilter());
            config.MessageHandlers.Add(new RequestResponseHandler());

            if (string.IsNullOrEmpty(ERPSettings.ConnectionString))
                return;

            FormatterConfig.Configure(config);
            RouteConfig.Configure(config);
            LoggerConfig.Configure(config);
            OptionsMessageHandlerConfig.Configure(config);
            SwaggerConfig.Configure(config);
            AutofacConfig.Configure(config);
            app.UseAutofacMiddleware(AutofacConfig.Container);
            app.UseAutofacWebApi(config);
            app.UseAuthentication();
            app.PreventResponseCaching();
            app.UseWebApi(config);
        }
    }
}
