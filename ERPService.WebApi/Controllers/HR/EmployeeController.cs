using ERPService.BC;
using ERPService.Common.Interfaces;
using ERPService.Common.Shared;
using ERPService.DataModel.DTO;
using Serilog;
using System;
using System.Collections.Generic;
using System.Web.Http;
using ERPService.DataModel.CTO;
using Microsoft.Owin;

namespace ERPService.WebApi
{
    [Route("api/[controller]")]
    [Authorize]
    public class EmployeeController : AppApiBaseController
    {
        public EmployeeController(ILogger logger, IRepository repository, IOwinContext context) : base(logger, repository, context)
        {
        }

        [HttpPost]
        [Route("SaveEmployee")]
        [Authorize]
        public AppResponse saveEmployee(Employee employeeInfo)
        {
            EmployeeBC employeeBC = new EmployeeBC(_logger, _repository);
            return employeeBC.saveEmployee(employeeInfo);
        }
        [HttpGet]
        [Route("getEmployeeList")]
        [Authorize]
        public List<Employee> GetEmployeeList()
        {
            EmployeeBC employeeBC = new EmployeeBC(_logger, _repository);
            return employeeBC.GetEmployeeList();
        }
        [HttpGet]
        [Route("GetEmployeeById/{employeeId}")]
        [Authorize]
        public Employee GetEmployeeById(Guid employeeId)
        {
            EmployeeBC employeeBC = new EmployeeBC(_logger, _repository);
            return employeeBC.GetEmployeeById(employeeId);
        }


        

        [HttpGet]
        [Route("GetEmployeeDetailesList/{employeeId}")]
        [Authorize]
        public EmployeeDetailes GetEmployeeDetailesList(Guid employeeId)
        {
            EmployeeBC employeeBC = new EmployeeBC(_logger, _repository);
            return employeeBC.GetEmployeeDetailesList(employeeId);
        }

        [HttpGet]
        [Route("GetEmployeeAttachments/{employeeId}")]
        [Authorize]
        public List<AppDocument> GetEmployeeAttachments(Guid employeeId)
        {
            EmployeeBC employeeBC = new EmployeeBC(_logger, _repository);
            return employeeBC.GetEmployeeAttachments(employeeId);
        }

        [HttpGet]
        [Route("markEmployeeInactive/{employeeid}")]
        [Authorize]
        public AppResponse markEmployeeInactive(Guid employeeid)
        {
            EmployeeBC employeeBC = new EmployeeBC(_logger, _repository);
            return employeeBC.employeeInactive(employeeid);
        }


        [HttpPost]
        [Route("getEmployeesBySearchCriteria")]
        [Authorize]
        public List<Employee> GetEmployeesBySearchCriteria(Employee employee)
        {
            EmployeeBC employeeBC = new EmployeeBC(_logger, _repository);
            return employeeBC.GetEmployeesBySearchCriteria(employee);
        }
    }
}