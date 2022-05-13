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
    public class EmpDependentController : ApiController
    {
        private ILogger _logger { get; }
        private IRepository _repository { get; }
        public EmpDependentController(ILogger logger, IRepository repository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository;
        }
        [HttpPost]
        [Route("saveEmpDependent")]
        [Authorize]
        public AppResponse saveEmpDependent(EmpDependent empInfo)
        {
            EmpDependentBC empBC = new EmpDependentBC(_logger, _repository);
            return empBC.saveEmpDependent(empInfo);
        }
        [HttpGet]
        [Route("getallEmpDependent")]
        [Authorize]
        public List<EmpDependent> getallEmpDependent()
        {
            EmpDependentBC empBC = new EmpDependentBC(_logger, _repository);
            return empBC.getallEmpDependent();
        }
        [HttpGet]
        [Route("getEmpDependent/{empId}")]
        [Authorize]
        public EmpDependent getEmpDependent(Guid empId)
        {
            EmpDependentBC empBC = new EmpDependentBC(_logger, _repository);
            return empBC.getEmpDependent(empId);
        }
        [HttpGet]
        [Route("empDependentDelete/{id}")]
        [Authorize]
        public AppResponse empDependentDelete(Guid id)
        {
            EmpDependentBC empBC = new EmpDependentBC(_logger, _repository);
            return empBC.empDependentDelete(id);
        }
    }
}