using ERPService.BC;
using ERPService.Common.Interfaces;
using ERPService.Common.Shared;
using ERPService.DataModel.CTO;
using ERPService.DataModel.DTO;
using Microsoft.Owin;
using Serilog;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;


namespace ERPService.WebApi
{
    [Route("api/[controller]")]
    [Authorize]
    public class DepartmentController : ApiController
    {
        private ILogger _logger { get; }
        private IRepository _repository { get; }

        private UserContext _userContext;
        public DepartmentController(ILogger logger, IRepository repository, UserContext userContext)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository;
            _userContext = userContext;

        }
        [HttpPost]
        [Route("SaveHrDepartment")]
        [Authorize]
        public AppResponse SaveHrDepartment(Department department)
        {
            var DepartmentBC = new DepartmentBC(_logger, _repository);
            return DepartmentBC.SaveHrDepartment(department);

        }
        [HttpGet]
        [Route("GetDepartmentList")]
        [Authorize]
        public List<Department> GetDepartmentList()
        {
            var DepartmentBC = new DepartmentBC(_logger, _repository);
            return DepartmentBC.GetDepartmentList();
        }

        [HttpGet]
        [Route("markdepartmentInactive/{userdepartmentId}")]
        [Authorize]
        public AppResponse markdepartmentInactive(Guid userdepartmentId)
        {
            var DepartmentBC = new DepartmentBC(_logger, _repository);
            return DepartmentBC.markdepartmentDelete(userdepartmentId);
        }

        [HttpGet]
        [Route("GetdepartmentByUserId/{departmentId}")]
        [Authorize]
        public Department GetdepartmentByUserId(Guid departmentId)
        {
            var DepartmentBC = new DepartmentBC(_logger, _repository);
            return DepartmentBC.GetdepartmentByUserId(departmentId);
        }

        [HttpGet]
        [Route("Getparentdepartment")]
        [Authorize]
        public List<Department> Getparentdepartment()
        {
            var DepartmentBC = new DepartmentBC(_logger, _repository);
            return DepartmentBC.Getparentdepartment();
        }


    }
}