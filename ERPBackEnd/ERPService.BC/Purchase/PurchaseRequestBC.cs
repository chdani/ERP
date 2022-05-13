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
    public class PurchaseRequestBC
    {
        private ILogger _logger;
        private IRepository _repository;
        private UserContext _userContext;

        public PurchaseRequestBC(ILogger logger, IRepository repository, UserContext userContext)
        {
            _logger = logger;
            _repository = repository;
            _userContext = userContext;
        }
        public AppResponse SavePurchaseRequest(PurchaseRequest purchaseRequest)
        {
            List<String> validationMessages = new List<string>();
            AppResponse appResponse = new AppResponse();
            if (purchaseRequest.Id == Guid.Empty)
            {
                var purchaseRequestTransNoAndSeqNo = AppGeneralMethods.TranstypeSeqNumber("purchaseRequestTransType", _repository);
                purchaseRequest.TransNo = purchaseRequestTransNoAndSeqNo.Item1;
                purchaseRequest.SeqNo = purchaseRequestTransNoAndSeqNo.Item2;
            }
            if (purchaseRequest != null)
            {
                InsertUpdatePurchaseRequest(purchaseRequest);
                appResponse.Status = APPMessageKey.DATASAVESUCSS;
            }
            else
            {
                appResponse.Status = APPMessageKey.ONEORMOREERR;
                appResponse.Messages = validationMessages;
            }


            return appResponse;
        }
        public AppResponse SavePurchaseRequestComment(PurchaseReqComment purchaseReqComment)
        {
            InsertPurchaseRequestComments(purchaseReqComment, true);
            return new AppResponse() { Status = APPMessageKey.DATASAVESUCSS };
        }
        private void InsertPurchaseRequestComments(PurchaseReqComment comment, bool saveChanges)
        {
            comment.Id = Guid.NewGuid();
            _repository.Add(comment);

            if (comment.AppDocuments != null)
            {
                foreach (var document in comment.AppDocuments)
                {
                    document.TransactionId = comment.Id;
                    document.TransactionType = "PURCSREQCOMM";
                    document.Active = "Y";
                }

                var appDocumentBC = new AppDocumentBC(_logger, _repository);
                appDocumentBC.saveAppDocument(comment.AppDocuments, false);
            }
            if (saveChanges)
                _repository.SaveChanges();

        }
        public AppResponse SavePurchaseReqDetComment(PurchaseReqDetComment purchaseReqDetComment)
        {
            InsertPurchaseReqDetComments(purchaseReqDetComment, true);
            return new AppResponse() { Status = APPMessageKey.DATASAVESUCSS };
        }
        private void InsertPurchaseReqDetComments(PurchaseReqDetComment comment, bool saveChanges)
        {
            comment.Id = Guid.NewGuid();
            _repository.Add(comment);

            if (comment.AppDocuments != null)
            {
                foreach (var document in comment.AppDocuments)
                {
                    document.TransactionId = comment.Id;
                    document.TransactionType = "PURCSREQDETCOMM";
                    document.Active = "Y";
                }
                var appDocumentBC = new AppDocumentBC(_logger, _repository);
                appDocumentBC.saveAppDocument(comment.AppDocuments, false);
            }
            if (saveChanges)
                _repository.SaveChanges();

        }

        private void InsertUpdatePurchaseRequest(PurchaseRequest purchaseRequest)
        {
            purchaseRequest.TransDate = purchaseRequest.TransDate.ToLocalTime().Date;
            var histories = new List<AppAudit>();
            if (purchaseRequest.Id != Guid.Empty)
            {
                var OldDet = _repository.GetById<PurchaseRequest>(purchaseRequest.Id);
                histories = AuditUtility.GetAuditableObject<PurchaseRequest>(OldDet, purchaseRequest);
            }
            else
                histories.Add(new AppAudit() { FieldName = "APP_RECORD_CREATED", NewValue = "", OldValue = "" });



            if (purchaseRequest.Id == Guid.Empty)
            {
                purchaseRequest.Id = Guid.NewGuid();
                _repository.Add(purchaseRequest, false);
            }
            else
            {
                _repository.Update(purchaseRequest, false);
            }
            if (histories != null && histories.Count != 0)
            {
                histories.ForEach(Hdr =>
                {
                    var purchaseReqHist = new PurchaseReqHist()
                    {
                        Id = Guid.NewGuid(),
                        PurchaseRequestId = purchaseRequest.Id,
                        FieldName = Hdr.FieldName,
                        PrevValue = Hdr.OldValue,
                        CurrentValue = Hdr.NewValue,
                        Active = "Y"

                    };
                    _repository.Add(purchaseReqHist);
                });

            }
            var statusHistory = new PurchaseRequestStatusHist()
            {

                Active = "Y",
                Comments = purchaseRequest.Remarks,
                Status = "PURTRNSTSSUBMITTED",
                PurchaseRequestId = purchaseRequest.Id,
            };
            InsertUpdatePurchaseRequestStatusHist(statusHistory, false);

            if (purchaseRequest.PurchaseRequestDetList != null)
            {
                foreach (var det in purchaseRequest.PurchaseRequestDetList)
                {
                    det.PurchaseRequestId = purchaseRequest.Id;
                    InsertUpdatePurchaseRequestDet(det, false);
                }
            }
            _repository.SaveChanges();
        }
        private void InsertUpdatePurchaseRequestStatusHist(PurchaseRequestStatusHist statusHis, bool saveChanges)
        {
            if (statusHis.Id == Guid.Empty)
            {
                statusHis.Id = Guid.NewGuid();
                _repository.Add(statusHis, false);
            }
            else
                _repository.Update(statusHis, false);
            if (saveChanges)
                _repository.SaveChanges();
        }
        private void InsertUpdatePurchaseRequestDet(PurchaseRequestDet purchaseRequestDet, Boolean saveChanges)
        {
            var detailHistory = new List<AppAudit>();
            if (purchaseRequestDet.Id != Guid.Empty)
            {
                var OldDet = _repository.GetById<PurchaseRequestDet>(purchaseRequestDet.Id);
                detailHistory = AuditUtility.GetAuditableObject<PurchaseRequestDet>(OldDet, purchaseRequestDet);
            }
            else
                detailHistory.Add(new AppAudit() { FieldName = "APP_RECORD_CREATED", NewValue = "", OldValue = "" });

            if (purchaseRequestDet.Id == Guid.Empty)
            {
                purchaseRequestDet.Id = Guid.NewGuid();
                _repository.Add(purchaseRequestDet, false);
            }
            else
            {
                _repository.Update(purchaseRequestDet, false);
            }
            if (detailHistory != null && detailHistory.Count != 0)
            {
                detailHistory.ForEach(Hdr =>
                {
                    var purchaseReqDetHist = new PurchaseReqDetHist()
                    {
                        Id = Guid.NewGuid(),
                        PurchaseRequestDetId = purchaseRequestDet.Id,
                        FieldName = Hdr.FieldName,
                        PrevValue = Hdr.OldValue,
                        CurrentValue = Hdr.NewValue,
                        Active = "Y"

                    };
                    _repository.Add(purchaseReqDetHist);
                });

            }
            if (saveChanges)
            {
                _repository.SaveChanges();
            }
        }
        public List<PurchaseRequestDet> GetPurchaseReqDetByHdrId(Guid id)
        {
            var purchaseRequestDet = _repository.GetQuery<PurchaseRequestDet>().Where(a => a.PurchaseRequestId == id && a.Active == "Y").ToList();
            return purchaseRequestDet;
        }
        public PurchaseRequest GetPurchaseRequestById(Guid purchaseRequestId)
        {
            PurchaseRequest purchaseRequest = new PurchaseRequest();
            purchaseRequest = _repository.GetById<PurchaseRequest>(purchaseRequestId);
            if (purchaseRequest != null)
            {
                purchaseRequest.PurchaseRequestDetList = _repository.GetQuery<PurchaseRequestDet>().Where(a => a.PurchaseRequestId == purchaseRequest.Id && a.Active == "Y").ToList();
            }
            return purchaseRequest;
        }
        public AppResponse ApprovePurchaseRequest(PurchaseRequest purchaseRequest)
        {
            List<String> validationMessages = new List<string>();
            AppResponse appResponse = new AppResponse();
            if (purchaseRequest != null)
            {
                _repository.Update(purchaseRequest);
                var statusHistory = new PurchaseRequestStatusHist()
                {

                    Active = "Y",
                    Comments = purchaseRequest.ApproverRemarks,
                    Status = purchaseRequest.Status,
                    PurchaseRequestId = purchaseRequest.Id,
                };
                InsertUpdatePurchaseRequestStatusHist(statusHistory, false);

                _repository.SaveChanges();
                appResponse.Status = APPMessageKey.DATASAVESUCSS;
            }

            else
            {
                appResponse.Status = APPMessageKey.ONEORMOREERR;
                appResponse.Messages = validationMessages;
            }
            return appResponse;
        }
        public List<PurchaseRequest> GetPurchaseRequestList(PurchaseRequest purchaseRequest, bool isExport)
        {
            List<PurchaseRequest> purchaseRequests = new List<PurchaseRequest>();
            purchaseRequest.FromTransDate = purchaseRequest.FromTransDate.ToLocalTime().Date;
            purchaseRequest.ToTransDate = purchaseRequest.ToTransDate.ToLocalTime().Date;
            purchaseRequests = _repository.GetQuery<PurchaseRequest>().Where(a => (purchaseRequest.VendorMasterId == Guid.Empty || purchaseRequest.VendorMasterId == a.VendorMasterId) &&
           (string.IsNullOrEmpty(purchaseRequest.TransNo) || a.TransNo == purchaseRequest.TransNo) && (purchaseRequest.VendorQuotationId == Guid.Empty || purchaseRequest.VendorQuotationId == a.VendorQuotationId) &&
            (string.IsNullOrEmpty(purchaseRequest.Status) || purchaseRequest.Status == a.Status)
            && (purchaseRequest.FromTransDate <= DateTime.MinValue || a.TransDate >= purchaseRequest.FromTransDate)
                        && (purchaseRequest.ToTransDate <= DateTime.MinValue || a.TransDate <= purchaseRequest.ToTransDate) && a.Active == "Y").OrderByDescending(a => a.CreatedDate).ToList();

            if (isExport)
            {
                var purchaseReqDet = _repository.GetQuery<PurchaseRequestDet>().ToList();
                var vendorQuotationList = _repository.GetQuery<VendorQuotation>().ToList();
                var vendorMasterList = _repository.GetQuery<VendorMaster>().ToList();
                var productMasterList = _repository.GetQuery<ProductMaster>().ToList();
                var unitMasterList = _repository.GetQuery<ProdUnitMaster>().ToList();
                purchaseRequests.ForEach(PR =>
                {
                    var vendorQuotationTitle = vendorQuotationList.FirstOrDefault(a => a.Id == PR.VendorQuotationId);
                    var vendorMasterListTitle = vendorMasterList.FirstOrDefault(a => a.Id == PR.VendorMasterId);
                    PR.VendorQuotation = vendorQuotationTitle.TransNo;
                    PR.VendorMaster = vendorMasterListTitle?.Title + "." + vendorMasterListTitle?.Name;
                    PR.PurchaseRequestDetList = purchaseReqDet.Where(a => a.PurchaseRequestId == PR.Id).ToList();
                    if (PR.PurchaseRequestDetList != null)
                    {
                        PR.PurchaseRequestDetList.ForEach(PRDetList =>
                        {
                            var prodname = productMasterList.FirstOrDefault(a => a.Id == PRDetList.ProductMasterId);
                            var prodUnitName = unitMasterList.FirstOrDefault(a => a.Id == PRDetList.UnitMasterId);
                            PRDetList.ProductName = prodname?.ProdDescription;
                            PRDetList.UnitName = prodUnitName?.UnitName;
                            PRDetList.Remark = PRDetList.Remarks;
                        });

                    }
                });
            }

            return purchaseRequests;
        }
        public List<PurchaseReqHist> GetPurchaseReqHistory(Guid purchaseReqId)
        {
            var usemaster = _repository.GetQuery<UserMaster>().ToList();
            var purchaseReqHist = _repository.GetQuery<PurchaseReqHist>().Where(a => a.PurchaseRequestId == purchaseReqId).OrderByDescending(a => a.CreatedDate).ToList();
            purchaseReqHist.ForEach(Hsty =>
            {
                var userName = usemaster.Where(a => a.Id == Hsty.CreatedBy).FirstOrDefault();
                if (userName != null)
                    Hsty.UserName = userName.FirstName + " " + userName.LastName;
            });
            return purchaseReqHist;
        }

        public List<PurchaseReqDetHist> GetPurchaseReqDetHistory(Guid purchaseReqDetId)
        {
            var usemaster = _repository.GetQuery<UserMaster>().ToList();
            var purchaseReqDetHist = _repository.GetQuery<PurchaseReqDetHist>().Where(a => a.PurchaseRequestDetId == purchaseReqDetId).OrderByDescending(a => a.CreatedDate).ToList();
            purchaseReqDetHist.ForEach(Hsty =>
            {
                var userName = usemaster.Where(a => a.Id == Hsty.CreatedBy).FirstOrDefault();
                if (userName != null)
                    Hsty.UserName = userName.FirstName + " " + userName.LastName;
            });
            return purchaseReqDetHist;
        }

        public List<PurchaseReqComment> GetPurchaseReqComment(Guid purchaseReqId)
        {
            var comments = _repository.GetQuery<PurchaseReqComment>().Where(a => a.PurchaseRequestId == purchaseReqId).ToList();
            var commentIds = comments.Select(a => a.Id).Distinct();

            var User = _repository.GetQuery<UserMaster>().ToList();

            var appDoucuments = _repository.GetQuery<AppDocument>().Where(a => a.Active == "Y"
                    && a.TransactionType == "PURCSREQCOMM" && commentIds.Contains(a.TransactionId)).ToList();

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

        public List<PurchaseReqDetComment> GetPurchaseReqDetComment(Guid purchaseReqDetId)
        {
            var comments = _repository.GetQuery<PurchaseReqDetComment>().Where(a => a.PurchaseRequestDetId == purchaseReqDetId).ToList();
            var commentIds = comments.Select(a => a.Id).Distinct();

            var User = _repository.GetQuery<UserMaster>().ToList();

            var appDoucuments = _repository.GetQuery<AppDocument>().Where(a => a.Active == "Y"
                    && a.TransactionType == "PURCSREQDETCOMM" && commentIds.Contains(a.TransactionId)).ToList();

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

        public List<AppDocument> GetPurchaseReqAttachments(Guid purchaseReqId)
        {
            var usemaster = _repository.GetQuery<UserMaster>().ToList();

            var comments = _repository.GetQuery<PurchaseReqComment>().Where(a => a.PurchaseRequestId == purchaseReqId && a.Active == "Y").ToList();

            var fileAttachmentIds = new List<Guid>();
            fileAttachmentIds.AddRange(comments.Select(a => a.Id).Distinct().ToList());
            fileAttachmentIds.Add(purchaseReqId);

            var appDoucuments = _repository.GetQuery<AppDocument>().Where(a => a.Active == "Y"
                    && (a.TransactionType == "PURCSREQCOMM" || a.TransactionType == "PURCHASEREQ") && fileAttachmentIds.Contains(a.TransactionId))
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

        public List<AppDocument> GetPurchaseReqDetAttachments(Guid purchaseReqDetId)
        {
            var usemaster = _repository.GetQuery<UserMaster>().ToList();

            var comments = _repository.GetQuery<PurchaseReqDetComment>().Where(a => a.PurchaseRequestDetId == purchaseReqDetId && a.Active == "Y").ToList();

            var fileAttachmentIds = new List<Guid>();
            fileAttachmentIds.AddRange(comments.Select(a => a.Id).Distinct().ToList());
            fileAttachmentIds.Add(purchaseReqDetId);

            var appDoucuments = _repository.GetQuery<AppDocument>().Where(a => a.Active == "Y"
                    && (a.TransactionType == "PURCSREQDETCOMM" || a.TransactionType == "PURCHASEREQDET") && fileAttachmentIds.Contains(a.TransactionId))
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

        public List<PurchaseRequestStatusHist> GetPurchaseReqStatusHistory(Guid purchaseReqId)
        {
            var usemaster = _repository.GetQuery<UserMaster>().Where(a => a.Active == "Y").ToList();
            var purchaseRequestStatusHist = _repository.GetQuery<PurchaseRequestStatusHist>().Where(a => a.PurchaseRequestId == purchaseReqId).OrderByDescending(a => a.CreatedDate).ToList();
            purchaseRequestStatusHist.ForEach(status =>
            {
                var userName = usemaster.Where(a => a.Id == status.CreatedBy).FirstOrDefault();
                if (userName != null)
                    status.UserName = userName.FirstName + " " + userName.LastName;
            });

            return purchaseRequestStatusHist;
        }



        public void NotificationsServiceRequest(PurchaseRequest purchaseRequest)
        {

            if (purchaseRequest.Status == "PURTRNSTSAPPROVED")
            {

                var vendorMaster = _repository.GetQuery<VendorMaster>().Where(a => a.Id == purchaseRequest.VendorMasterId).FirstOrDefault();
                var productMasterList = _repository.GetQuery<ProductMaster>().ToList();
                var unitMasterList = _repository.GetQuery<ProdUnitMaster>().ToList();
                string text = "Purchase Request# " + purchaseRequest.TransNo + " has been made from " + vendorMaster.Title + "." + vendorMaster.Name + ". following are the details. " + Environment.NewLine;

                int x = 0;
                purchaseRequest.PurchaseRequestDetList.ForEach(PR =>
                {

                    var prodname = productMasterList.FirstOrDefault(a => a.Id == PR.ProductMasterId);
                    var prodUnitName = unitMasterList.FirstOrDefault(a => a.Id == PR.UnitMasterId);
                    PR.ProductName = prodname?.ProdDescription;
                    PR.UnitName = prodUnitName?.UnitName;
                    PR.Remark = PR.Remarks;

                    text = text + "Sr#" + x++ + ":  Product " + PR.ProductName + "   required Quantity " + PR.Quantity + "  " + PR.UnitName + Environment.NewLine;
                });

                SMSUtility.SendSMSService(text, vendorMaster.Mobile);
            }
        }
    }
}

