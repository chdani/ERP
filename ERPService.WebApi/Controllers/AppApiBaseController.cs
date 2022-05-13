using ERPService.BC;
using ERPService.BC.Utility;
using ERPService.Common;
using ERPService.Common.Interfaces;
using ERPService.Common.Shared;
using ERPService.DataModel.CTO;
using ERPService.DataModel.DTO;
using Microsoft.Owin;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

namespace ERPService.WebApi
{
    public class AppApiBaseController : ApiController
    {
        private Microsoft.Owin.Logging.ILogger logger;
        private IRepository repository;
        private IOwinContext context;

        protected ILogger _logger { get; }
        protected IRepository _repository { get; }
        protected UserContext _userContext { get; }
        
        public AppApiBaseController(ILogger logger, IRepository repository, IOwinContext context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository;
            if (context.Environment.ContainsKey("USRCTX"))
                _userContext = (UserContext)context.Environment["USRCTX"];
            else
                _userContext = new UserContext();

            AppMessagesBC.GetAppMessages(_repository,_userContext);


            var lang = "en";
            if (_userContext != null)
                lang = _userContext.Language;

            var langMasterBC = new LangMasterBC(logger, repository);
            var systemSettings = langMasterBC.GetLangBasedDataForSystemSettings(lang);
            foreach (var systemSetting in systemSettings)
            {
                if (!ERPSettings.APPSYSTEMSETTINGS.ContainsKey(systemSetting.ConfigKey))
                    ERPSettings.APPSYSTEMSETTINGS.Add(systemSetting.ConfigKey, systemSetting.ConfigValue);
            }

        }

        public AppApiBaseController(Microsoft.Owin.Logging.ILogger logger, IRepository repository, IOwinContext context)
        {
            this.logger = logger;
            this.repository = repository;
            this.context = context;
        }

        public void CreateFileResponse(HttpResponseMessage httpResponse, byte[] _byteArray)
        {
            // string fileName = "file.pdf";
            if (_byteArray != null)
            {
                httpResponse.Content = new ByteArrayContent(_byteArray);
                httpResponse.Content.Headers.ContentLength = _byteArray.Length;
                httpResponse.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                //  httpResponse.Content.Headers.ContentDisposition.FileName = fileName;
                //  httpResponse.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(MimeMapping.GetMimeMapping(httpResponse.Content.Headers.ContentDisposition.FileName));

            }
            else
            {
                httpResponse = new HttpResponseMessage(System.Net.HttpStatusCode.NoContent);
                httpResponse.Content = new StringContent("No records found");
            }
        }

    }

}