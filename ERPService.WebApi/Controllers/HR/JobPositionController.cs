using ERPService.BC;
using ERPService.Common.Interfaces;
using ERPService.Common.Shared;
using ERPService.DataModel.DTO;
using Serilog;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace ERPService.WebApi
{
    [Route("api/[controller]")]
    [Authorize]
    public class JobPositionController : ApiController
    {
        private ILogger _logger { get; }
        private IRepository _repository { get; }
        public JobPositionController(ILogger logger, IRepository repository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository;
        }
        [HttpPost]
        [Route("saveJobPosition")]
        [Authorize]
        public AppResponse saveJobPosition(JobPosition jobInfo)
        {
             JobPositionBC JobBC = new JobPositionBC(_logger, _repository);
              return JobBC.SaveJobPosition(jobInfo);
        }
        [HttpGet]
        [Route("getalljobposition")]
        [Authorize]
        public List<JobPosition> getalljobposition()
        {
            JobPositionBC JobBC = new JobPositionBC(_logger, _repository);
            return JobBC.getalljobposition();
        }
        [HttpGet]
        [Route("getJobPositionById/{jobId}")]
        [Authorize]
        public JobPosition getJobPositionById(Guid jobId)
        {
            JobPositionBC JobBC = new JobPositionBC(_logger, _repository);
            return JobBC.getJobPositionById(jobId);
        }
        [HttpGet]
        [Route("jobpositionDelete/{id}")]
        [Authorize]
        public AppResponse jobpositionDelete(Guid id)
        {
            JobPositionBC JobBC = new JobPositionBC(_logger, _repository);
            return JobBC.jobpositionDelete(id);
        }
        [HttpGet]
        [Route("jobpositionActiveorInactive/{id}")]
        [Authorize]
        public AppResponse jobpositionActiveorInactive(Guid id)
        {
            JobPositionBC JobBC = new JobPositionBC(_logger, _repository);
            return JobBC.jobpositionActiveorInactive(id);
        }
        [HttpGet]
        [Route("getJobPosition")]
        [Authorize]
        public List<JobPosition> getJobPosition()
        {
            JobPositionBC JobBC = new JobPositionBC(_logger, _repository);
            return JobBC.getJobPosition();
        }
    }
}