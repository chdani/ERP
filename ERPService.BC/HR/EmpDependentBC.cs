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
    public class EmpDependentBC
    {
        private ILogger _logger;
        private IRepository _repository;
        private ObjectCache cache = MemoryCache.Default;

        public EmpDependentBC(ILogger logger, IRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }
        public AppResponse saveEmpDependent(EmpDependent empInfo)
        {
            AppResponse appResponse = new AppResponse();
            List<String> validationMessages = new List<string>();
            bool validation = true;
            var emp = _repository.GetQuery<EmpDependent>().Where(a => a.Id != empInfo.Id).FirstOrDefault();
            if (emp != null)
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.DUBLICATEDATAALREADY]);
                validation = false;
            }
            if (emp != null && validation)
            {
                var empDependent = new EmpDependent();
                empDependent.EmployeeId = empInfo.EmployeeId;
                empDependent.FullName = empInfo.FullName;
                empDependent.QatariID = empInfo.QatariID;
                empDependent.Passport = empInfo.Passport;
                empDependent.DOB = empInfo.DOB;
                empDependent.PlaceOfBirth = empInfo.PlaceOfBirth;
                empDependent.RelationCode = empInfo.RelationCode;
                empDependent.Active = "Y";

                InsertUpdateEmpDependent(empDependent);
                appResponse.Status = APPMessageKey.DATASAVESUCSS;
            }
            else
            {
                appResponse.Status = APPMessageKey.ONEORMOREERR;
                appResponse.Messages = validationMessages;
            }
            return appResponse;
        }
        private void InsertUpdateEmpDependent(EmpDependent empInfo)
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

        public List<EmpDependent> getallEmpDependent()
        {
            var getEmp = _repository.GetQuery<EmpDependent>();
            List<EmpDependent> map = new List<EmpDependent>();
            foreach (var empData in getEmp)
            {
                map.Add(new EmpDependent
                {
                    EmployeeId = empData.EmployeeId,
                    FullName = empData.FullName,
                    QatariID = empData.QatariID,
                    Passport = empData.Passport,
                    DOB = empData.DOB,
                    PlaceOfBirth = empData.PlaceOfBirth,
                    RelationCode = empData.RelationCode,
                    Active = empData.Active
                });
            }
            return map;
        }
        public EmpDependent getEmpDependent(Guid empId)
        {
            var empdependent = _repository.GetById<EmpDependent>(empId);
            return empdependent;
        }
        public AppResponse empDependentDelete(Guid id)
        {
            EmpDependent empDependent = new EmpDependent();
            AppResponse appResponse = new AppResponse();
            List<String> validationMessages = new List<string>();

            bool validation = true;
            var emp = _repository.GetQuery<EmpDependent>().Where(a => a.EmployeeId == id).FirstOrDefault();

            if (emp != null)
            {
                if (emp != null)
                    validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.NOREC_EMPDEP]);
                else
                    validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.RECNOTFOUND]);

                validation = false;
            }
            if (emp != null && validation)
            {
                emp.Active = "N";
                InsertUpdateEmpDependent(emp);
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

