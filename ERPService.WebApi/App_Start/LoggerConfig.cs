using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using SerilogWeb.Classic;
using System.IO;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using static System.Environment;
using System.Configuration;
using ERPService.DataModel.CTO;

namespace ERPService.WebApi
{
    public static class LoggerConfig
    {
        public static void Configure(HttpConfiguration config)
        {
            // More information can be found here https://github.com/serilog/serilog/wiki/Getting-Started
            var f = new FileInfo(Assembly.GetExecutingAssembly().Location);
            var name = f.Name.Replace(f.Extension, "");

            // TODO: Adjust log file location and name. 
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(ConfigurationManager.AppSettings["LogfilePath"].ToString() + $"{name}..txt", rollingInterval: RollingInterval.Day)
                // Enrich with SerilogWeb.Classic (https://github.com/serilog-web/classic)
                .Enrich.WithHttpRequestUrl()
                .Enrich.WithHttpRequestType()
                .Enrich.WithHttpRequestClientHostIP()
                .Enrich.WithHttpRequestId()
                .Enrich.WithExceptionDetails()
                .MinimumLevel.Information()
                .CreateLogger();

            var logLevel = ConfigurationManager.AppSettings["LogLevel"].ToString();
            if (logLevel != "ALL")
            {
                if(logLevel == "DEBUG")
                    SerilogWebClassic.Configure(cfg => cfg.LogAtLevel(LogEventLevel.Debug));   
                else if (logLevel == "INFO")
                    SerilogWebClassic.Configure(cfg => cfg.LogAtLevel(LogEventLevel.Information));   
                else if (logLevel == "ERROR")
                    SerilogWebClassic.Configure(cfg => cfg.LogAtLevel(LogEventLevel.Error));
                else if (logLevel == "FATAL")
                    SerilogWebClassic.Configure(cfg => cfg.LogAtLevel(LogEventLevel.Fatal));
            }
            config.Services.Replace(typeof(IExceptionLogger), new ExceptionLogger());
        }
    }
}
