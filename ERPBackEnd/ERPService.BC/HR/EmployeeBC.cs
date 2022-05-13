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
using ERPService.DataModel.CTO;

namespace ERPService.BC
{
    public class EmployeeBC
    {
        private ILogger _logger;
        private IRepository _repository;
        private ObjectCache cache = MemoryCache.Default;

        public EmployeeBC(ILogger logger, IRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }
        public AppResponse saveEmployee(Employee employeeInfo)
        {
            EmpEducation empEducation = new EmpEducation();
            EmpDependent empDependent = new EmpDependent();
            AppResponse appResponse = new AppResponse();
            List<String> validationMessages = new List<string>();
            bool validation = true;

            var qatriId = _repository.GetQuery<Employee>().FirstOrDefault(a => a.Active == "Y" && a.Id != employeeInfo.Id && (a.QatariID == employeeInfo.QatariID));

            if (qatriId != null)
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.DUPLIQATARIID]);
                validation = false;
            }
            var EmployeeId  = _repository.GetQuery<Employee>().FirstOrDefault(a => a.Active == "Y" && a.Id != employeeInfo.Id && (a.EmpNumber == employeeInfo.EmpNumber));

            if (EmployeeId != null)
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.DUPLIEMPLOYEEID]);
                validation = false;
            }


            if (employeeInfo.DOB >= DateTime.Now.AddYears(-18))
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.DATATIMEVALID]);
                validation = false;
            }


            if (employeeInfo != null && validation)
            {

                employeeInfo.DOB = employeeInfo.DOB.Value.ToLocalTime();
                employeeInfo.Active = "Y";
                var existingEmpEducation = _repository.GetQuery<EmpEducation>().Where(a => a.EmployeeId == employeeInfo.Id).ToList();
                if (existingEmpEducation != null && existingEmpEducation.Count > 0)
                {
                    foreach (var item in existingEmpEducation)
                    {
                        _repository.Delete<EmpEducation>(item);
                    }
                }
                var existingEmpDependent = _repository.GetQuery<EmpDependent>().Where(a => a.EmployeeId == employeeInfo.Id).ToList();
                if (existingEmpDependent != null && existingEmpDependent.Count > 0)
                {
                    foreach (var item in existingEmpDependent)
                    {
                        _repository.Delete<EmpDependent>(item);
                    }
                }
                InsertUpdateemployee(employeeInfo);
                if (employeeInfo.CreateUser == true && employeeInfo.Id != Guid.Empty)
                {
                    var userMaster = new UserMaster();
                    userMaster.FirstName = employeeInfo.FullNameEng;
                    userMaster.LastName = employeeInfo.FullNameEng;
                    userMaster.EmailId = employeeInfo.Email;
                    userMaster.EmployeeId = employeeInfo.Id;
                    userMaster.UserName = employeeInfo.FullNameEng;
                    userMaster.UserType = "G";
                    var userBC = new UserMasterBC(_logger, _repository);
                    var newUser = userBC.SaveUserInfo(userMaster);
                    if (newUser.ReferenceId != Guid.Empty)
                    {
                        var userMasterBC = new UserMasterBC(_logger, _repository);
                        return userMasterBC.SendPasswordResetMail(newUser.ReferenceId);
                    }
                }

                appResponse.Status = APPMessageKey.DATASAVESUCSS;
            }
            else
            {
                appResponse.Status = APPMessageKey.ONEORMOREERR;
                appResponse.Messages = validationMessages;
            }

            return appResponse;
        }

        public List<AppDocument> GetEmployeeAttachments(Guid Id)
        {
            var usemaster = _repository.GetQuery<UserMaster>().ToList();

            var comments = _repository.GetQuery<Employee>().Where(a => a.Id == Id && a.Active == "Y").ToList();

            var fileAttachmentIds = new List<Guid>();
            fileAttachmentIds.AddRange(comments.Select(a => a.Id).Distinct().ToList());
            fileAttachmentIds.Add(Id);

            var appDoucuments = _repository.GetQuery<AppDocument>().Where(a => a.Active == "Y"
                    && (a.TransactionType == "EMPLOYEE" || a.TransactionType == "EMPLOYEE") && fileAttachmentIds.Contains(a.TransactionId))
                .OrderByDescending(a => a.CreatedDate).ToList();

            appDoucuments.ForEach(ele =>
            {
                var userName = usemaster.Where(a => a.Id == ele.CreatedBy).FirstOrDefault();
                if (userName != null)
                    ele.UserName = userName.FirstName + " " + userName.LastName;
                ele.FileContent = null;
            });
            return appDoucuments.OrderByDescending(a => a.CreatedDate).ToList();
        }
        private void InsertUpdateemployee(Employee employeeInfo)
        {
            if (employeeInfo.Id == Guid.Empty)
            {
                employeeInfo.Id = Guid.NewGuid();
                _repository.Add(employeeInfo, true);
            }
            else
            {
                _repository.Update(employeeInfo, true);
            }

            if (employeeInfo.EmpEducation != null && employeeInfo.EmpEducation.Count != 0)
            {
                foreach (var item in employeeInfo.EmpEducation)
                {
                    item.EmployeeId = employeeInfo.Id;
                    item.Active = "Y";
                    InsertUpdateEmpEducation(item, false);
                }
            }
            if (employeeInfo.AppDocuments != null && employeeInfo.AppDocuments.Count > 0)
            {
                foreach (var appDocument in employeeInfo.AppDocuments)
                {
                    appDocument.TransactionId = employeeInfo.Id;
                }
                var appDocumentBC = new AppDocumentBC(_logger, _repository);
                var AppDocument = appDocumentBC.saveAppDocument(employeeInfo.AppDocuments, false);
            }
            if (employeeInfo.EmpDependent != null && employeeInfo.EmpDependent.Count != 0)
            {
                foreach (var item in employeeInfo.EmpDependent)
                {
                    item.EmployeeId = employeeInfo.Id;
                    item.DOB = item.DOB.Value.ToLocalTime();
                    item.Active = "Y";
                    InsertUpdateEmpDependent(item, false);
                }
            }
            _repository.SaveChanges();
        }

        private void InsertUpdateEmpDependent(EmpDependent EmpDependent, Boolean depent)
        {
            if (EmpDependent.Id == Guid.Empty)
            {
                EmpDependent.Id = Guid.NewGuid();
                _repository.Add(EmpDependent, true);
            }
            else
                _repository.Update(EmpDependent, true);
            if (depent)
                _repository.SaveChanges();

        }
        private void InsertUpdateEmpEducation(EmpEducation empEducation, Boolean edu)
        {
            if (empEducation.Id == Guid.Empty)
            {
                empEducation.Id = Guid.NewGuid();
                _repository.Add(empEducation, true);
            }
            else
                _repository.Update(empEducation, true);

            if (edu)
                _repository.SaveChanges();
        }
        public List<Employee> GetEmployeesBySearchCriteria(Employee employee)
        {
            if (employee.FromDobDate != null && employee.ToDobDate != null)
            {
                employee.FromDobDate = employee.FromDobDate.Value.ToLocalTime();
                employee.ToDobDate = employee.ToDobDate.Value.ToLocalTime();
            }

            return _repository.GetQuery<Employee>().Where(a =>
               (string.IsNullOrEmpty(employee.EmpNumber) || a.EmpNumber.Contains(employee.EmpNumber))
              && (string.IsNullOrEmpty(employee.QatariID) || a.QatariID.Contains(employee.QatariID))
              && (string.IsNullOrEmpty(employee.FullNameEng) || a.FullNameEng.Contains(employee.FullNameEng))
               && (string.IsNullOrEmpty(employee.FullNameArb) || a.FullNameArb.Contains(employee.FullNameArb))
                && (string.IsNullOrEmpty(employee.Nationality) || a.Nationality.Contains(employee.Nationality))
             && ((employee.CurrDepartmentId == Guid.Empty) || (employee.CurrDepartmentId == a.CurrDepartmentId)) &&
              ((employee.CurrPositionId == Guid.Empty) || (employee.CurrPositionId == a.CurrPositionId)) &&
              ((a.DOB >= employee.FromDobDate) && (a.DOB <= employee.ToDobDate) || employee.FromDobDate == null && employee.ToDobDate == null) &&
              ((employee.CurrentGrade == 0) || (employee.CurrentGrade == a.CurrentGrade)) &&
              (string.IsNullOrEmpty(employee.Others) || a.PhoneNumber.Contains(employee.Others)
              || a.Email.Contains(employee.Others) || a.Passport.Contains(employee.Others)
              || a.PlaceOfBirth.Contains(employee.Others) || a.Address.Contains(employee.Others)
              || a.MaritalStatusCode.Contains(employee.Others)) &&
               a.Active == "Y").OrderByDescending(a => a.CreatedDate).ToList(); ;
        }




        public List<CodesDetails> getMaritialStatus(string basicinfo)
        {
            var codemaster = _repository.GetQuery<CodesMaster>().Where(a => a.Code == basicinfo).FirstOrDefault();

            var codedetails = _repository.GetQuery<CodesDetails>().Where(a => a.CodesMasterId == codemaster.Id).ToList();
            return codedetails;
        }


        public List<Employee> GetEmployeeList()
        {

            return _repository.GetQuery<Employee>().Where(a =>
               a.Active == "Y").OrderByDescending(a => a.CreatedDate).ToList();
        }
        public Employee GetEmployeeById(Guid employeeId)
        {

            var employee = _repository.GetById<Employee>(employeeId);
            employee.EmpEducation = _repository.GetQuery<EmpEducation>().Where(a => a.EmployeeId == employeeId).ToList();
            employee.EmpDependent = _repository.GetQuery<EmpDependent>().Where(a => a.EmployeeId == employeeId).ToList();
            employee.AppDocuments = GetEmployeeAttachments(employeeId);
            return employee;
        }
        public EmployeeDetailes GetEmployeeDetailesList(Guid employeeId)
        {
            EmployeeDetailes employeeDetailes = new EmployeeDetailes();
            employeeDetailes.employees = new List<Employee>();
            employeeDetailes.Educations = new List<EmpEducation>();
            employeeDetailes.dependents = new List<EmpDependent>();
            var data = _repository.GetById<Employee>(employeeId);

            employeeDetailes.employees.Add(data);
            var EmpEducation = _repository.GetQuery<EmpEducation>().Where(a => a.EmployeeId == employeeId).ToList();
            var dependents = _repository.GetQuery<EmpDependent>().Where(a => a.EmployeeId == employeeId).ToList();

            employeeDetailes.dependents = dependents;
            employeeDetailes.Educations = EmpEducation;
            return employeeDetailes;
        }
        public AppResponse employeeInactive(Guid employeeid)
        {
            AppResponse appResponse = new AppResponse();
            List<String> validationMessages = new List<string>();

            bool validation = true;
            var employee = _repository.GetQuery<Employee>().Where(a => a.Id == employeeid).FirstOrDefault();
            var EmpEducation = _repository.GetQuery<EmpEducation>().Where(a => a.EmployeeId == employeeid).FirstOrDefault();
            var EmpDependent = _repository.GetQuery<EmpDependent>().Where(a => a.EmployeeId == employeeid).FirstOrDefault();
            if (employee == null || employee.Id == Guid.Empty)
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.RECNOTFOUND]);
                validation = false;
            }

            if (employee != null && validation)
            {
                employee.Active = "N";
                InsertUpdateemployee(employee);
                if (EmpEducation != null)
                {

                    EmpEducation.Active = "N";
                    InsertUpdateEmpEducation(EmpEducation, true);
                }
                if (EmpDependent != null)
                {
                    EmpDependent.Active = "N";
                    InsertUpdateEmpDependent(EmpDependent, true);

                }
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
