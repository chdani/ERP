using ERPService.Common;
using ERPService.DataModel.CTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using ERPService.Data;
using Serilog;
using ERPService.DataModel.DTO;
using ERPService.Common.Shared;
using ERPService.Common.Interfaces;
using System.Net;
using System.Net.Mail;
using System.Runtime.Caching;
using System.IO;
namespace ERPService.BC
{
    public class JobPositionBC
    {
        private ILogger _logger;
        private IRepository _repository;
        private ObjectCache cache = MemoryCache.Default;

        public JobPositionBC(ILogger logger, IRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }
        public AppResponse SaveJobPosition(JobPosition jobInfo)
        {
            AppResponse appResponse = new AppResponse();
            List<String> validationMessages = new List<string>();
            bool validation = true;
            var job = _repository.GetQuery<JobPosition>().Where(a => a.Id != jobInfo.Id && (a.Code == jobInfo.Code || a.Name == jobInfo.Name)).FirstOrDefault();
            if (job != null)
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.DUPLICATECODE_NAME]);
                validation = false;
            }
            if (jobInfo != null && validation)
            {
                var jobposition = new JobPosition();
                jobposition.Id = jobInfo.Id;
                jobposition.Code = jobInfo.Code;
                jobposition.Name = jobInfo.Name;
                jobposition.CreatedBy = jobInfo.CreatedBy;
                jobposition.CreatedDate = jobInfo.CreatedDate;
                jobposition.ModifiedBy = jobInfo.ModifiedBy;
                jobposition.ModifiedDate = jobInfo.ModifiedDate;
                jobposition.Active = "Y";

                InsertUpdateJob(jobposition);
                appResponse.Status = APPMessageKey.DATASAVESUCSS;
            }
            else
            {
                appResponse.Status = APPMessageKey.ONEORMOREERR;
                appResponse.Messages = validationMessages;
            }
            return appResponse;
        }
        private void InsertUpdateJob(JobPosition jobInfo)
        {
            try
            {
                if (jobInfo.Id == Guid.Empty)
                {
                    jobInfo.Id = Guid.NewGuid();
                    _repository.Add(jobInfo, true);

                }
                else
                {

                    _repository.Update(jobInfo, true);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public List<JobPosition> getJobPosition()
        {
            return _repository.GetQuery<JobPosition>().Where(a => a.Active == "Y").ToList();
        }
        public List<JobPosition> getalljobposition()
        {
            var getjob = _repository.GetQuery<JobPosition>();
            List<JobPosition> map = new List<JobPosition>();
            foreach (var jobdata in getjob)
            {
                map.Add(new JobPosition
                {
                    Id = jobdata.Id,
                    Code = jobdata.Code,
                    Name = jobdata.Name,
                    Actives = jobdata.Active == "Y" ? true : false,
                });
            }


            return map;
        }
        public JobPosition getJobPositionById(Guid jobId)
        {
            var jobposition = _repository.GetById<JobPosition>(jobId);
            return jobposition;
        }

        public AppResponse jobpositionActiveorInactive(Guid id)
        {
            AppResponse appResponse = new AppResponse();
            List<String> validationMessages = new List<string>();

            bool validation = true;
            var user = _repository.GetQuery<JobPosition>().Where(a => a.Id == id).FirstOrDefault();
            if (user == null || user.Id == Guid.Empty)
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.RECNOTFOUND]);
                validation = false;
            }

            if (user != null && validation)
            {
                if (user.Active == "Y")
                {
                    user.Active = "N";
                }
                else
                {
                    user.Active = "Y";
                }
                InsertUpdateJob(user);
                appResponse.Status = APPMessageKey.DATASAVESUCSS;
            }
            else
            {
                appResponse.Status = APPMessageKey.ONEORMOREERR;
                appResponse.Messages = validationMessages;
            }

            return appResponse;
        }


        public AppResponse jobpositionDelete(Guid id)
        {
            JobPosition jobposition = new JobPosition();
            AppResponse appResponse = new AppResponse();
            List<String> validationMessages = new List<string>();

            bool validation = true;
            var jobposition1 = _repository.GetQuery<JobPosition>().Where(a => a.Id == id).FirstOrDefault();
            var empolyee = _repository.GetQuery<Employee>().Where(a => a.CurrPositionId == id).FirstOrDefault();

            if (jobposition1 == null || jobposition1.Id == Guid.Empty || empolyee != null)
            {
                if (jobposition1 != null)
                    validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.CANOTDETASSEMP]);
                else
                    validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.RECNOTFOUND]);

                validation = false;
            }
            if (jobposition1 != null && validation)
            {
                _repository.Delete<JobPosition>(jobposition1);
                appResponse.Status = APPMessageKey.DATASAVESUCSS;
            }
            else
            {
                appResponse.Status = APPMessageKey.ONEORMOREERR;
                appResponse.Messages = validationMessages;
            }

            return appResponse;
        }
    }
}
