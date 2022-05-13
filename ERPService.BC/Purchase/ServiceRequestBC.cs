using ERPService.BC.Utility;
using ERPService.Common;
using ERPService.Common.Interfaces;
using ERPService.Common.Shared;
using ERPService.DataModel;
using ERPService.DataModel.CTO;
using ERPService.DataModel.DTO;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using static ERPService.BC.Utility.AppGeneralMethods;

namespace ERPService.BC
{
    public class ServiceRequestBC
    {
        private ILogger _logger;
        private IRepository _repository;
        private UserContext _userContext;

        public ServiceRequestBC(ILogger logger, IRepository repository, UserContext userContext)
        {
            _logger = logger;
            _repository = repository;
            _userContext = userContext;
        }

        public ServiceRequest GetServiceRequestById(Guid requestId)
        {
            var serviceRequest = _repository.GetById<ServiceRequest>(requestId);
            if (serviceRequest != null)
            {

                serviceRequest.ServReqApproval = _repository.GetQuery<ServiceReqApproval>().Where(a => a.Active == "Y" && a.ServiceRequestId == serviceRequest.Id).ToList();

                var approverIds = serviceRequest.ServReqApproval.Select(a => a.ApprovedBy).Distinct().ToList();

                var userMasterQuery = _repository.GetQuery<UserMaster>();
                var empQuery = _repository.GetQuery<Employee>();
                var empUser = (from usr in userMasterQuery
                               join emp in empQuery on usr.EmployeeId equals emp.Id
                               where approverIds.Contains(usr.Id)
                               select new
                               {
                                   UserId = usr.Id,
                                   EmpNameEng = emp.FullNameEng,
                                   EmpNameArb = emp.FullNameArb
                               }).ToList();

                var languageBC = new LangMasterBC(_logger, _repository);
                var prodCategories = languageBC.GetLangBasedDataForProductCategory(_userContext.Language);
                var prodSubCategories = languageBC.FetchLangBasedDataForProductSubCategory(_userContext.Language);

                foreach (var app in serviceRequest.ServReqApproval)
                {
                    var employee = empUser.FirstOrDefault(a => a.UserId == app.ApprovedBy);
                    app.UserName = employee == null ? "" : (_userContext.Language == "en" ? employee.EmpNameEng : employee.EmpNameArb);
                }

                var category = prodCategories.FirstOrDefault(a => a.Id == serviceRequest.ProdCategoryId);
                var subCategory = prodSubCategories.FirstOrDefault(a => a.Id == serviceRequest.ProdSubCategoryId);
                var reqEmployee = _repository.GetById<Employee>(serviceRequest.EmployeeId);

                serviceRequest.ProdCategoryName = category == null ? "" : category.Name;
                serviceRequest.ProdSubCategoryName = subCategory == null ? "" : subCategory.Name;
                serviceRequest.EmployeeName = reqEmployee == null ? "" : (_userContext.Language == "en" ? reqEmployee.FullNameEng : reqEmployee.FullNameArb);
            }
            return serviceRequest;
        }
        public AppResponse SaveServiceRequest(ServiceRequest serviceRequest)
        {
            List<String> validationMessages = new List<string>();
            AppResponse appResponse = new AppResponse();
            bool validation = true;

            if (serviceRequest.ProdCategoryId == Guid.Empty || serviceRequest.EmployeeId == Guid.Empty || serviceRequest.TransDate <= DateTime.MinValue)
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.MANDMISSING]);
                validation = false;
            }
            if (serviceRequest.Active == "Y")
            {

                var childValid = true;

            }
            var currentLevel = 0;
            if (serviceRequest != null && validation)
            {
                var levels = GetNextWorkFlowLevel(0, serviceRequest.ProdCategoryId);
                if (currentLevel == 0 && serviceRequest.Id == Guid.Empty)
                {
                    var serviceRequestTransNoAndSeqNo = AppGeneralMethods.TranstypeSeqNumber("serviceRequestTransType", _repository);
                    serviceRequest.TransNo = serviceRequestTransNoAndSeqNo.Item1;
                    serviceRequest.SeqNo = serviceRequestTransNoAndSeqNo.Item2;
                    serviceRequest.ServReqApproval = new List<ServiceReqApproval>();
                    serviceRequest.ServReqApproval.Add(new ServiceReqApproval()
                    {
                        Active = "Y",
                        ApprovalLevel = currentLevel,
                        ApprovedBy = _userContext.Id,
                        ApprovedDate = DateTime.Now,
                        Remarks = serviceRequest.Remarks,
                        Status = "SERREQSUBMITTED"
                    });
                }
                serviceRequest.CurApprovalLevel = 0;
                serviceRequest.NextApprovalLevel = levels.Item2;

                serviceRequest.TransDate = serviceRequest.TransDate.ToLocalTime().Date;

                InsertUpdateServiceRequest(serviceRequest, true);
                appResponse.Status = APPMessageKey.DATASAVESUCSS;
            }
            else
            {
                appResponse.Status = APPMessageKey.ONEORMOREERR;
                appResponse.Messages = validationMessages;
            }
            return appResponse;
        }

        public AppResponse SaveServiceRequestComment(ServiceReqComment serviceReqComment)
        {
            InsertServiceReqComments(serviceReqComment, true);
            return new AppResponse() { Status = APPMessageKey.DATASAVESUCSS };
        }
        public AppResponse ApproveServiceRequest(ServiceRequest serviceRequest, bool saveChanges)
        {
            List<String> validationMessages = new List<string>();
            AppResponse appResponse = new AppResponse();
            bool validation = true;

            if (serviceRequest.Id == Guid.Empty || string.IsNullOrEmpty(serviceRequest.ApproverRemarks))
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.MANDMISSING]);
                validation = false;
            }

            var approval = _repository.GetQuery<ProdCategoryWorkFlow>().FirstOrDefault(a => a.ProdCategoryId == serviceRequest.ProdCategoryId && a.ApprovalLevel == serviceRequest.NextApprovalLevel && a.Active == "Y");
            if (approval == null)
            {
                appResponse.Status = APPMessageKey.DATASAVESUCSS;
                return appResponse;
            }

            var loggedInUser = _repository.GetById<UserMaster>(_userContext.Id);
            if (loggedInUser == null || loggedInUser.Active == "N")
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES["NOTPERMITTEDAPPROVER"]);
                validation = false;
            }
            var loggedinEmployee = _repository.GetById<Employee>(loggedInUser.EmployeeId.Value);

            if (loggedinEmployee == null || loggedinEmployee.Active == "N")
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES["NOTPERMITTEDAPPROVER"]);
                validation = false;
            }
            if (validation)
            {
                switch (approval.ApprovalType)
                {
                    case "DEPARTMENTHEAD":
                        var depHead = _repository.GetQuery<Employee>().FirstOrDefault(a => a.IsDepratmentHead && a.Id == loggedinEmployee.Id && a.Active == "Y");
                        if (depHead == null)
                        {
                            validationMessages.Add(ERPExceptions.APP_MESSAGES["NOTPERMITTEDAPPROVER"]);
                            validation = false;
                        }
                        break;
                    case "DEPARTMENT":
                        var department = _repository.GetById<Department>(approval.ApprovalId);
                        if (department == null)
                        {
                            validationMessages.Add(ERPExceptions.APP_MESSAGES["NOTPERMITTEDAPPROVER"]);
                            validation = false;
                        }
                        else
                        {
                            var depEmployee = _repository.GetQuery<Employee>().FirstOrDefault(a => a.CurrDepartmentId == department.Id && a.Id == loggedinEmployee.Id && a.Active == "Y");
                            if (depEmployee == null)
                            {
                                validationMessages.Add(ERPExceptions.APP_MESSAGES["NOTPERMITTEDAPPROVER"]);
                                validation = false;
                            }
                        }
                        break;
                    case "USER":
                        if (loggedinEmployee.Id != approval.ApprovalId)
                        {
                            validationMessages.Add(ERPExceptions.APP_MESSAGES["NOTPERMITTEDAPPROVER"]);
                            validation = false;
                        }
                        break;
                    case "EMPLOYEEMANAGER":
                        var employee = _repository.GetById<Employee>(serviceRequest.EmployeeId);
                        if (employee == null || employee.Active == "N")
                        {
                            validationMessages.Add(ERPExceptions.APP_MESSAGES["NOTPERMITTEDAPPROVER"]);
                            validation = false;
                        }
                        else
                        {
                            var manager = _repository.GetQuery<Employee>().FirstOrDefault(a => a.Id == employee.ManagerId && a.Id == loggedinEmployee.Id && a.Active == "Y");
                            if (manager == null)
                            {
                                validationMessages.Add(ERPExceptions.APP_MESSAGES["NOTPERMITTEDAPPROVER"]);
                                validation = false;
                            }
                        }
                        break;
                }

                if (validation)
                {

                    var currentLevel = serviceRequest.CurApprovalLevel;
                    var nextLevel = 0;
                    if (serviceRequest.StatusCode == "SERREQAPPROVED")
                    {
                        var levels = GetNextWorkFlowLevel(currentLevel, serviceRequest.ProdCategoryId);
                        currentLevel = levels.Item2;
                        nextLevel = levels.Item1;
                    }
                    else
                    {
                        nextLevel = -1;
                        currentLevel = -1;
                    }

                    serviceRequest.CurApprovalLevel = currentLevel;
                    serviceRequest.NextApprovalLevel = nextLevel;
                    serviceRequest.ServReqApproval = new List<ServiceReqApproval>();
                    serviceRequest.ServReqApproval.Add(new ServiceReqApproval()
                    {
                        Remarks = serviceRequest.ApproverRemarks,
                        ApprovedDate = DateTime.Now,
                        Active = "Y",
                        ServiceRequestId = serviceRequest.Id,
                        ApprovedBy = _userContext.Id,
                        ApprovalLevel = currentLevel,
                        Status = serviceRequest.StatusCode
                    });

                    InsertUpdateServiceRequest(serviceRequest, saveChanges);
                    if (saveChanges)
                        _repository.SaveChanges();

                    appResponse.Status = APPMessageKey.DATASAVESUCSS;


                }
            }

            if (!validation)
            {
                appResponse.Status = APPMessageKey.ONEORMOREERR;
                appResponse.Messages = validationMessages;
            }
            else
            {
                NotificationsServiceRequest(serviceRequest.Id);
            }




            return appResponse;
        }

        public List<ServiceReqHist> GetServiceReqHistory(Guid serviceReqId)
        {
            var usemaster = _repository.GetQuery<UserMaster>().ToList();
            var serviceReqHist = _repository.GetQuery<ServiceReqHist>().Where(a => a.ServiceRequestId == serviceReqId).OrderByDescending(a => a.CreatedDate).ToList();
            serviceReqHist.ForEach(Hsty =>
            {
                var userName = usemaster.Where(a => a.Id == Hsty.CreatedBy).FirstOrDefault();
                if (userName != null)
                    Hsty.UserName = userName.FirstName + " " + userName.LastName;
            });
            return serviceReqHist;
        }


        public List<ServiceReqComment> GetServiceReqComment(Guid servReqId)
        {
            var comments = _repository.GetQuery<ServiceReqComment>().Where(a => a.ServiceRequestId == servReqId).ToList();
            var commentIds = comments.Select(a => a.Id).Distinct();

            var User = _repository.GetQuery<UserMaster>().ToList();

            var appDoucuments = _repository.GetQuery<AppDocument>().Where(a => a.Active == "Y"
                    && a.TransactionType == "SERVICEREQCOMM" && commentIds.Contains(a.TransactionId)).ToList();

            comments.ForEach(ele =>
            {
                var userName = User.Where(a => a.Id == ele.CreatedBy).FirstOrDefault();
                var appDoucument = appDoucuments.Where(a => a.TransactionId == ele.Id).ToList();
                if (userName != null)
                    ele.UserName = userName.FirstName + " " + userName.LastName;

                if (appDoucument != null)
                {
                    foreach (var doc in appDoucument)
                        doc.FileContent = null;
                    ele.AppDocuments = appDoucument;
                }

            });
            return comments;
        }


        public List<AppDocument> GetServReqAttachments(Guid servReqId)
        {
            var usemaster = _repository.GetQuery<UserMaster>().ToList();

            var comments = _repository.GetQuery<ServiceReqComment>().Where(a => a.ServiceRequestId == servReqId && a.Active == "Y").ToList();

            var fileAttachmentIds = new List<Guid>();
            fileAttachmentIds.AddRange(comments.Select(a => a.Id).Distinct().ToList());
            fileAttachmentIds.Add(servReqId);

            var appDoucuments = _repository.GetQuery<AppDocument>().Where(a => a.Active == "Y"
                    && (a.TransactionType == "SERVICEREQCOMM" || a.TransactionType == "SERVREQUEST") && fileAttachmentIds.Contains(a.TransactionId))
                .OrderByDescending(a => a.CreatedDate).ToList();

            appDoucuments.ForEach(ele =>
            {
                var userName = usemaster.Where(a => a.Id == ele.CreatedBy).FirstOrDefault();
                if (userName != null)
                    ele.UserName = userName.FirstName + " " + userName.LastName;
                ele.FileContent = null;
            });
            return appDoucuments;
        }


        public List<ServiceReqApproval> GetServiceReqStatusHistory(Guid serviceReqId)
        {
            var usemaster = _repository.GetQuery<UserMaster>().Where(a => a.Active == "Y").ToList();
            var approvals = _repository.GetQuery<ServiceReqApproval>().Where(a => a.ServiceRequestId == serviceReqId).OrderByDescending(a => a.CreatedDate).ToList();
            approvals.ForEach(status =>
            {
                var userName = usemaster.Where(a => a.Id == status.CreatedBy).FirstOrDefault();
                if (userName != null)
                    status.UserName = userName.FirstName + " " + userName.LastName;
            });

            return approvals;
        }


        private (int, int) GetNextWorkFlowLevel(int currentLevel, Guid prodCategoryId)
        {
            var approvalLevels = _repository.GetQuery<ProdCategoryWorkFlow>().Where(a => a.ProdCategoryId == prodCategoryId && a.Active == "Y"
                && a.ApprovalLevel > currentLevel).OrderBy(a => a.ApprovalLevel).ToList();
            var nextlevel = currentLevel;
            if (approvalLevels.Count >= 2)
            {
                nextlevel = approvalLevels[1].ApprovalLevel;
                currentLevel = approvalLevels[0].ApprovalLevel;
            }
            else if (approvalLevels.Count == 1)
            {
                nextlevel = approvalLevels[0].ApprovalLevel;
                currentLevel = approvalLevels[0].ApprovalLevel;
            }
            return (nextlevel, currentLevel);
        }
        private void InsertUpdateServiceRequest(ServiceRequest serviceRequest, bool saveChanges)
        {
            var histories = new List<AppAudit>();

            if (serviceRequest.Id == Guid.Empty)
            {
                histories.Add(new AppAudit() { FieldName = "APP_RECORD_CREATED", NewValue = "", OldValue = "" });
                serviceRequest.Id = Guid.NewGuid();
                _repository.Add(serviceRequest, false);
            }
            else
            {
                var OldValue = _repository.GetById<ServiceRequest>(serviceRequest.Id);
                histories = AuditUtility.GetAuditableObject(OldValue, serviceRequest);
                _repository.Update(serviceRequest, false);
            }


            if (histories != null)
            {
                histories.ForEach(Hdr =>
                {
                    var serviceReqDetHist = new ServiceReqHist()
                    {
                        ServiceRequestId = serviceRequest.Id,
                        FieldName = Hdr.FieldName,
                        PrevValue = Hdr.OldValue,
                        CurrentValue = Hdr.NewValue,
                        Active = "Y",
                        Id = Guid.NewGuid()
                    };
                    _repository.Add(serviceReqDetHist);
                });

            }



            if (serviceRequest.ServReqApproval != null)
            {
                foreach (var approval in serviceRequest.ServReqApproval)
                {
                    approval.ServiceRequestId = serviceRequest.Id;
                    InsertUpdateServiceRequestApproval(approval, false);
                }
            }
            if (saveChanges)
                _repository.SaveChanges();


        }

        private void InsertUpdateServiceRequestApproval(ServiceReqApproval approval, bool saveChanges)
        {
            if (approval.Id == Guid.Empty)
            {
                approval.Id = Guid.NewGuid();
                _repository.Add(approval, false);
            }
            else
                _repository.Update(approval, false);

            var comment = new ServiceReqComment()
            {
                Active = "Y",
                Comments = approval.Remarks,
                Id = Guid.NewGuid(),
                ServiceRequestId = approval.ServiceRequestId,
            };
            InsertServiceReqComments(comment, false);

            if (saveChanges)
                _repository.SaveChanges();
        }

        private void InsertServiceReqComments(ServiceReqComment comment, bool saveChanges)
        {
            comment.Id = Guid.NewGuid();
            _repository.Add(comment);

            if (comment.AppDocuments != null)
            {
                foreach (var document in comment.AppDocuments)
                {
                    document.TransactionId = comment.Id;
                    document.TransactionType = "SERVICEREQCOMM";
                    document.Active = "Y";
                }

                var appDocumentBC = new AppDocumentBC(_logger, _repository);
                appDocumentBC.saveAppDocument(comment.AppDocuments, false);
            }
            if (saveChanges)
                _repository.SaveChanges();

        }

        public List<ServiceRequest> GetServiceRequests(ServiceRequestSearch search, bool isExport = false)
        {
            var employees = _repository.GetQuery<Employee>().Where(a => a.Active == "Y").ToList();

            var currEmployee = employees.FirstOrDefault(a => a.Id == _userContext.EmployeeId);
            var languageBC = new LangMasterBC(_logger, _repository);
            var departments = languageBC.GetLangBasedDataForDepartments(_userContext.Language);

            var serReqQuery = _repository.GetQuery<ServiceRequest>();
            var serReqApprovalQuery = _repository.GetQuery<ServiceReqApproval>();
            var productQry = _repository.GetQuery<ProductMaster>();
            var prodCategoryQry = _repository.GetQuery<ProductCategory>();
            var prodSubCategoryQry = _repository.GetQuery<ProdSubCategory>();
            var prodCateWfQry = _repository.GetQuery<ProdCategoryWorkFlow>();
            var userMsterQry = _repository.GetQuery<UserMaster>();
            var employeeQry = _repository.GetQuery<Employee>();

            //this is required to avoid performnace issue to avoid loading all the data with out date filter
            if (search.FromTransDate <= DateTime.MinValue && search.ToTransDate <= DateTime.MinValue && string.IsNullOrEmpty(search.TransNo) && search.EmployeeId == Guid.Empty)
            {
                search.FromTransDate = DateTime.Now.AddMonths(-1);
                search.ToTransDate = DateTime.Now;
            }

            search.FromTransDate = search.FromTransDate.ToLocalTime().Date;
            search.ToTransDate = search.ToTransDate.ToLocalTime().Date;
            //This list should return only if the logged in employee is creator or he is part of any of the approver in the workflow.
            //Approver should be part of next approver (manager, department staff, department head  or specified user in the approver list
            var serviceRequestQuery =
                    (
                        from sr in serReqQuery
                        join sra in serReqApprovalQuery on sr.Id equals sra.ServiceRequestId
                        join cate in prodCategoryQry on sr.ProdCategoryId equals cate.Id
                        join wf in prodCateWfQry on cate.Id equals wf.ProdCategoryId
                        join crusr in userMsterQry on sr.CreatedBy equals crusr.Id
                        join emp in employeeQry on crusr.EmployeeId equals emp.Id into cremployee
                        from cremp in cremployee.DefaultIfEmpty()
                        where sr.Active == "Y"
                        && sra.Active == "Y"
                        && cate.Active == "Y"
                        && wf.Active == "Y"
                        && crusr.Active == "Y"
                        && (string.IsNullOrEmpty(search.TransNo) || sr.TransNo == search.TransNo)
                        && (search.EmployeeId == Guid.Empty || sr.EmployeeId == search.EmployeeId)
                        && (search.FromTransDate <= DateTime.MinValue || sr.TransDate >= search.FromTransDate)
                        && (search.ToTransDate <= DateTime.MinValue || sr.TransDate <= search.ToTransDate)
                        && (string.IsNullOrEmpty(search.Status)
                                || (search.Status == "SERREQCOMPLETE" && sr.NextApprovalLevel == sr.CurApprovalLevel && sr.CurApprovalLevel != -1)
                                || (search.Status == "SERREQREJECTED" && sr.CurApprovalLevel == -1)
                                || (search.Status == "SERREQPENDING" && sr.CurApprovalLevel != sr.NextApprovalLevel))
                          && (sr.CreatedBy == _userContext.Id || sra.ApprovedBy == _userContext.Id
                            || (wf.ApprovalType == "DEPARTMENT" && currEmployee.CurrDepartmentId == wf.ApprovalId && sr.NextApprovalLevel == wf.ApprovalLevel)
                            || (wf.ApprovalType == "DEPARTMENTHEAD" && currEmployee.CurrDepartmentId == wf.ApprovalId && currEmployee.IsDepratmentHead && sr.NextApprovalLevel == wf.ApprovalLevel)
                            || (wf.ApprovalType == "EMPLOYEEMANAGER" && cremp.ManagerId == currEmployee.Id && sr.NextApprovalLevel == wf.ApprovalLevel)
                            || (wf.ApprovalType == "USER" && wf.ApprovalId == currEmployee.Id && sr.NextApprovalLevel == wf.ApprovalLevel)
                            )

                        select new
                        {
                            ServiceRequest = sr,
                            ServReqApp = sra,
                            WorkFlow = wf,
                            Category = cate,
                            CreatedEmp = cremp,
                            CreatedUsr = crusr
                        });
            var qryResult = serviceRequestQuery.OrderByDescending(a=>a.ServiceRequest.TransNo).ToList();

            var result = new List<ServiceRequest>();

            var groupedRes = qryResult.GroupBy(u => u.ServiceRequest.Id).ToList();
            foreach (var grp in groupedRes)
            {
                var serReq = new ServiceRequest();

                serReq = grp.First().ServiceRequest;
                serReq.ServReqApproval = new List<ServiceReqApproval>();
                serReq.CanApproveReject = false;

                if (serReq.CurApprovalLevel == -1)
                    serReq.StatusText = ERPExceptions.APP_MESSAGES["STATUSREJECTED"];
                else if (serReq.NextApprovalLevel == serReq.CurApprovalLevel)
                    serReq.StatusText = ERPExceptions.APP_MESSAGES["STATUSCOMPLETED"];
                else
                {
                    var wf = grp.FirstOrDefault(a => a.WorkFlow.ApprovalLevel == serReq.NextApprovalLevel).WorkFlow;
                    var CrEmp = grp.FirstOrDefault(a => a.CreatedUsr.Id == serReq.CreatedBy).CreatedEmp;

                    var approvalInfo = getApprovalStatusText(wf, CrEmp.ManagerId, departments, employees);
                    serReq.StatusText = approvalInfo.Item1;
                    if (approvalInfo.Item2 && _userContext.Id != serReq.CreatedBy)
                        serReq.CanApproveReject = true;

                }

                foreach (var req in grp)
                {

                    if (!serReq.ServReqApproval.All(a => a.Id == req.ServReqApp.Id))
                        serReq.ServReqApproval.Add(req.ServReqApp);
                }
                result.Add(serReq);
            }
            var unitmaster = _repository.GetQuery<ProdUnitMaster>();
            result.ForEach(p =>
            {
                p.ProdSubCategoryName = prodSubCategoryQry.FirstOrDefault(a => a.Id == p.ProdSubCategoryId)?.Name;
                p.EmployeeName = employeeQry.FirstOrDefault(a => a.Id == p.EmployeeId)?.FullNameEng;
                p.ProdCategoryName = prodCategoryQry.FirstOrDefault(a => a.Id == p.ProdCategoryId)?.Name;
                if (p.CurApprovalLevel == p.NextApprovalLevel && p.NextApprovalLevel != -1)
                {
                    p.ProductName = productQry.FirstOrDefault(a => a.Id == p.ProductMasterId && a.Active == "Y")?.ProdDescription;
                }
                p.UnitName = unitmaster.FirstOrDefault(a => a.Id == p.UnitMasterId && a.Active == "Y")?.UnitName;
            });
            if (isExport)
            {
                foreach (var det in result)
                {
                    det.ServReqApproval.Add(new ServiceReqApproval
                    {
                        ProdCategoryName = det?.ProdCategoryName,
                        ProdSubCategoryName = det?.ProdSubCategoryName,
                        ProductName = det?.ProductName,
                        UnitName = det?.UnitName,
                        Quantitys = det.Quantity,
                    });
                }

            }
            return result;
        }


        public List<ServiceRequest> GetServRequestsWithServiceDepartment()
        {
            var result = new List<ServiceRequest>();

            var languageBC = new LangMasterBC(_logger, _repository);
            var departments = languageBC.GetLangBasedDataForDepartments("en");
            var servDepartment = departments.FirstOrDefault(a => a.Type == "SERVICEDEP");
            if (servDepartment != null)
            {
                var serReqQuery = _repository.GetQuery<ServiceRequest>();
                var prodCategoryQry = _repository.GetQuery<ProductCategory>();
                var prodCateWfQry = _repository.GetQuery<ProdCategoryWorkFlow>();

                var serviceRequestQuery =
                        (
                            from sr in serReqQuery
                            join cate in prodCategoryQry on sr.ProdCategoryId equals cate.Id
                            join wf in prodCateWfQry on cate.Id equals wf.ProdCategoryId
                            where sr.Active == "Y"
                            && cate.Active == "Y"
                            && wf.Active == "Y"
                            && sr.CurApprovalLevel != sr.NextApprovalLevel
                            && sr.CurApprovalLevel != -1
                            && sr.NextApprovalLevel == wf.ApprovalLevel
                            && wf.ApprovalId == servDepartment.Id
                            && wf.ApprovalType == "DEPARTMENT"
                            select sr);
                result = serviceRequestQuery.ToList();
            }
            return result;
        }

        private (string, bool) getApprovalStatusText(ProdCategoryWorkFlow wf, Guid managerId, List<Department> departments, List<Employee> employees)
        {
            var status = "";
            var canApprove = false;
            switch (wf.ApprovalType)
            {
                case "DEPARTMENTHEAD":
                    var appDepart = departments.FirstOrDefault(a => a.Id == wf.ApprovalId);
                    if (appDepart != null)
                    {
                        var depHead = employees.FirstOrDefault(a => a.CurrDepartmentId == appDepart.Id && a.IsDepratmentHead);
                        if (depHead != null)
                        {
                            status = string.Format(ERPExceptions.APP_MESSAGES["WFPENDINGWITH"], _userContext.Language == "en" ? depHead.FullNameEng : depHead.FullNameArb);
                            if (depHead.Id == _userContext.EmployeeId)
                                canApprove = true;
                        }
                    }
                    break;
                case "DEPARTMENT":
                    var department = departments.FirstOrDefault(a => a.Id == wf.ApprovalId);
                    if (department != null)
                    {
                        status = string.Format(ERPExceptions.APP_MESSAGES["WFPENDINGWITH"], department.Name);
                        var curEmployee = employees.FirstOrDefault(a => a.Id == _userContext.EmployeeId);
                        if (curEmployee != null && curEmployee.CurrDepartmentId == department.Id)
                            canApprove = true;
                    }
                    break;
                case "USER":
                    var employee = employees.FirstOrDefault(a => a.Id == wf.ApprovalId);
                    if (employee != null)
                    {
                        status = string.Format(ERPExceptions.APP_MESSAGES["WFPENDINGWITH"], _userContext.Language == "en" ? employee.FullNameEng : employee.FullNameArb);
                        if (_userContext.EmployeeId == employee.Id)
                            canApprove = true;
                    }
                    break;
                case "EMPLOYEEMANAGER":
                    var manager = employees.FirstOrDefault(a => a.Id == managerId);
                    if (manager != null)
                    {
                        status = string.Format(ERPExceptions.APP_MESSAGES["WFPENDINGWITH"], _userContext.Language == "en" ? manager.FullNameEng : manager.FullNameArb);
                        if (_userContext.EmployeeId == manager.Id)
                            canApprove = true;
                    }
                    break;
            }

            return (status, canApprove);
        }
        public List<ServiceRequest> GetServRequestsWithIdDepartment()
        {
            var result = new List<ServiceRequest>();

            var languageBC = new LangMasterBC(_logger, _repository);
            var departments = languageBC.GetLangBasedDataForDepartments("en");
            var servDepartment = departments.FirstOrDefault(a => a.Type == "ITDEPRATMENT");
            if (servDepartment != null)
            {
                var serReqQuery = _repository.GetQuery<ServiceRequest>();
                var prodCategoryQry = _repository.GetQuery<ProductCategory>();
                var prodCateWfQry = _repository.GetQuery<ProdCategoryWorkFlow>();

                var serviceRequestQuery =
                        (
                            from sr in serReqQuery
                            join cate in prodCategoryQry on sr.ProdCategoryId equals cate.Id
                            join wf in prodCateWfQry on cate.Id equals wf.ProdCategoryId
                            where sr.Active == "Y"
                            && cate.Active == "Y"
                            && wf.Active == "Y"
                            && sr.CurApprovalLevel != sr.NextApprovalLevel
                            && sr.CurApprovalLevel != -1
                            && sr.NextApprovalLevel == wf.ApprovalLevel
                            && wf.ApprovalId == servDepartment.Id
                            && wf.ApprovalType == "DEPARTMENT"
                            select sr);
                result = serviceRequestQuery.ToList();
            }
            return result;
        }
        public List<ServiceRequest> GetServRequestsWithProdConfiguration()
        {
            return _repository.GetQuery<ServiceRequest>().Where(a => a.CurApprovalLevel == a.NextApprovalLevel &&
            a.Active == "Y").ToList();
        }




        public void NotificationsServiceRequest(Guid Id)
        {

            var employees = _repository.GetQuery<Employee>().Where(a => a.Active == "Y").ToList();
            var currEmployee = employees.FirstOrDefault(a => a.Id == _userContext.EmployeeId);
            var languageBC = new LangMasterBC(_logger, _repository);
            var departments = languageBC.GetLangBasedDataForDepartments(_userContext.Language);
            var serReqQuery = _repository.GetQuery<ServiceRequest>().Where(a => a.Id == Id && a.Active == "Y").ToList();
            var serReqApprovalQuery = _repository.GetQuery<ServiceReqApproval>();
            var productQry = _repository.GetQuery<ProductMaster>();
            var prodCategoryQry = _repository.GetQuery<ProductCategory>();
            var prodCateWfQry = _repository.GetQuery<ProdCategoryWorkFlow>();
            var userMsterQry = _repository.GetQuery<UserMaster>();
            var employeeQry = _repository.GetQuery<Employee>();
            string text = "";
            var serviceRequestQuery =
                             (
                                 from sr in serReqQuery
                                 join sra in serReqApprovalQuery on sr.Id equals sra.ServiceRequestId
                                 join cate in prodCategoryQry on sr.ProdCategoryId equals cate.Id
                                 join wf in prodCateWfQry on cate.Id equals wf.ProdCategoryId
                                 join crusr in userMsterQry on sr.CreatedBy equals crusr.Id
                                 join dep in departments on wf.ApprovalId equals dep.Id
                                 join emp in employeeQry on dep.Id equals emp.CurrDepartmentId
                                 where


                                   sra.Active == "Y"
                                 && cate.Active == "Y"
                                 && wf.Active == "Y"
                                 && crusr.Active == "Y"
                                 && wf.ApprovalType == "DEPARTMENT"
                                 && sr.NextApprovalLevel == wf.ApprovalLevel
                                 && dep.Code == "SERVICEDEP"
                                     && sr.CurApprovalLevel != -1
                                 select new
                                 {
                                     employeeID = emp.Id,
                                     ServiceReqTran = sr.TransNo,
                                     EmployeeArbName = emp.FullNameArb,
                                     EmployeeEngName = emp.FullNameEng,
                                     EmployeePhoneNumber = emp.PhoneNumber,
                                     sr.NextApprovalLevel,
                                     sr.CurApprovalLevel,
                                     RequestEmployeeId = sr.EmployeeId

                                 });

            var qryResult = serviceRequestQuery.ToList();
            var requestEmployee = qryResult.Select(a => a.RequestEmployeeId);
            if (qryResult != null && qryResult.Count > 0)
            {
                if (qryResult.Select(x => x.NextApprovalLevel).FirstOrDefault() == qryResult.Select(x => x.CurApprovalLevel).FirstOrDefault())
                {
                    text = "Your Service Order " + qryResult.Select(x => x.ServiceReqTran).FirstOrDefault() + " request has been approved from all levels";
                    SMSUtility.SendSMSService(text, employeeQry.FirstOrDefault(x => requestEmployee.Contains(x.Id)).PhoneNumber);
                }
                else
                {
                    List<Guid?> EmployeeIds = null;

                    foreach (var item in qryResult)
                    {
                        EmployeeIds.Add(item.employeeID);

                    }

                    AppAccessBC ap = new AppAccessBC(_logger);
                    List<ApprovalAccessDetails> approvalAccessDetails = ap.GetApprovalAccessDetail(EmployeeIds, "SCR_SERVICE_REQUEST");

                    foreach (var employee in approvalAccessDetails)
                    {
                        text = "Service Order " + qryResult.Select(x => x.ServiceReqTran).FirstOrDefault() + " is pending for your approval";
                        SMSUtility.SendSMSService(text, employee.PhoneNumber);
                    }

                }

            }

        }
        public AppResponse UpdateTransNoSeqNo()
        {
            var appResponse = new AppResponse();
            var currentYear = DateTime.Now.Year.ToString();
            long SeqNo = 0;
            List<ServiceRequest> serviceRequests = new List<ServiceRequest>();
            var serviceRequestList = _repository.GetQuery<ServiceRequest>().ToList();
            SeqNo = 0;
            if (serviceRequestList != null && serviceRequestList.Count > 0)
            {
                SeqNo = serviceRequestList.Max(a=>a.SeqNo) + 1;
                serviceRequestList.ForEach(serviceReq =>
                {
                    if (serviceReq.TransNo == null || serviceReq.SeqNo == 0)
                    {
                        serviceReq.TransNo = string.Format("{0}1{1}", currentYear, SeqNo.ToString("00000#"));
                        serviceReq.SeqNo = SeqNo;
                        serviceRequests.Add(serviceReq);
                        SeqNo += 1;
                    }
                });
            }
            if (serviceRequests.Count > 0 && serviceRequests != null)
            {
                foreach (var serviceRequest in serviceRequests)
                {
                    _repository.Update(serviceRequest, false);
                }
            }
            List<QuotationRequest> quotationRequests = new List<QuotationRequest>();
            var quotationReqList = _repository.GetQuery<QuotationRequest>().ToList();
            SeqNo = 0;
            if (quotationReqList != null && quotationReqList.Count > 0)
            {
                SeqNo = quotationReqList.Max(a => a.SeqNo) + 1;
                quotationReqList.ForEach(quoReq =>
                {
                    if (quoReq.TransNo == null || quoReq.SeqNo == 0)
                    {
                        quoReq.TransNo = string.Format("{0}2{1}", currentYear, SeqNo.ToString("00000#"));
                        quoReq.SeqNo = SeqNo;
                        quotationRequests.Add(quoReq);
                        SeqNo += 1;
                    }
                });
            }
            if (quotationRequests.Count > 0 && quotationRequests != null)
            {
                foreach (var quotationRequest in quotationRequests)
                {
                    _repository.Update(quotationRequest, false);
                }
            }
            List<VendorQuotation> vendorQuotations = new List<VendorQuotation>();
            var vendorQuotationList = _repository.GetQuery<VendorQuotation>().ToList();
            SeqNo = 0;
            if (vendorQuotationList != null && vendorQuotationList.Count > 0)
            {
                SeqNo = vendorQuotationList.Max(a => a.SeqNo) + 1;
                vendorQuotationList.ForEach(vendorQuo =>
                {
                    if (vendorQuo.TransNo == null || vendorQuo.SeqNo == 0)
                    {
                        vendorQuo.TransNo = string.Format("{0}3{1}", currentYear, SeqNo.ToString("00000#"));
                        vendorQuo.SeqNo = SeqNo;
                        vendorQuotations.Add(vendorQuo);
                        SeqNo += 1;
                    }
                });
            }
            if (vendorQuotations.Count > 0 && vendorQuotations != null)
            {
                foreach (var vendorQuotation in vendorQuotations)
                {
                    _repository.Update(vendorQuotation, false);
                }
            }
            List<PurchaseRequest> purchaseRequests = new List<PurchaseRequest>();
            var purchaseRequestList = _repository.GetQuery<PurchaseRequest>().ToList();
            SeqNo = 0;
            if (purchaseRequestList != null && purchaseRequestList.Count > 0)
            {
                SeqNo = purchaseRequestList.Max(a => a.SeqNo) + 1;
                purchaseRequestList.ForEach(purReq =>
                {
                    if (purReq.TransNo == null || purReq.SeqNo == 0)
                    {
                        purReq.TransNo = string.Format("{0}4{1}", currentYear, SeqNo.ToString("00000#"));
                        purReq.SeqNo = SeqNo;
                        purchaseRequests.Add(purReq);
                        SeqNo += 1;
                    }
                });
            }
            if (purchaseRequests.Count > 0 || purchaseRequests != null)
            {
                foreach (var purchaseRequest in purchaseRequests)
                {
                    _repository.Update(purchaseRequest, false);
                }
            }
            List<PurchaseOrder> purchaseOrders = new List<PurchaseOrder>();
            var purchaseOrderList = _repository.GetQuery<PurchaseOrder>().ToList();
            SeqNo = 0;
            if (purchaseOrderList != null && purchaseOrderList.Count > 0)
            {
                SeqNo = purchaseOrderList.Max(a => a.SeqNo) + 1;
                purchaseOrderList.ForEach(purOrder =>
                {
                    if (purOrder.TransNo == null || purOrder.SeqNo == 0)
                    {
                        purOrder.TransNo = string.Format("{0}5{1}", currentYear, SeqNo.ToString("00000#"));
                        purOrder.SeqNo = SeqNo;
                        purchaseOrders.Add(purOrder);
                        SeqNo += 1;
                    }
                });
            }
            if (purchaseOrders.Count > 0 || purchaseOrders != null)
            {
                foreach (var purchaseOrder in purchaseOrders)
                {
                    _repository.Update(purchaseOrder, false);
                }
            }
            List<GoodsRecNote> goodsRecNotes = new List<GoodsRecNote>();
            var goodsRecNoteList = _repository.GetQuery<GoodsRecNote>().ToList();
            SeqNo = 0;
            if (goodsRecNoteList != null && goodsRecNoteList.Count > 0)
            {
                SeqNo = goodsRecNoteList.Max(a => a.SeqNo) + 1;
                goodsRecNoteList.ForEach(grn =>
                {
                    if (grn.TransNo == null || grn.SeqNo == 0)
                    {
                        grn.TransNo = string.Format("{0}6{1}", currentYear, SeqNo.ToString("00000#"));
                        grn.SeqNo = SeqNo;
                        goodsRecNotes.Add(grn);
                        SeqNo += 1;
                    }
                });
            }
            if (goodsRecNotes.Count > 0 && goodsRecNotes != null)
            {
                foreach (var goodsRecNote in goodsRecNotes)
                {
                    _repository.Update(goodsRecNote, false);
                }
            }
            List<ProdInvIssue> prodInvIssues = new List<ProdInvIssue>();
            var prodInvIssueList = _repository.GetQuery<ProdInvIssue>().ToList();
            SeqNo = 0;
            if (prodInvIssueList != null && prodInvIssueList.Count > 0)
            {
                SeqNo = prodInvIssueList.Max(a => a.SeqNo) + 1;
                prodInvIssueList.ForEach(invIssue =>
                {
                    if (invIssue.TransNo == null || invIssue.SeqNo == 0)
                    {
                        invIssue.TransNo = string.Format("{0}7{1}", currentYear, SeqNo.ToString("00000#"));
                        invIssue.SeqNo = SeqNo;
                        prodInvIssues.Add(invIssue);
                        SeqNo += 1;
                    }
                });
            }
            if (prodInvIssues.Count > 0 && prodInvIssues != null)
            {
                foreach (var prodInvIssue in prodInvIssues)
                {
                    _repository.Update(prodInvIssue, false);
                }
            }
            List<InventoryTransfer> inventoryTransfers = new List<InventoryTransfer>();
            var inventoryTransferList = _repository.GetQuery<InventoryTransfer>().ToList();
            SeqNo = 0;
            if (inventoryTransferList != null && inventoryTransferList.Count > 0)
            {
                SeqNo = inventoryTransferList.Max(a => a.SeqNo) + 1;
                inventoryTransferList.ForEach(invTransfer =>
                {
                    if (invTransfer.TransNo == null || invTransfer.SeqNo == 0)
                    {
                        invTransfer.TransNo = string.Format("{0}8{1}", currentYear, SeqNo.ToString("00000#"));
                        invTransfer.SeqNo = SeqNo;
                        inventoryTransfers.Add(invTransfer);
                        SeqNo += 1;
                    }
                });
            }
            if (inventoryTransfers.Count > 0 && inventoryTransfers != null)
            {
                foreach (var inventoryTransfer in inventoryTransfers)
                {
                    _repository.Update(inventoryTransfer, false);
                }
            }
            _repository.SaveChanges();
            appResponse.Status = APPMessageKey.DATASAVESUCSS;
            return appResponse;
        }
        public List<ServiceRequest> GetServRequestsWithApprovalServiceDepartment()
        {
            var result = new List<ServiceRequest>();

            var languageBC = new LangMasterBC(_logger, _repository);
            var departments = languageBC.GetLangBasedDataForDepartments("en");
            var servDepartment = departments.FirstOrDefault(a => a.Type == "SERVICEDEP");
            if (servDepartment != null)
            {
                var serReqQuery = _repository.GetQuery<ServiceRequest>();
                var prodCategoryQry = _repository.GetQuery<ProductCategory>();
                var prodCateWfQry = _repository.GetQuery<ProdCategoryWorkFlow>();

                var serviceRequestQuery =
                        (
                            from sr in serReqQuery
                            join cate in prodCategoryQry on sr.ProdCategoryId equals cate.Id
                            join wf in prodCateWfQry on cate.Id equals wf.ProdCategoryId
                            where sr.Active == "Y"
                            && cate.Active == "Y"
                            && wf.Active == "Y"
                            && sr.CurApprovalLevel == sr.NextApprovalLevel
                            && sr.CurApprovalLevel != -1
                            && sr.NextApprovalLevel == wf.ApprovalLevel
                            && wf.ApprovalId == servDepartment.Id
                            && wf.ApprovalType == "DEPARTMENT"
                            select sr);
                result = serviceRequestQuery.ToList();
            }
            return result;
        }


    }
}