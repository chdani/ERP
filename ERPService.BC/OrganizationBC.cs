using ERPService.BC.Utility;
using ERPService.Common;
using ERPService.Common.Interfaces;
using ERPService.Common.Shared;
using ERPService.Data;
using ERPService.DataModel.CTO;
using ERPService.DataModel.DTO;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Caching;

namespace ERPService.BC
{
    public class OrganizationBC
    {
        private ILogger _logger;
        private IRepository _repository;
        public OrganizationBC(ILogger logger, IRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }
        public Organization GetOrganizationById(Guid organizationId)
        {
            return _repository.Get<Organization>(a => a.Id == organizationId);
        }

        public List<Organization> GetUserOrganizations(Guid userId)
        {
            var LuserOrgs = _repository.GetQuery<UserOrganizationMap>().Where(a => a.UserID == userId).ToList();
            var allOrganizations = _repository.GetQuery<Organization>().Where(a => a.Active == "Y").ToList();
            var organizations = (from o in allOrganizations
                                 from l in LuserOrgs
                                 where o.Id == l.OrganizationId
                                 select o).ToList();
            return organizations;
        }
        public List<Organization> GetOrganizationList()
        {
            ObjectCache cache = MemoryCache.Default;
            var cacheKey = ERPCacheKey.ORGINFO;
            List<Organization> organizations = null;
            if (cache.Contains(cacheKey))
                organizations = (List<Organization>)cache.Get(cacheKey);
            else
            {
                organizations = _repository.GetQuery<Organization>().Where(a => a.Active == "Y").ToList();
                cache.Set(cacheKey, organizations, DateTime.Now.AddDays(1));
            }
            return organizations;
        }

        public AppResponse SaveOrganization(Organization organization)
        {
            AppResponse appResponse = new AppResponse();
            List<String> validationMessages = new List<string>();

            bool validation = true;

            if (string.IsNullOrEmpty(organization.OrgCode) || string.IsNullOrEmpty(organization.OrgName))
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.CODEORNAMEMISSING]);
                validation = false;
            }

            var orgInfo = _repository.GetQuery<Organization>().Where(a => a.Id != organization.Id &&
            (a.OrgCode == organization.OrgCode || a.OrgName == organization.OrgName)).FirstOrDefault();

            if (orgInfo != null)
            {
                validationMessages.Add(String.Format(ERPExceptions.APP_MESSAGES[APPMessageKey.DUPDATACODENAME], organization.OrgCode, organization.OrgName));
                validation = false;
            }


            if (validation)
            {
                InsertUpdateOrganization(organization, true);
                appResponse.Status = APPMessageKey.DATASAVESUCSS;
                ObjectCache cache = MemoryCache.Default;
                cache.Remove(ERPCacheKey.ORGINFO);
            }
            else
            {
                appResponse.Status = APPMessageKey.ONEORMOREERR;
                appResponse.Messages = validationMessages;
            }
            return appResponse;
        }
        public AppResponse SaveOrganizationList(List<Organization> organizationList)
        {
            AppResponse appResponse = new AppResponse();
            foreach (var organization in organizationList)
            {
                InsertUpdateOrganization(organization, false);
                appResponse.Status = APPMessageKey.DATASAVESUCSS;

            }
            if (appResponse.Status == APPMessageKey.DATASAVESUCSS)
            {
                try
                {
                    _repository.SaveChanges();
                    AppGeneralMethods.RemoveCache(ERPCacheKey.ORGINFO, _repository);
                    appResponse.Status = APPMessageKey.DATASAVESUCSS;
                    appResponse.Messages = new List<string>();
                }
                catch (Exception ex)
                {
                    appResponse.Status = APPMessageKey.ONEORMOREERR;
                    appResponse.Messages = new List<string>();
                    appResponse.Messages.Add(ex.Message);
                }
            }
            return appResponse;
        }

        private void InsertUpdateOrganization(Organization org, bool saveChanges)
        {
            if (org.Id == Guid.Empty)
            {
                org.Id = Guid.NewGuid();
                _repository.Add(org, false);
            }
            else
                _repository.Update(org, false);
            if (saveChanges)
            {
                _repository.SaveChanges();
            }

        }
    }
}
