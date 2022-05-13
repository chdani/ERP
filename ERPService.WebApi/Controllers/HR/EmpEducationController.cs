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
    public class EmpEducationController : ApiController
    {
        private ILogger _logger { get; }
        private IRepository _repository { get; }
        public EmpEducationController(ILogger logger, IRepository repository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository;
        }
        [HttpPost]
        [Route("saveEmpEducation")]
        [Authorize]
        public AppResponse saveEmpEducation(EmpEducation empInfo)
        {
            EmpEducationBC empBC = new EmpEducationBC(_logger, _repository);
            return empBC.saveEmpEducation(empInfo);
        }
        [HttpGet]
        [Route("getallEmpEducation")]
        [Authorize]
        public List<EmpEducation> getallEmpEducation()
        {
            EmpEducationBC empBC = new EmpEducationBC(_logger, _repository);
            return empBC.getallEmpEducation();
        }
        [HttpGet]
        [Route("getEmpEducation/{empId}")]
        [Authorize]
        public EmpEducation getEmpEducation(Guid empId)
        {
            EmpEducationBC empBC = new EmpEducationBC(_logger, _repository);
            return empBC.getEmpEducation(empId);
        }
        [HttpGet]
        [Route("empEducationDelete/{id}")]
        [Authorize]
        public AppResponse empEducationDelete(Guid id)
        {
            EmpEducationBC empBC = new EmpEducationBC(_logger, _repository);
            return empBC.empEducationDelete(id);
        }
    }
}