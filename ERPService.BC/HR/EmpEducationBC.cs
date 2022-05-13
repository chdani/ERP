using ERPService.Common;
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
    public class EmpEducationBC
    {
        private ILogger _logger;
        private IRepository _repository;
        private ObjectCache cache = MemoryCache.Default;

        public EmpEducationBC(ILogger logger, IRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }
        public AppResponse saveEmpEducation(EmpEducation empInfo)
        {
            AppResponse appResponse = new AppResponse();
            List<String> validationMessages = new List<string>();
            bool validation = true;
            var emp = _repository.GetQuery<EmpEducation>().Where(a => a.Id != empInfo.Id).FirstOrDefault();
            if (emp != null)
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.DUBLICATEDATAALREADY]);
                validation = false;
            }
            if (emp != null && validation)
            {
                var empEducation = new EmpEducation();
                empEducation.EmployeeId = empInfo.EmployeeId;
                empEducation.EduLevelCode = empInfo.EduLevelCode;
                empEducation.EstablishmentCode = empInfo.EstablishmentCode;
                empEducation.Specialization = empInfo.Specialization;
                empEducation.CompletedYear = empInfo.CompletedYear;
                empEducation.GradePercentage = empInfo.GradePercentage;
                empEducation.Remarks = empInfo.Remarks;
                empEducation.Active = "Y";

                InsertUpdateEmpEducation(empEducation);
                appResponse.Status = APPMessageKey.DATASAVESUCSS;
            }
            else
            {
                appResponse.Status = APPMessageKey.ONEORMOREERR;
                appResponse.Messages = validationMessages;
            }
            return appResponse;
        }
        private void InsertUpdateEmpEducation(EmpEducation empInfo)
        {
            try
            {
                if (empInfo.Id == Guid.Empty)
                {
                    empInfo.Id = Guid.NewGuid();
                    _repository.Add(empInfo, true);

                }
                else
                {

                    _repository.Update(empInfo, true);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<EmpEducation> getallEmpEducation()
        {
            var getEmp = _repository.GetQuery<EmpEducation>();
            List<EmpEducation> map = new List<EmpEducation>();
            foreach (var empData in getEmp)
            {
                map.Add(new EmpEducation
                {
                    EmployeeId = empData.EmployeeId,
                    EduLevelCode = empData.EduLevelCode,
                    EstablishmentCode = empData.EduLevelCode,
                    Specialization = empData.Specialization,
                    CompletedYear = empData.CompletedYear,
                    GradePercentage = empData.GradePercentage,
                    Remarks = empData.Remarks,
                    Active = empData.Active
                });
            }
            return map;
        }
        public EmpEducation getEmpEducation(Guid empId)
        {
            var empEducation = _repository.GetById<EmpEducation>(empId);
            return empEducation;
        }
        public AppResponse empEducationDelete(Guid id)
        {
            EmpEducation empEducation = new EmpEducation();
            AppResponse appResponse = new AppResponse();
            List<String> validationMessages = new List<string>();

            bool validation = true;
            var emp = _repository.GetQuery<EmpEducation>().Where(a => a.EmployeeId == id).FirstOrDefault();

            if (emp != null)
            {
                if (emp != null)
                    validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.NOREC_EMPEDUCATE]);
                else
                    validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.RECNOTFOUND]);

                validation = false;
            }
            if (emp != null && validation)
            {
                emp.Active = "N";
                InsertUpdateEmpEducation(emp);
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
