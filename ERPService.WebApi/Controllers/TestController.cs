using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ERPService.BC;
using ERPService.Common.Interfaces;
using ERPService.DataModel.DTO;
using Serilog;

namespace ERPService.WebApi.Controllers
{
    public class TestController : ApiController
    {
        private ILogger _logger { get; }
        private IRepository _repository { get; }

        public TestController(ILogger logger, IRepository repository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository;
        }

        [ActivityLogActionFilter("LoggIn")]
        [HttpGet]

        public void Index(string p1, string p2)
        {
            //try
            //{
            //var activityLog = new ActivityLogBC(_repository);
            //ActivityLog ac = new ActivityLog();
            //activityLog.InsertActvity(ac);
            int a = 1;
            int b = 0;
            int c = a / b;
            //}
            //catch (Exception)
            //{
            //    throw;
            //}
        }

        [HttpPost]

        public void Index_Post(TestParam test)
        {
            int a = 1;
            int b = 0;
            int c = a / b;
        }
    }

    public class TestParam
    {
        public string p1 { get; set; }
        public string p2 { get; set; }

    }

    public class TestParam2
    {
        public string p1 { get; set; }
        public string p2 { get; set; }

    }
}
