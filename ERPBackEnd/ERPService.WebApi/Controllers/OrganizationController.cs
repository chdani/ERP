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
    [Route("api/organization")]
    [Authorize]
    public class OrganizationController : ApiController
    {
        private ILogger _logger { get; }
        private IRepository _repository { get; }

        public OrganizationController(ILogger logger, IRepository repository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository;
        }

        [HttpGet]
        [Route("getOrganizations")]
        [Authorize]
        public List<Organization> GetOrganizations()
        {
            var organizationBC = new OrganizationBC(_logger, _repository);
            return organizationBC.GetOrganizationList();
        }
        [HttpGet]
        [Route("GetUserOrganizations/{userId}")]
        [Authorize]
        public List<Organization> GetUserOrganizations(Guid userId)
        {
            var organizationBC = new OrganizationBC(_logger, _repository);
            return organizationBC.GetUserOrganizations(userId);
        }

        [HttpGet]
        [Route("getOrganizationById/{orgId}")]
        [Authorize]
        public Organization GetOrganizationById(Guid orgId)
        {
            var organizationBC = new OrganizationBC(_logger, _repository);
            return organizationBC.GetOrganizationById(orgId);
        }

        [HttpPost]
        [Route("saveOrganization")]
        [Authorize]
        public AppResponse SaveOrganization(Organization Org)
        {
            var organizationBC = new OrganizationBC(_logger, _repository);
            return organizationBC.SaveOrganization(Org);
        }
    }
}