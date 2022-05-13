using ERPService.BC.Utility;
using ERPService.Common;
using ERPService.Common.Interfaces;
using ERPService.Common.Shared;
using ERPService.DataModel.DTO;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERPService.BC
{
    public class DepartmentBC
    {
        private ILogger _logger;
        private IRepository _repository;
        public DepartmentBC(ILogger logger, IRepository repository)
        {
            _logger = logger;
            _repository = repository;

        }

        public DepartmentBC(ILogger logger)
        {
            _logger = logger;
        }

        public AppResponse SaveHrDepartment(Department department)
        {
            List<String> validationMessages = new List<string>();
            AppResponse appResponse = new AppResponse();
            bool validation = true;
            var Dept = _repository.GetQuery<Department>().Where(a => a.Id != department.Id && (a.Code == department.Code || a.Name == department.Name)).FirstOrDefault();
            if (Dept != null)
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.DUPLICATECODE_NAME]);
                validation = false;
            }
            var department1 = new Department();
            if (string.IsNullOrEmpty(department.Code) || string.IsNullOrEmpty(department.Name))
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.MANDMISSING]);
                validation = false;
            }
            if (department != null && validation)

            {
                InsertUpdateHr(department, true);
                appResponse.Status = APPMessageKey.DATASAVESUCSS;

            }
            else
            {
                appResponse.Status = APPMessageKey.ONEORMOREERR;
                appResponse.Messages = validationMessages;
            }
            return appResponse;
        }
        public AppResponse SaveDepartmentList(List<Department> departmentList)
        {
            AppResponse appResponse = new AppResponse();
            foreach (var department in departmentList)
            {
                InsertUpdateHr(department, false);
                appResponse.Status = APPMessageKey.DATASAVESUCSS;

            }
            if (appResponse.Status == APPMessageKey.DATASAVESUCSS)
            {
                try
                {
                    _repository.SaveChanges();
                    AppGeneralMethods.RemoveCache(ERPCacheKey.DEPARTMENT, _repository);
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

        private void InsertUpdateHr(Department department1, bool saveChanges)
        {
            if (department1.Id == Guid.Empty)
            {
                department1.Id = Guid.NewGuid();
                _repository.Add(department1, false);

            }
            else
                _repository.Update(department1, false);
            if (saveChanges)
            {
                _repository.SaveChanges();
            }


        }

        public List<Department> GetDepartmentList()
        {
            return _repository.GetQuery<Department>().Where(a => a.Active == "Y").ToList();
        }
        public AppResponse markdepartmentDelete(Guid userdepartmentId)
        {
            Department department = new Department();
            AppResponse appResponse = new AppResponse();
            List<String> validationMessages = new List<string>();

            bool validation = true;
            var department1 = _repository.GetQuery<Department>().FirstOrDefault(a => a.Id == userdepartmentId);
            var department2 = _repository.GetQuery<Employee>().FirstOrDefault(a => a.CurrDepartmentId == userdepartmentId && a.Active == "Y");
            var department3 = _repository.GetQuery<Department>().FirstOrDefault(a => a.ParentId == userdepartmentId && a.Active == "Y");
            if (department1 == null || department1.Id == Guid.Empty || department2 != null || department3 != null)
            {
                if (department2 != null)
                    validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.CANOTDETASSEMP]);
                if (department3 != null)
                    validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.CANOTDETASSPARDEP]);
                else
                    validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.RECNOTFOUND]);

                validation = false;
            }
            if (department1 != null && validation)
            {
                _repository.Delete<Department>(department1);
                appResponse.Status = APPMessageKey.DATASAVESUCSS;
            }
            else
            {
                appResponse.Status = APPMessageKey.ONEORMOREERR;
                appResponse.Messages = validationMessages;
            }
            _repository.SaveChanges();
            return appResponse;
        }
        public Department GetdepartmentByUserId(Guid departmentid)
        {

            return _repository.GetById<Department>(departmentid);
        }
        public List<Department> Getparentdepartment()
        {

            return _repository.GetQuery<Department>().Where(a => a.Active == "Y").ToList();
        }
    }
}
