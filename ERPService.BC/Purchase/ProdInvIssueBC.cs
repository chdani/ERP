using ERPService.BC.Utility;
using ERPService.Common;
using ERPService.Common.Interfaces;
using ERPService.Common.Shared;
using ERPService.DataModel.CTO;
using ERPService.DataModel.DTO;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERPService.BC
{
    public class ProdInvIssueBC
    {
        private ILogger _logger;
        private IRepository _repository;
        private UserContext _userContext;

        public ProdInvIssueBC(ILogger logger, IRepository repository, UserContext userContext)
        {
            _logger = logger;
            _repository = repository;
            _userContext = userContext;
        }

        public ProdInvIssue GetProdInvIssueById(Guid requestId)
        {
            var prodInvIssue = _repository.GetById<ProdInvIssue>(requestId);
            if (prodInvIssue != null)
                prodInvIssue.ProdInvIssueDet = GetProdInvIssueDetByHdrId(requestId);
            return prodInvIssue;
        }

        public List<ProdInvIssueDet> GetProdInvIssueDetByHdrId(Guid hdrId)
        {
            return _repository.GetQuery<ProdInvIssueDet>().Where(a => a.Active == "Y" && a.ProdInvIssueId == hdrId).ToList(); ;
        }
        public AppResponse SaveProdInvIssue(ProdInvIssue prodInvIssue)
        {
            List<String> validationMessages = new List<string>();
            AppResponse appResponse = new AppResponse();
            bool validation = true;

            if (prodInvIssue.TransDate <= DateTime.MinValue)
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.MANDMISSING]);
                validation = false;
            }
            if (prodInvIssue.Active == "Y")
            {
                if (prodInvIssue.ProdInvIssueDet == null || prodInvIssue.ProdInvIssueDet.Count == 0)
                {
                    validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.NOTDETROWEXIST]);
                    validation = false;
                }
            }
            if (prodInvIssue.Id == Guid.Empty)
            {
                var IssueTransactionTransNoAndSeqNo = AppGeneralMethods.TranstypeSeqNumber("IssueTransactionTransType", _repository);
                prodInvIssue.TransNo = IssueTransactionTransNoAndSeqNo.Item1;
                prodInvIssue.SeqNo = IssueTransactionTransNoAndSeqNo.Item2;
            }

            if (prodInvIssue != null && validation)
            {

                prodInvIssue.TransDate = prodInvIssue.TransDate.ToLocalTime().Date;
                prodInvIssue.Status = "PURTRNSTSSUBMITTED";

                InsertUpdateProdInvIssue(prodInvIssue);
                appResponse.Status = APPMessageKey.DATASAVESUCSS;
            }
            else
            {
                appResponse.Status = APPMessageKey.ONEORMOREERR;
                appResponse.Messages = validationMessages;
            }
            return appResponse;
        }


        public AppResponse ApproveProdInvIssue(ProdInvIssue prodInvIssue)
        {
            List<String> validationMessages = new List<string>();
            AppResponse appResponse = new AppResponse();
            bool validation = true;

            if (prodInvIssue.Id == Guid.Empty || string.IsNullOrEmpty(prodInvIssue.ApproverRemarks))
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.MANDMISSING]);
                validation = false;
            }

            var prodIssueDets = GetProdInvIssueDetByHdrId(prodInvIssue.Id);
            if (prodIssueDets == null || prodIssueDets.Count <= 0)
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.MANDMISSING]);
                validation = false;
            }

            if (validation)
            {
                var statusHistory = new ProdInvIssueStatusHist()
                {
                    Active = "Y",
                    Comments = prodInvIssue.ApproverRemarks,
                    Status = prodInvIssue.Status,
                    ProdInvIssueId = prodInvIssue.Id,
                };

                prodInvIssue.ProdInvIssueDet = null;

                var inventories = new List<ProductInventory>();
                if (prodInvIssue.Status == "PURTRNSTSAPPROVED")
                {

                    var balanceStockBC = new ProductInventoryBC(_logger, _repository);
                    var input = new ProdInventoryBalance() { WareHouseLocationId = prodIssueDets[0].WareHouseLocationId };
                    var stock = balanceStockBC.GetProdInventoryBalance(input);
                    if (stock != null && stock.Count > 0)
                    {
                        var units = _repository.GetQuery<ProdUnitMaster>();

                        foreach (var det in prodIssueDets)
                        {
                            var unit = units.FirstOrDefault(a => a.Id == det.UnitMasterId);
                            decimal convertQty = 1;
                            if (unit != null)
                                convertQty = unit.ConversionUnit;

                            var prodStock = stock.Where(a => a.ProductMasterId == det.ProductMasterId).ToList();
                            if (prodStock.Sum(a => a.AvlQuantity) < (det.Quantity * convertQty))
                            {
                                validation = false;
                                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.NOSTOCK]);
                                break;
                            }
                            else
                            {
                                var balanceQty = det.Quantity * convertQty;
                                var prodInvIssueTransno = long.Parse(prodInvIssue.TransNo);
                                foreach (var stk in prodStock)
                                {
                                    var reduceQty = stk.AvlQuantity > balanceQty ? balanceQty : stk.AvlQuantity;

                                    balanceQty -= reduceQty;
                                    inventories.Add(new ProductInventory
                                    {
                                        Active = "Y",
                                        TransDate = prodInvIssue.TransDate,
                                        ProductMasterId = det.ProductMasterId,
                                        StockOut = reduceQty,
                                        TransNo = prodInvIssueTransno,
                                        ActorType = "EMPLOYEE",
                                        ActorId = prodInvIssue.EmployeeId,
                                        TransId = det.Id,
                                        TransType = "ISSUEDET",
                                        ExpiryDate = stk.ExpiryDate,
                                        WareHouseLocationId = stk.WareHouseLocationId,
                                        Id = Guid.NewGuid()
                                    });
                                    if (balanceQty <= 0)
                                        break;
                                }
                            }
                        }
                    }
                    else
                    {
                        validation = false;
                        validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.NOSTOCK]);
                    }
                }
                if (validation)
                {
                    if (prodInvIssue.Status == "PURTRNSTSAPPROVED" && prodInvIssue.Type == "INVISSUEEMP")
                    {
                        var servReq = _repository.GetById<ServiceRequest>(prodInvIssue.ServiceRequestId);
                        servReq.StatusCode = "SERREQAPPROVED";
                        servReq.ApproverRemarks = prodInvIssue.ApproverRemarks;

                        var servReqBC = new ServiceRequestBC(_logger, _repository, _userContext);
                        var response = servReqBC.ApproveServiceRequest(servReq, false);
                        if (response.Status != APPMessageKey.DATASAVESUCSS)
                        {
                            validation = false;
                            validationMessages.AddRange(response.Messages);
                        }
                    }

                    if (validation)
                    {
                        if (inventories.Count > 0)
                        {
                            foreach (var inv in inventories)
                                _repository.Add(inv, false);
                        }
                        _repository.Update(prodInvIssue, false);

                        InsertUpdateProdInvIssueApproval(statusHistory, false);
                        _repository.SaveChanges();
                        appResponse.Status = APPMessageKey.DATASAVESUCSS;
                    }
                }
            }
            if (!validation)
            {
                appResponse.Status = APPMessageKey.ONEORMOREERR;
                appResponse.Messages = validationMessages;
            }
            return appResponse;
        }

        public AppResponse SaveProdInvIssueComment(ProdInvIssueComment prodInvIssueComment)
        {
            InsertProdInvIssueComments(prodInvIssueComment, true);
            return new AppResponse() { Status = APPMessageKey.DATASAVESUCSS };
        }

        public AppResponse SaveProdInvIssueDetComment(ProdInvIssueDetComment prodInvIssueDetComment)
        {
            InsertProdInvIssueDetComments(prodInvIssueDetComment, true);
            return new AppResponse() { Status = APPMessageKey.DATASAVESUCSS };
        }

        public List<ProdInvIssueHist> GetProdInvIssueHistory(Guid prodInvIssueId)
        {
            var usemaster = _repository.GetQuery<UserMaster>().ToList();
            var prodInvIssueHist = _repository.GetQuery<ProdInvIssueHist>().Where(a => a.ProdInvIssueId == prodInvIssueId).OrderByDescending(a => a.CreatedDate).ToList();
            prodInvIssueHist.ForEach(Hsty =>
            {
                var userName = usemaster.Where(a => a.Id == Hsty.CreatedBy).FirstOrDefault();
                if (userName != null)
                    Hsty.UserName = userName.FirstName + " " + userName.LastName;
            });
            return prodInvIssueHist.OrderByDescending(a => a.CreatedDate).ToList();
        }

        public List<ProdInvIssueDetHist> GetProdInvIssueDetHistory(Guid prodInvIssueDetId)
        {
            var usemaster = _repository.GetQuery<UserMaster>().ToList();
            var prodInvIssueDetHist = _repository.GetQuery<ProdInvIssueDetHist>().Where(a => a.ProdInvIssueDetId == prodInvIssueDetId).OrderByDescending(a => a.CreatedDate).ToList();
            prodInvIssueDetHist.ForEach(Hsty =>
            {
                var userName = usemaster.Where(a => a.Id == Hsty.CreatedBy).FirstOrDefault();
                if (userName != null)
                    Hsty.UserName = userName.FirstName + " " + userName.LastName;
            });
            return prodInvIssueDetHist.OrderByDescending(a => a.CreatedDate).ToList();
        }

        public List<ProdInvIssueComment> GetProdInvIssueComment(Guid prodInvIssueId)
        {
            var comments = _repository.GetQuery<ProdInvIssueComment>().Where(a => a.ProdInvIssueId == prodInvIssueId).ToList();
            var commentIds = comments.Select(a => a.Id).Distinct();

            var User = _repository.GetQuery<UserMaster>().ToList();

            var appDoucuments = _repository.GetQuery<AppDocument>().Where(a => a.Active == "Y"
                    && a.TransactionType == "PRODINVISSCOMM" && commentIds.Contains(a.TransactionId)).ToList();

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
            return comments.OrderByDescending(a => a.CreatedDate).ToList();
        }

        public List<ProdInvIssueDetComment> GetProdInvIssueDetComment(Guid prodInvIssueId)
        {
            var comments = _repository.GetQuery<ProdInvIssueDetComment>().Where(a => a.ProdInvIssueDetId == prodInvIssueId).ToList();
            var commentIds = comments.Select(a => a.Id).Distinct();

            var User = _repository.GetQuery<UserMaster>().ToList();

            var appDoucuments = _repository.GetQuery<AppDocument>().Where(a => a.Active == "Y"
                    && a.TransactionType == "PRODINVISSDETCOMM" && commentIds.Contains(a.TransactionId)).ToList();

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
            return comments.OrderByDescending(a => a.CreatedDate).ToList();
        }

        public List<AppDocument> GetProdInvIssuesAttachments(Guid prodInvIssueId)
        {
            var usemaster = _repository.GetQuery<UserMaster>().ToList();

            var comments = _repository.GetQuery<ProdInvIssueComment>().Where(a => a.ProdInvIssueId == prodInvIssueId && a.Active == "Y").ToList();

            var fileAttachmentIds = new List<Guid>();
            fileAttachmentIds.AddRange(comments.Select(a => a.Id).Distinct().ToList());
            fileAttachmentIds.Add(prodInvIssueId);

            var appDoucuments = _repository.GetQuery<AppDocument>().Where(a => a.Active == "Y"
                    && (a.TransactionType == "PRODINVISSCOMM" || a.TransactionType == "PRODINVISS") && fileAttachmentIds.Contains(a.TransactionId))
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

        public List<ProdInvIssueStatusHist> GetProdInvIssueStatusHistory(Guid prodInvIssueId)
        {
            var usemaster = _repository.GetQuery<UserMaster>().Where(a => a.Active == "Y").ToList();
            var approvals = _repository.GetQuery<ProdInvIssueStatusHist>().Where(a => a.ProdInvIssueId == prodInvIssueId).OrderByDescending(a => a.CreatedDate).ToList();
            approvals.ForEach(status =>
            {
                var userName = usemaster.Where(a => a.Id == status.CreatedBy).FirstOrDefault();
                if (userName != null)
                    status.UserName = userName.FirstName + " " + userName.LastName;
            });

            return approvals.OrderByDescending(a => a.CreatedDate).ToList();
        }
        private List<string> ValidateChildRecords(List<ProdInvIssueDet> prodInvIssueDet, out bool validation)
        {
            var validationMessages = new List<string>();
            validation = true;
            foreach (var det in prodInvIssueDet)
            {
                if (det.ProductMasterId == Guid.Empty
                    || det.Quantity <= 0
                    || det.UnitMasterId == Guid.Empty
                    || det.WareHouseLocationId == Guid.Empty)
                {
                    validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.MANDMISSING]);
                    validation = false;
                }
            }

            return validationMessages;
        }

        private void InsertUpdateProdInvIssue(ProdInvIssue prodInvIssue)
        {
            var histories = new List<AppAudit>();

            if (prodInvIssue.Id == Guid.Empty)
            {
                histories.Add(new AppAudit() { FieldName = "APP_RECORD_CREATED", NewValue = "", OldValue = "" });
                prodInvIssue.Id = Guid.NewGuid();
                _repository.Add(prodInvIssue, false);
            }
            else
            {
                var OldValue = _repository.GetById<ProdInvIssue>(prodInvIssue.Id);
                histories = AuditUtility.GetAuditableObject(OldValue, prodInvIssue);
                _repository.Update(prodInvIssue, false);
            }


            if (histories != null)
            {
                histories.ForEach(Hdr =>
                {
                    var prodInvIssueHist = new ProdInvIssueHist()
                    {
                        ProdInvIssueId = prodInvIssue.Id,
                        FieldName = Hdr.FieldName,
                        PrevValue = Hdr.OldValue,
                        CurrentValue = Hdr.NewValue,
                        Active = "Y",
                        Id = Guid.NewGuid()

                    };
                    _repository.Add(prodInvIssueHist);
                });

            }

            if (prodInvIssue.ProdInvIssueDet != null)
            {
                foreach (var det in prodInvIssue.ProdInvIssueDet)
                {
                    det.ProdInvIssueId = prodInvIssue.Id;
                    InsertUpdateProdInvIssueDet(det, false);
                }
            }

            var statusHistory = new ProdInvIssueStatusHist()
            {
                Active = "Y",
                Comments = prodInvIssue.Remarks,
                Status = "PURTRNSTSSUBMITTED",
                ProdInvIssueId = prodInvIssue.Id,
            };

            InsertUpdateProdInvIssueApproval(statusHistory, false);

            _repository.SaveChanges();
        }

        private void InsertUpdateProdInvIssueDet(ProdInvIssueDet det, bool saveChanges)
        {

            var detailHistory = new List<AppAudit>();
            if (det.Id != Guid.Empty)
            {
                var OldDet = _repository.GetById<ProdInvIssueDet>(det.Id);
                detailHistory = AuditUtility.GetAuditableObject<ProdInvIssueDet>(OldDet, det);
                _repository.Update(det, false);
            }
            else
            {
                det.Id = Guid.NewGuid();
                _repository.Add(det, false);
                detailHistory.Add(new AppAudit() { FieldName = "APP_RECORD_CREATED", NewValue = "", OldValue = "" });
            }

            if (detailHistory != null)
            {
                detailHistory.ForEach(Hdr =>
                {
                    var prodInvIssueDetHist = new ProdInvIssueDetHist()
                    {
                        ProdInvIssueDetId = det.Id,
                        FieldName = Hdr.FieldName,
                        PrevValue = Hdr.OldValue,
                        CurrentValue = Hdr.NewValue,
                        Active = "Y",
                        Id = Guid.NewGuid()
                    };
                    _repository.Add(prodInvIssueDetHist);
                });

            }

            if (saveChanges)
                _repository.SaveChanges();
        }

        private void InsertProdInvIssueComments(ProdInvIssueComment comment, bool saveChanges)
        {
            comment.Id = Guid.NewGuid();
            _repository.Add(comment);

            if (comment.AppDocuments != null && comment.AppDocuments.Count > 0)
            {
                foreach (var document in comment.AppDocuments)
                {
                    document.TransactionId = comment.Id;
                    document.TransactionType = "PRODINVISSCOMM";
                    document.Active = "Y";
                }

                var appDocumentBC = new AppDocumentBC(_logger, _repository);
                appDocumentBC.saveAppDocument(comment.AppDocuments, false);
            }
            if (saveChanges)
                _repository.SaveChanges();

        }

        private void InsertProdInvIssueDetComments(ProdInvIssueDetComment comment, bool saveChanges)
        {
            comment.Id = Guid.NewGuid();
            _repository.Add(comment);

            if (comment.AppDocuments != null && comment.AppDocuments.Count > 0)
            {
                foreach (var document in comment.AppDocuments)
                {
                    document.TransactionId = comment.Id;
                    document.TransactionType = "PRODINVISSDETCOMM";
                    document.Active = "Y";
                }
                var appDocumentBC = new AppDocumentBC(_logger, _repository);
                appDocumentBC.saveAppDocument(comment.AppDocuments, false);
            }
            if (saveChanges)
                _repository.SaveChanges();

        }
        private void InsertUpdateProdInvIssueApproval(ProdInvIssueStatusHist statusHis, bool saveChanges)
        {
            if (statusHis.Id == Guid.Empty)
            {
                statusHis.Id = Guid.NewGuid();
                _repository.Add(statusHis, false);
            }
            else
                _repository.Update(statusHis, false);

            var comment = new ProdInvIssueComment()
            {
                Active = "Y",
                Comments = statusHis.Comments,
                Id = Guid.NewGuid(),
                ProdInvIssueId = statusHis.ProdInvIssueId,
            };
            InsertProdInvIssueComments(comment, false);

            if (saveChanges)
                _repository.SaveChanges();
        }

        public List<ProdInvIssue> GetProdInvIssues(ProdInvIssueSearch search, bool isExport = false)
        {

            if (search.FromTransDate <= DateTime.MinValue && search.ToTransDate <= DateTime.MinValue && string.IsNullOrEmpty(search.TransNo) && string.IsNullOrEmpty(search.Status))
            {
                search.FromTransDate = DateTime.Now.AddMonths(-1);
                search.ToTransDate = DateTime.Now;
            }

            var invQry = _repository.GetQuery<ProdInvIssue>();
            var serviceReqQry = _repository.GetQuery<ServiceRequest>();

            var result = (from inv in invQry
                          join sr in serviceReqQry on inv.ServiceRequestId equals sr.Id into serviceReq
                          from srReq in serviceReq.DefaultIfEmpty()
                          where
                              inv.Active == "Y"
                             && (string.IsNullOrEmpty(search.TransNo) || inv.TransNo == search.TransNo)
                             && (string.IsNullOrEmpty(search.Status) || inv.Status == search.Status)
                             && (search.EmployeeId == Guid.Empty || inv.EmployeeId == search.EmployeeId)
                             && (search.FromTransDate <= DateTime.MinValue || inv.TransDate >= search.FromTransDate)
                             && (search.ToTransDate <= DateTime.MinValue || inv.TransDate <= search.ToTransDate)
                          select new
                          {
                              InvInfo = inv,
                              ServReq = srReq
                          }).OrderByDescending(a => a.InvInfo.TransNo).ToList();

            var inventoryList = new List<ProdInvIssue>();
            foreach (var inv in result)
            {
                inventoryList.Add(new ProdInvIssue()
                {
                    Id = inv.InvInfo.Id,
                    Active = inv.InvInfo.Active,
                    CreatedBy = inv.InvInfo.CreatedBy,
                    CreatedDate = inv.InvInfo.CreatedDate,
                    EmployeeId = inv.InvInfo.EmployeeId,
                    ModifiedBy = inv.InvInfo.ModifiedBy,
                    ModifiedDate = inv.InvInfo.ModifiedDate,
                    Remarks = inv.InvInfo.Remarks,
                    ServiceReqNo = inv.ServReq == null ? "" : inv.ServReq.TransNo.ToString(),
                    ServiceRequestId = inv.InvInfo.ServiceRequestId,
                    TransDate = inv.InvInfo.TransDate,
                    SeqNo=inv.InvInfo.SeqNo,
                    TransNo = inv.InvInfo.TransNo,
                    Status = inv.InvInfo.Status,
                    Type = inv.InvInfo.Type,
                });
            }
            if (isExport)
            {
                var employee = _repository.GetQuery<Employee>().Where(a => a.Active == "Y").ToList();
                var invdet = _repository.GetQuery<ProdInvIssueDet>().Where(a => a.Active == "Y").ToList();
                var product = _repository.GetQuery<ProductMaster>().Where(a => a.Active == "Y").ToList();
                var unit = _repository.GetQuery<ProdUnitMaster>().Where(a => a.Active == "Y").ToList();
                var location = _repository.GetQuery<WareHouseLocation>().Where(a => a.Active == "Y").ToList();
                foreach (var inv in inventoryList)
                {
                    var employeename = employee.FirstOrDefault(a => a.Id == inv.EmployeeId);
                    inv.EmployeeName = employeename?.FullNameEng;
                    inv.ProdInvIssueDet = invdet.Where(a => a.ProdInvIssueId == inv.Id).ToList();
                    if (inv.ProdInvIssueDet != null && inv.ProdInvIssueDet.Count > 0)
                    {
                        foreach (var det in inv.ProdInvIssueDet)
                        {
                            det.ProductName = product.FirstOrDefault(a => a.Id == det.ProductMasterId).ProdDescription;
                            det.UnitName = unit.FirstOrDefault(a => a.Id == det.UnitMasterId).UnitName;
                            det.WareHouse = location.FirstOrDefault(a => a.Id == det.WareHouseLocationId).Name;
                            det.Remark = det.Remarks;
                        }
                    }
                }

            }
            return inventoryList;
        }
    }
}