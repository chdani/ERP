using ERPService.BC.Utility;
using ERPService.Common;
using ERPService.Common.Interfaces;
using ERPService.Common.Shared;
using ERPService.DataModel.CTO;
using ERPService.DataModel.DTO;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;

namespace ERPService.BC
{
    public class PurchaseOrderBC
    {
        private ILogger _logger;
        private IRepository _repository;
        private UserContext _userContext;
        private byte[] buff;

        public PurchaseOrderBC(ILogger logger, IRepository repository, UserContext userContext)
        {
            _logger = logger;
            _repository = repository;
            _userContext = userContext;
        }

        public PurchaseOrder GetPurchaseOrderById(Guid purchaseOrderId)
        {
            var goodsRecNote = _repository.GetById<PurchaseOrder>(purchaseOrderId);
            if (goodsRecNote != null)
                goodsRecNote.PurchaseOrderDet = GetPurchaseOrderDetByHdrId(purchaseOrderId);
            return goodsRecNote;
        }

        public List<PurchaseOrderDet> GetPurchaseOrderDetByHdrId(Guid hdrId)
        {
            return _repository.GetQuery<PurchaseOrderDet>().Where(a => a.Active == "Y" && a.PurchaseOrderId == hdrId).ToList(); ;
        }

        public List<PurchaseOrder> GetPurchaseOrders(PurchaseOrderSearch search, bool isExport = false)
        {
            if (search.FromTransDate <= DateTime.MinValue && search.ToTransDate <= DateTime.MinValue && string.IsNullOrEmpty(search.TransNo))
            {
                search.FromTransDate = DateTime.Now.AddMonths(-1);
                search.ToTransDate = DateTime.Now;
            }

            var purchaseOrderList = _repository.GetQuery<PurchaseOrder>().Where(a => a.Active == "Y"
                        && (string.IsNullOrEmpty(search.TransNo) || a.TransNo == search.TransNo) &&
                         (string.IsNullOrEmpty(search.FinYear) || a.FinYear.Contains(search.FinYear))
              && (search.OrgId == Guid.Empty || search.OrgId == a.OrgId)
                        && (search.VendorMasterId == Guid.Empty || a.VendorMasterId == search.VendorMasterId)
                        && (string.IsNullOrEmpty(search.Status) || a.Status == search.Status)
                        && (search.FromTransDate <= DateTime.MinValue || a.TransDate >= search.FromTransDate)
                        && (search.ToTransDate <= DateTime.MinValue || a.TransDate <= search.ToTransDate)
                        ).OrderByDescending(a => a.TransNo).ToList();
            if (purchaseOrderList != null && purchaseOrderList.Count > 0)
            {
                var languageBC = new LangMasterBC(_logger, _repository);
                var costcenterList = languageBC.GetLangBasedDataForCostCenters(_userContext.Language);
                var ledgerList = languageBC.GetLangBasedDataForLedgerAccounts(_userContext.Language);
                var purchase = _repository.GetQuery<PurchaseRequest>().Where(a => a.Active == "Y");
                foreach (var purchaeReq in purchaseOrderList)
                {
                    purchaeReq.PurchaseRequestNo = purchase.FirstOrDefault(p => p.Id == purchaeReq.PurchaseRequestId).TransNo;
                    var ledger = ledgerList.FirstOrDefault(a => a.LedgerCode == purchaeReq.LedgerCode);
                    var cost = costcenterList.FirstOrDefault(a => a.Code == purchaeReq.CostCenterCode);
                    purchaeReq.ledger = $"{ledger?.LedgerCode}{"-"}{ledger?.LedgerDesc}";
                    purchaeReq.CostCenter = $"{cost?.Code}{""}{cost?.Description}";
                }
            }
            if (isExport)
            {
                VendorMasterBC vendorBC = new VendorMasterBC(_logger, _repository, _userContext);
                var vendorlist = vendorBC.GetVendorMasterList();
                var product = _repository.GetQuery<ProductMaster>().Where(a => a.Active == "Y").ToList();
                var unit = _repository.GetQuery<ProdUnitMaster>().Where(a => a.Active == "Y").ToList();
                var productorderDet = _repository.GetQuery<PurchaseOrderDet>().Where(a => a.Active == "Y").ToList();
                foreach (var productorder in purchaseOrderList)
                {
                    var vendor = vendorlist.FirstOrDefault(a => a.Id == productorder.VendorMasterId && a.Active == "Y");
                    productorder.Vendor = $"{vendor.Title} {" "} {vendor.Name}";
                    productorder.PurchaseOrderDet = _repository.GetQuery<PurchaseOrderDet>().Where(a => a.Active == "Y" && a.PurchaseOrderId == productorder.Id).ToList();
                    if (productorder.PurchaseOrderDet != null && productorder.PurchaseOrderDet.Count > 0)
                    {
                        foreach (var det in productorder.PurchaseOrderDet)
                        {
                            det.ProductName = product.FirstOrDefault(a => a.Id == det.ProductMasterId).ProdDescription;
                            det.UnitName = unit.FirstOrDefault(a => a.Id == det.UnitMasterId).UnitName;
                            det.Remark = det.Remarks;
                        }
                    }

                }
            }
            return purchaseOrderList;
        }
        public PurchaseRequest GetPurchaseRequestId(Guid hdrId)
        {
            return _repository.GetQuery<PurchaseRequest>().FirstOrDefault(a => a.Active == "Y" && a.Id == hdrId);
        }
        public List<PurchaseRequestDet> GetPurchaseRequestDetByHdrId(Guid hdrId)
        {
            return _repository.GetQuery<PurchaseRequestDet>().Where(a => a.Active == "Y" && a.PurchaseRequestId == hdrId).ToList(); ;
        }
        public List<PurchaseRequest> GetPurchaseRequest(PurchaseRequest purchaseRequest)
        {
            List<PurchaseRequest> purchaseRequests = new List<PurchaseRequest>();
            purchaseRequests = _repository.GetQuery<PurchaseRequest>().Where(a => (purchaseRequest.VendorMasterId == Guid.Empty || purchaseRequest.VendorMasterId == a.VendorMasterId) &&
           (string.IsNullOrEmpty(purchaseRequest.TransNo) || a.TransNo == purchaseRequest.TransNo) && (purchaseRequest.VendorQuotationId == Guid.Empty || purchaseRequest.VendorQuotationId == a.VendorQuotationId) &&
            (string.IsNullOrEmpty(purchaseRequest.Status) || purchaseRequest.Status == a.Status)
            && a.Active == "Y").ToList();

            return purchaseRequests;
        }
        public AppResponse SavePurchaseOrder(PurchaseOrder purchaseOrder)
        {
            List<String> validationMessages = new List<string>();
            AppResponse appResponse = new AppResponse();
            bool validation = true;

            if (purchaseOrder.PurchaseRequestId == Guid.Empty || purchaseOrder.VendorMasterId == Guid.Empty
                || purchaseOrder.TransDate <= DateTime.MinValue)

            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.MANDMISSING]);
                validation = false;
            }
            if (purchaseOrder.Active == "Y")
            {
                if (purchaseOrder.PurchaseOrderDet == null || purchaseOrder.PurchaseOrderDet.Count == 0)
                {
                    validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.NOTDETROWEXIST]);
                    validation = false;
                }
                else
                {

                    bool childValid = true;
                    var childMessages = ValidateChildRecords(purchaseOrder.PurchaseOrderDet, out childValid);
                    if (!childValid)
                    {
                        validation = false;
                        validationMessages.AddRange(childMessages);
                    }
                }
            }
            if (purchaseOrder.Id == Guid.Empty)
            {
                var PurchaseOrderTransNoAndSeqNo = AppGeneralMethods.TranstypeSeqNumber("PurchaseOrderTransType", _repository);
                purchaseOrder.TransNo = PurchaseOrderTransNoAndSeqNo.Item1;
                purchaseOrder.SeqNo = PurchaseOrderTransNoAndSeqNo.Item2;
            }

            if (purchaseOrder != null && validation)
            {

                purchaseOrder.TransDate = purchaseOrder.TransDate.ToLocalTime().Date;
                purchaseOrder.Status = "PURTRNSTSSUBMITTED";
                InsertUpdatePurchaseOrder(purchaseOrder);
                appResponse.Status = APPMessageKey.DATASAVESUCSS;
            }
            else
            {
                appResponse.Status = APPMessageKey.ONEORMOREERR;
                appResponse.Messages = validationMessages;
            }
            return appResponse;
        }
        private List<string> ValidateChildRecords(List<PurchaseOrderDet> purchaseOrderDets, out bool validation)
        {
            var validationMessages = new List<string>();
            validation = true;
            foreach (var det in purchaseOrderDets)
            {
                if (det.ProductMasterId == Guid.Empty
                    || det.Quantity <= 0
                    || det.UnitMasterId == Guid.Empty
                    || det.Price <= 0)
                {
                    validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.MANDMISSING]);
                    validation = false;
                }
            }

            return validationMessages;
        }
        private void InsertUpdatePurchaseOrder(PurchaseOrder purchaseOrder)
        {
            var histories = new List<AppAudit>();

            if (purchaseOrder.Id == Guid.Empty)
            {
                histories.Add(new AppAudit() { FieldName = "APP_RECORD_CREATED", NewValue = "", OldValue = "" });
                purchaseOrder.Id = Guid.NewGuid();
                _repository.Add(purchaseOrder, false);
            }
            else
            {
                var OldValue = _repository.GetById<PurchaseOrder>(purchaseOrder.Id);
                histories = AuditUtility.GetAuditableObject(OldValue, purchaseOrder);
                _repository.Update(purchaseOrder, false);
            }


            if (histories != null)
            {
                histories.ForEach(Hdr =>
                {
                    var purchaseOrdHist = new PurchaseOrdHist()
                    {
                        PurchaseOrderId = purchaseOrder.Id,
                        FieldName = Hdr.FieldName,
                        PrevValue = Hdr.OldValue,
                        CurrentValue = Hdr.NewValue,
                        Active = "Y",
                        Id = Guid.NewGuid()

                    };
                    _repository.Add(purchaseOrdHist);
                });

            }

            if (purchaseOrder.PurchaseOrderDet != null)
            {
                foreach (var det in purchaseOrder.PurchaseOrderDet)
                {
                    det.PurchaseOrderId = purchaseOrder.Id;
                    InsertUpdatePurchaseOrderDet(det, false);
                }
            }

            var statusHistory = new PurchaseOrdStatusHist()
            {
                Active = "Y",
                Comments = purchaseOrder.Remarks,
                Status = "PURTRNSTSSUBMITTED",
                PurchaseOrderId = purchaseOrder.Id,
            };

            InsertUpdatePurchaseOrderApproval(statusHistory, false);

            _repository.SaveChanges();
        }
        private void InsertUpdatePurchaseOrderDet(PurchaseOrderDet det, bool saveChanges)
        {

            var detailHistory = new List<AppAudit>();
            if (det.Id != Guid.Empty)
            {
                var OldDet = _repository.GetById<PurchaseOrderDet>(det.Id);
                detailHistory = AuditUtility.GetAuditableObject<PurchaseOrderDet>(OldDet, det);
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
                    var purchaseOrdDetHist = new PurchaseOrdDetHist()
                    {
                        PurchaseOrderDetId = det.Id,
                        FieldName = Hdr.FieldName,
                        PrevValue = Hdr.OldValue,
                        CurrentValue = Hdr.NewValue,
                        Active = "Y",
                        Id = Guid.NewGuid()
                    };
                    _repository.Add(purchaseOrdDetHist);
                });

            }

            if (saveChanges)
                _repository.SaveChanges();
        }

        private void InsertUpdatePurchaseOrderApproval(PurchaseOrdStatusHist statusHis, bool saveChanges)
        {
            if (statusHis.Id == Guid.Empty)
            {
                statusHis.Id = Guid.NewGuid();
                _repository.Add(statusHis, false);
            }
            else
                _repository.Update(statusHis, false);

            var comment = new PurchaseOrdComment()
            {
                Active = "Y",
                Comments = statusHis.Comments,
                Id = Guid.NewGuid(),
                PurchaseOrderId = statusHis.PurchaseOrderId,
            };


            if (saveChanges)
                _repository.SaveChanges();
        }

        private void InsertPurchaseOrderComments(PurchaseOrdComment comment, bool saveChanges)
        {
            comment.Id = Guid.NewGuid();
            _repository.Add(comment);

            if (comment.AppDocuments != null && comment.AppDocuments.Count > 0)
            {
                foreach (var document in comment.AppDocuments)
                {
                    document.TransactionId = comment.Id;
                    document.TransactionType = "PURCHASEORDERCOMM";
                    document.Active = "Y";
                }

                var appDocumentBC = new AppDocumentBC(_logger, _repository);
                appDocumentBC.saveAppDocument(comment.AppDocuments, false);
            }
            if (saveChanges)
                _repository.SaveChanges();

        }

        private void InsertPurchaseOrderDetComments(PurchaseOrdDetComment comment, bool saveChanges)
        {
            comment.Id = Guid.NewGuid();
            _repository.Add(comment);

            if (comment.AppDocuments != null && comment.AppDocuments.Count > 0)
            {
                foreach (var document in comment.AppDocuments)
                {
                    document.TransactionId = comment.Id;
                    document.TransactionType = "PURCHASEORDERDETCOMM";
                    document.Active = "Y";
                }
                var appDocumentBC = new AppDocumentBC(_logger, _repository);
                appDocumentBC.saveAppDocument(comment.AppDocuments, false);
            }
            if (saveChanges)
                _repository.SaveChanges();

        }

        public AppResponse ApprovePurchaseOrder(PurchaseOrder purchaseOrder)
        {
            List<String> validationMessages = new List<string>();
            AppResponse appResponse = new AppResponse();
            bool validation = true;

            if (purchaseOrder.Id == Guid.Empty || string.IsNullOrEmpty(purchaseOrder.ApproverRemarks))
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.MANDMISSING]);
                validation = false;
            }
            appResponse.ReferenceId = purchaseOrder.Id;
            var purchaseOrderDetails = GetPurchaseOrderDetByHdrId(purchaseOrder.Id);
            if (purchaseOrderDetails == null || purchaseOrderDetails.Count <= 0)
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.MANDMISSING]);
                validation = false;
            }

            if (validation)
            {
                var statusHistory = new PurchaseOrdStatusHist()
                {
                    Active = "Y",
                    Comments = purchaseOrder.ApproverRemarks,
                    Status = purchaseOrder.Status,
                    PurchaseOrderId = purchaseOrder.Id,
                };


                purchaseOrder.PurchaseOrderDet = null;

                if (purchaseOrder.Status == "PURTRNSTSAPPROVED")
                {

                    _repository.Update(purchaseOrder, false);
                    InsertUpdatePurchaseOrderApproval(statusHistory, false);
                    var purchaseOrderTransNo = long.Parse(purchaseOrder.TransNo);
                    var DirPrePay = new DirectInvPrePaymentBC(_logger, _repository);
                    var dirPrePayment = new DirectInvPrePayment()
                    {
                        FinYear = purchaseOrder.FinYear,
                        OrgId = purchaseOrder.OrgId,
                        VendorMasterId = purchaseOrder.VendorMasterId,
                        DocumentDate = purchaseOrder.TransDate,
                        LedgerCode = purchaseOrder.LedgerCode,
                        CostCenterCode = purchaseOrder.CostCenterCode,
                        Amount = purchaseOrder.TotalAmount,
                        DueAmount = purchaseOrder.TotalAmount,
                        Remarks = purchaseOrder.Remarks,
                        Active = "Y",
                        Status = "APPROVED",
                        ApproverRemarks = purchaseOrder.ApproverRemarks,
                        PurchaseOrderId = purchaseOrder.Id,
                        DocumentNo = purchaseOrderTransNo,
                        ApprovedBy = _userContext.Id,
                        ApprovedDate = DateTime.Now,
                    };
                    var saveDirPrePay = DirPrePay.SaveDirectInvPrePayment(dirPrePayment, false);

                    _repository.SaveChanges();
                    appResponse.Status = APPMessageKey.DATASAVESUCSS;
                }

                else
                {
                    _repository.Update(purchaseOrder, false);
                    InsertUpdatePurchaseOrderApproval(statusHistory, false);
                    _repository.SaveChanges();
                    appResponse.Status = APPMessageKey.DATASAVESUCSS;
                }

            }
            else
            {
                appResponse.Status = APPMessageKey.ONEORMOREERR;
                appResponse.Messages = validationMessages;
            }
            return appResponse;
        }

        public List<PurchaseOrdDetComment> GetPurchaseOrderDetComment(Guid id)
        {
            var comments = _repository.GetQuery<PurchaseOrdDetComment>().Where(a => a.PurchaseOrderDetId == id).ToList();
            var commentIds = comments.Select(a => a.Id).Distinct();

            var User = _repository.GetQuery<UserMaster>().ToList();

            var appDoucuments = _repository.GetQuery<AppDocument>().Where(a => a.Active == "Y"
                    && a.TransactionType == "PURCHASEORDERDETCOMM" && commentIds.Contains(a.TransactionId)).ToList();

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
        public List<PurchaseOrdDetHist> GetPurchaseOrderDetHistory(Guid id)
        {
            var usemaster = _repository.GetQuery<UserMaster>().ToList();
            var purchaseorderDetIdHist = _repository.GetQuery<PurchaseOrdDetHist>().Where(a => a.PurchaseOrderDetId == id).OrderByDescending(a => a.CreatedDate).ToList();
            purchaseorderDetIdHist.ForEach(Hsty =>
            {
                var userName = usemaster.Where(a => a.Id == Hsty.CreatedBy).FirstOrDefault();
                if (userName != null)
                    Hsty.UserName = userName.FirstName + " " + userName.LastName;
            });
            return purchaseorderDetIdHist.OrderByDescending(a => a.CreatedDate).ToList();
        }
        public AppResponse SavePurchaseOrderDetComment(PurchaseOrdDetComment purchaseOrdDet)
        {
            InsertPurchaseOrderDetComments(purchaseOrdDet, true);
            return new AppResponse() { Status = APPMessageKey.DATASAVESUCSS };
        }
        public AppResponse SavePurchaseOrderComment(PurchaseOrdComment purchaseOrd)
        {
            InsertPurchaseOrderComments(purchaseOrd, true);
            return new AppResponse() { Status = APPMessageKey.DATASAVESUCSS };
        }
        public List<PurchaseOrdComment> GetPurchaseOrderComment(Guid Id)
        {
            var comments = _repository.GetQuery<PurchaseOrdComment>().Where(a => a.PurchaseOrderId == Id).ToList();
            var commentIds = comments.Select(a => a.Id).Distinct();

            var User = _repository.GetQuery<UserMaster>().ToList();

            var appDoucuments = _repository.GetQuery<AppDocument>().Where(a => a.Active == "Y"
                    && a.TransactionType == "PURCHASEORDERCOMM" && commentIds.Contains(a.TransactionId)).ToList();

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
        public List<AppDocument> GetPurchaseOrderAttachments(Guid Id)
        {
            var usemaster = _repository.GetQuery<UserMaster>().ToList();

            var comments = _repository.GetQuery<PurchaseOrdComment>().Where(a => a.PurchaseOrderId == Id && a.Active == "Y").ToList();

            var fileAttachmentIds = new List<Guid>();
            fileAttachmentIds.AddRange(comments.Select(a => a.Id).Distinct().ToList());
            fileAttachmentIds.Add(Id);

            var appDoucuments = _repository.GetQuery<AppDocument>().Where(a => a.Active == "Y"
                    && (a.TransactionType == "PURCHASEORDERCOMM" || a.TransactionType == "PURCHASEORDER") && fileAttachmentIds.Contains(a.TransactionId))
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
        public List<PurchaseOrdStatusHist> GetPurchaseOrderStatusHistory(Guid Id)
        {
            var usemaster = _repository.GetQuery<UserMaster>().Where(a => a.Active == "Y").ToList();
            var approvals = _repository.GetQuery<PurchaseOrdStatusHist>().Where(a => a.PurchaseOrderId == Id).OrderByDescending(a => a.CreatedDate).ToList();
            approvals.ForEach(status =>
            {
                var userName = usemaster.Where(a => a.Id == status.CreatedBy).FirstOrDefault();
                if (userName != null)
                    status.UserName = userName.FirstName + " " + userName.LastName;
            });

            return approvals.OrderByDescending(a => a.CreatedDate).ToList();
        }
        public List<PurchaseOrdHist> GetPurchaseOrderHistory(Guid Id)
        {
            var usemaster = _repository.GetQuery<UserMaster>().ToList();
            var purchaseorderHist = _repository.GetQuery<PurchaseOrdHist>().Where(a => a.PurchaseOrderId == Id).OrderByDescending(a => a.CreatedDate).ToList();
            purchaseorderHist.ForEach(Hsty =>
            {
                var userName = usemaster.Where(a => a.Id == Hsty.CreatedBy).FirstOrDefault();
                if (userName != null)
                    Hsty.UserName = userName.FirstName + " " + userName.LastName;
            });
            return purchaseorderHist.OrderByDescending(a => a.CreatedDate).ToList();
        }

        public List<AppDocument> GetPurchaseOrderDetAttachments(Guid Id)
        {
            var usemaster = _repository.GetQuery<UserMaster>().ToList();

            var comments = _repository.GetQuery<PurchaseOrdDetComment>().Where(a => a.PurchaseOrderDetId == Id && a.Active == "Y").ToList();

            var fileAttachmentIds = new List<Guid>();
            fileAttachmentIds.AddRange(comments.Select(a => a.Id).Distinct().ToList());
            fileAttachmentIds.Add(Id);

            var appDoucuments = _repository.GetQuery<AppDocument>().Where(a => a.Active == "Y"
                    && (a.TransactionType == "PURCHASEORDERDETCOMM" || a.TransactionType == "PURCHASEORDER") && fileAttachmentIds.Contains(a.TransactionId))
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

        public AppResponse SendPurchaseOrderMail(Guid purchaseId)
        {
            VendorMailSend purchaseOrderMailSend = new VendorMailSend();
            List<PurchaseOrderDet> purchaseRequestDets = new List<PurchaseOrderDet>();
            PurchaseOrderDet purchaseOrder1 = new PurchaseOrderDet();
            var purReq = _repository.GetQuery<PurchaseRequest>();
            var purchaseOrder = _repository.GetById<PurchaseOrder>(purchaseId);
            purchaseRequestDets = _repository.GetQuery<PurchaseOrderDet>().Where(a => a.Active == "Y" && a.PurchaseOrderId == purchaseId).ToList();
            purchaseOrderMailSend.QuotionReqNo = purchaseOrder.TransNo;
            purchaseOrderMailSend.purchaseTransNo = purReq.FirstOrDefault(a => a.Id == purchaseOrder.PurchaseRequestId).TransNo;
            purchaseOrderMailSend.QuotationReqDate = purchaseOrder.TransDate;
            purchaseOrderMailSend.Remarks = purchaseOrder.Remarks;
            purchaseOrderMailSend.ExportHeaderText = "Purchase Order";

            var vendorList = _repository.List<VendorMaster>(q => q.Active == "Y");
            var vendorContact = _repository.List<VendorContact>(q => q.Active == "Y");

            if (purchaseOrder != null)
            {
                var productMailDetData = new VendorMailDet();
                var vendorResult = vendorList.FirstOrDefault(a => a.Active == "Y" && a.Id == purchaseOrder.VendorMasterId);

                if (vendorResult != null)
                {
                    productMailDetData.Name = vendorResult.Name;
                    productMailDetData.VendorName = $"{vendorResult.Title} {" "} {vendorResult.Name}";
                    productMailDetData.ToMail = vendorResult.Email;
                }

                productMailDetData.CCMail = vendorContact.Where(a => a.Active == "Y" && a.VendorMasterId == purchaseOrder.VendorMasterId).Select(a => a.EmailId).ToList();

                purchaseOrderMailSend.VendorMailDets.Add(productMailDetData);


            }

            if (purchaseRequestDets != null)
            {

                var units = _repository.GetQuery<ProdUnitMaster>().Where(a => a.Active == "Y").ToList();
                var products = _repository.GetQuery<ProductMaster>().Where(a => a.Active == "Y").ToList();

                foreach (var det in purchaseRequestDets)
                {
                    var product = products.FirstOrDefault(a => a.Id == det.ProductMasterId);
                    var unit = units.FirstOrDefault(a => a.Id == det.UnitMasterId);
                    det.ProductName = product != null ? product.ProdDescription : "";
                    det.UnitName = unit != null ? unit.UnitName : "";
                }
                purchaseOrder1.Amount = purchaseRequestDets.Sum(item => item.Amount);
                purchaseOrder1.Quantity = purchaseRequestDets.Sum(item => item.Quantity);
                purchaseOrder1.ProductName = "Total";
                purchaseRequestDets.Add(purchaseOrder1);
            }

            if (purchaseOrderMailSend != null && purchaseOrderMailSend.VendorMailDets != null)
            {
                purchaseOrderMailSend.VendorMailDets.ForEach(mail =>
                {
                    var vendorProduct = _repository.GetQuery<VendorMaster>().FirstOrDefault(q => q.Name == mail.Name);
                    if (vendorProduct != null)
                    {

                        var gridData = purchaseRequestDets;

                        int SNo = 1;

                        if (gridData != null)
                        {
                            gridData.ForEach(data =>
                            {
                                data.SNo = SNo;
                                SNo++;
                            });
                        }

                        var file = new QuotationDetSendMail<PurchaseOrderDet>
                        {
                            gridData = gridData,
                            _headerText = purchaseOrderMailSend.ExportHeaderText,
                            _repository = _repository,
                            _userContext = _userContext,
                            _logger = _logger,
                            vendorMailSend = purchaseOrderMailSend,

                        };
                        buff = file.getByte();

                        Guid guid = Guid.NewGuid();
                        string key = guid.ToString();

                        var stoemail = mail.ToMail;
                        var sccmail = mail.CCMail;
                        var femail = ERPSettings.APPSYSTEMSETTINGS[APPSystemsettingsKey.SENDEREMAILID];
                        var EmailTemlate = ERPSettings.PurchaseOrdMailTemplate;
                        string body = string.Empty;
                        var root = AppDomain.CurrentDomain.BaseDirectory; using (var reader = new StreamReader(root + EmailTemlate))
                        {
                            string readFile = reader.ReadToEnd();
                            string StrContent = string.Empty;
                            StrContent = readFile;
                            StrContent = StrContent.Replace("#VENDORNAME#", mail.VendorName);
                            StrContent = StrContent.Replace("#LASTDATE#", purchaseOrderMailSend.QuotationReqDate.AddMonths(1).ToShortDateString());
                            body = StrContent.ToString();
                        }

                        MailMessage mailMessage = new MailMessage(femail, stoemail);
                        mailMessage.Subject = ERPSettings.PurchaseOrderMailSubject;
                        foreach (string CCEmail in mail.CCMail)
                        {
                            mailMessage.CC.Add(new MailAddress(CCEmail));
                        }
                        mailMessage.Body = body;
                        mailMessage.IsBodyHtml = true;
                        mailMessage.Attachments.Add(new Attachment(new MemoryStream(buff), "Purchase_Order.pdf"));
                        EmailUtility.SendEmail(mailMessage);
                    }
                });

                return new AppResponse() { Status = APPMessageKey.DATASAVESUCSS };
            }
            else
            {
                var repsonse = new AppResponse() { Status = APPMessageKey.ONEORMOREERR, Messages = new List<string>() };
                repsonse.Messages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.USERNOTFOUDMAIL]);
                return repsonse;
            }

        }
    }

}