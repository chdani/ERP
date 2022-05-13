using ERPService.Common;
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
using System.Runtime.Caching;
using System.Text;


namespace ERPService.BC
{
    public class VendorQuotationBC
    {

        private ILogger _logger;
        private IRepository _repository;
        private UserContext _userContext;
        private ObjectCache cache = MemoryCache.Default;
        private object search;
        private byte[] buff;

        public VendorQuotationBC(ILogger logger, IRepository repository, UserContext userContext)
        {
            _logger = logger;
            _repository = repository;
            _userContext = userContext;
        }


        public AppResponse SaveVendorQuotation(VendorQuotation vendorQuotation)
        {
            List<String> validationMessages = new List<string>();
            AppResponse appResponse = new AppResponse();
            bool validation = true;



            var childValid = true;
            var childMessages = ValidateChildRecords(vendorQuotation.vendorquotationDets, out childValid);
            if (!childValid)
            {
                validation = false;
                validationMessages.AddRange(childMessages);
            }
            if (vendorQuotation != null && vendorQuotation.IsApproved == true)
            {
                var vendorQuo = _repository.GetQuery<VendorQuotation>().Where(a => a.QuotationRequestId == vendorQuotation.QuotationRequestId && a.IsApproved == true && a.Active == "Y").ToList();
                if (vendorQuo.Count > 0 && vendorQuo != null)
                {
                    validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.ALLREADYAPPROVE]);
                    validation = false;
                }
            }
            if (vendorQuotation != null)
            {
                var oldVendorQuoDet = _repository.GetQuery<VendorQuotationDet>().Where(a => a.VendorQuotationId == vendorQuotation.Id).ToList();
                if (oldVendorQuoDet != null && oldVendorQuoDet.Count > 0)
                {
                    foreach (var det in oldVendorQuoDet)
                    {
                        _repository.Delete(det);
                    }
                }

            }
            if (vendorQuotation.Id == Guid.Empty)
            {
                var vendorQuotationTransNoAndSeqNo = AppGeneralMethods.TranstypeSeqNumber("vendorQuotationTransType", _repository);
                vendorQuotation.TransNo = vendorQuotationTransNoAndSeqNo.Item1;
                vendorQuotation.SeqNo = vendorQuotationTransNoAndSeqNo.Item2;
            }


            if (vendorQuotation != null && validation)
            {
                vendorQuotation.TransDate = vendorQuotation.TransDate.ToLocalTime().Date;

                InsertUpdatevendorQuotation(vendorQuotation);
                appResponse.Status = APPMessageKey.DATASAVESUCSS;
            }
            else
            {
                appResponse.Status = APPMessageKey.ONEORMOREERR;
                appResponse.Messages = validationMessages;
            }
            return appResponse;
        }

        public AppResponse MarkVendorQuotationInactive(Guid id)
        {
            AppResponse appResponse = new AppResponse();
            List<String> validationMessages = new List<string>();

            bool validation = true;
            var vendor = _repository.GetQuery<VendorQuotation>().Where(a => a.Id == id).FirstOrDefault();
            if (vendor == null || vendor.Id == Guid.Empty)
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.RECNOTFOUND]);
                validation = false;
            }

            if (vendor != null && validation)
            {
                vendor.Active = "N";
                InsertUpdatevendorQuotation(vendor);
                appResponse.Status = APPMessageKey.DATASAVESUCSS;
            }
            else
            {
                appResponse.Status = APPMessageKey.ONEORMOREERR;
                appResponse.Messages = validationMessages;
            }

            return appResponse;
        }

        public AppResponse ApproveVendorQuotation(VendorQuotation vendorQuotation)
        {
            List<String> validationMessages = new List<string>();
            AppResponse appResponse = new AppResponse();
            bool validation = true;

            if (vendorQuotation.Id == Guid.Empty || string.IsNullOrEmpty(vendorQuotation.ApproverRemarks))
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.MANDMISSING]);
                validation = false;
            }
            if (validation)
            {
                var statusHistory = new VendorQuotationStatusHist()
                {
                    Active = "Y",
                    Comments = vendorQuotation.ApproverRemarks,
                    Status = vendorQuotation.Status,
                    VendorQuotationId = vendorQuotation.Id,
                };

                _repository.Update(vendorQuotation, false);
                InsertUpdateVendorQuoApproval(statusHistory, false);
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
        private void InsertUpdatevendorQuotation(VendorQuotation vendorQuotation)
        {
            var histories = new List<AppAudit>();
            if (vendorQuotation.Id == Guid.Empty)
            {
                histories.Add(new AppAudit() { FieldName = "APP_RECORD_CREATED", NewValue = "", OldValue = "" });
                vendorQuotation.Id = Guid.NewGuid();
                vendorQuotation.Active = "Y";
                _repository.Add(vendorQuotation, false);

                if (vendorQuotation.vendorquotationDets != null)
                    foreach (var det in vendorQuotation.vendorquotationDets)
                    {
                        det.VendorQuotationId = vendorQuotation.Id;
                        det.Active = "Y";
                        det.Id = Guid.Empty;


                        InsertUpdateQuotationRequestDet(det, true);

                    }
            }
            else
            {
                var OldValue = _repository.GetById<VendorQuotation>(vendorQuotation.Id);
                histories = AuditUtility.GetAuditableObject(OldValue, vendorQuotation);
                _repository.Update(vendorQuotation, false);



                if (vendorQuotation.vendorquotationDets != null)
                    foreach (var det in vendorQuotation.vendorquotationDets)
                    {

                        det.VendorQuotationId = vendorQuotation.Id;
                        det.Active = "Y";
                        det.Id = Guid.Empty;

                        InsertUpdateQuotationRequestDet(det, false);

                    }
            }
            if (histories != null)
            {
                histories.ForEach(Hdr =>
                {
                    var Hist = new VendorQuotationHist()
                    {
                        VendorQuotationId = vendorQuotation.Id,
                        FieldName = Hdr.FieldName,
                        PrevValue = Hdr.OldValue,
                        CurrentValue = Hdr.NewValue,
                        Active = "Y",
                        Id = Guid.NewGuid()

                    };
                    _repository.Add(Hist);
                });

            }
            if (vendorQuotation.AppDocuments != null && vendorQuotation.AppDocuments.Count > 0)
            {
                foreach (var appDocument in vendorQuotation.AppDocuments)
                {
                    appDocument.TransactionId = vendorQuotation.Id;
                }
                var appDocumentBC = new AppDocumentBC(_logger, _repository);
                var AppDocument = appDocumentBC.saveAppDocument(vendorQuotation.AppDocuments, false);
            }
            var statusHistory = new VendorQuotationStatusHist()
            {
                Active = "Y",
                Comments = vendorQuotation.Remarks,
                Status = "PURTRNSTSSUBMITTED",
                VendorQuotationId = vendorQuotation.Id,
            };

            InsertUpdateVendorQuoApproval(statusHistory, false);


            _repository.SaveChanges();
        }

        private void InsertUpdateVendorQuoApproval(VendorQuotationStatusHist statusHis, bool saveChanges)
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

        private void InsertUpdateQuotationRequestDet(VendorQuotationDet det, bool saveChanges)
        {
            var detailHistory = new List<AppAudit>();
            if (det.Id != Guid.Empty)
            {
                var OldDet = _repository.GetById<VendorQuotationDet>(det.Id);
                detailHistory = AuditUtility.GetAuditableObject<VendorQuotationDet>(OldDet, det);
            }
            else
                detailHistory.Add(new AppAudit() { FieldName = "APP_RECORD_CREATED", NewValue = "", OldValue = "" });
            if (det.Id == Guid.Empty)
            {
                det.Id = Guid.NewGuid();
                _repository.Add(det, false);
            }
            else
                _repository.Update(det, false);

            if (detailHistory != null && detailHistory.Count > 0)
            {
                detailHistory.ForEach(Hdr =>
                {
                    var Hist = new VendorQuotationDetHist()
                    {
                        Id = Guid.NewGuid(),
                        VendorQuotationDetId = det.Id,
                        FieldName = Hdr.FieldName,
                        PrevValue = Hdr.OldValue,
                        CurrentValue = Hdr.NewValue,
                        Active = "Y"

                    };
                    _repository.Add(Hist);
                });

            }

            if (saveChanges)
                _repository.SaveChanges();
        }



        private List<string> ValidateChildRecords(List<VendorQuotationDet> qutotionReqDets, out bool validation)
        {
            var validationMessages = new List<string>();
            validation = true;
            foreach (var det in qutotionReqDets)
            {
                if (det.ProductMasterId == Guid.Empty || det.Quantity <= 0 || det.UnitMasterId == Guid.Empty)
                {
                    validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.MANDMISSING]);
                    validation = false;
                }
            }

            return validationMessages;
        }




        public VendorQuotation GetVendorQuotationById(Guid quotationId)
        {
            VendorQuotation VendorQuotations = _repository.Get<VendorQuotation>(a => a.Id == quotationId && a.Active == "Y");
            VendorQuotations.vendaorData = _repository.GetQuery<VendorMaster>().Where(a => a.Id == VendorQuotations.VendorMasterId && a.Active == "Y").FirstOrDefault();
            VendorQuotations.vendorquotationDets = _repository.GetQuery<VendorQuotationDet>().Where(a => a.VendorQuotationId == VendorQuotations.Id && a.Active == "Y").ToList();
            VendorQuotations.quotationRequest = _repository.Get<QuotationRequest>(a => a.Id == VendorQuotations.QuotationRequestId && a.Active == "Y");
            VendorQuotations.quotationReqVendordet = _repository.List<QuotaReqVendorDet>(a => a.QuotationRequestId == VendorQuotations.quotationRequest.Id);


            var languageBC = new LangMasterBC(_logger, _repository);
            var units = languageBC.GetLangBasedDataForProdUnitMaster(_userContext.Language);
            var products = languageBC.GetLangBasedDataForProductMaster(_userContext.Language);

            foreach (var det in VendorQuotations.vendorquotationDets)
            {
                var product = products.FirstOrDefault(a => a.Id == det.ProductMasterId);
                var unit = units.FirstOrDefault(a => a.Id == det.UnitMasterId);
                det.ProductName = product != null ? product.ProdDescription : "";
                det.UnitName = unit != null ? unit.UnitName : "";
            }






            return VendorQuotations;

        }

        public List<VendorQuotation> GetVendorQuotationList()
        {
            List<VendorQuotation> VendorQuotations = null;
            VendorQuotations = _repository.GetQuery<VendorQuotation>().Where(a => a.Active == "Y").ToList();

            foreach (var VendorQuotation in VendorQuotations)
            {
                VendorQuotation.vendaorData = _repository.GetQuery<VendorMaster>().Where(a => a.Id == VendorQuotation.VendorMasterId && a.Active == "Y").FirstOrDefault();
                VendorQuotation.quotationRequest = _repository.GetQuery<QuotationRequest>().Where(a => a.Id == VendorQuotation.QuotationRequestId && a.Active == "Y").FirstOrDefault();
                VendorQuotation.vendorquotationDets = _repository.GetQuery<VendorQuotationDet>().Where(a => a.VendorQuotationId == VendorQuotation.Id && a.Active == "Y").ToList();


            }

            return VendorQuotations;
        }



        public List<VendorQuotationDet> GetVendorQuotationDetList(Guid quotationId)
        {
            List<VendorQuotationDet> VendorQuotationDets = null;
            VendorQuotationDets = _repository.GetQuery<VendorQuotationDet>().Where(a => a.VendorQuotationId == quotationId && a.Active == "Y").ToList();

            foreach (var VendorQuotationDet in VendorQuotationDets)
            {
                VendorQuotationDet.productDetail = _repository.GetQuery<ProductMaster>().Where(a => a.Id == VendorQuotationDet.ProductMasterId && a.Active == "Y").FirstOrDefault();

                VendorQuotationDet.prodUnit = _repository.GetQuery<ProdUnitMaster>().Where(a => a.Id == VendorQuotationDet.UnitMasterId && a.Active == "Y").FirstOrDefault();

            }




            return VendorQuotationDets;
        }



        public List<VendorQuotation> GetVendorQuotationSerachFilter(VendorQuotation vendorQuotation)
        {
            var vendorQuotationList = _repository.GetQuery<VendorQuotation>().Where(a =>
               (vendorQuotation.Id == Guid.Empty || a.Id == vendorQuotation.Id)
              && (vendorQuotation.QuotationRequestId == Guid.Empty || a.QuotationRequestId == vendorQuotation.QuotationRequestId)
              && (string.IsNullOrEmpty(vendorQuotation.TransNo) || a.TransNo == vendorQuotation.TransNo)
               && (string.IsNullOrEmpty(vendorQuotation.Remarks) || a.Remarks == vendorQuotation.Remarks)
                && (string.IsNullOrEmpty(vendorQuotation.Status) || a.Status == vendorQuotation.Status)
               && (vendorQuotation.IsApproved == false || a.IsApproved == vendorQuotation.IsApproved) && a.Active == "Y").OrderByDescending(a => a.CreatedDate).ToList();

            return vendorQuotationList;
        }
        public List<VendorQuotation> GetVendorQuotationListByActive(VendorQuotation vendorQuotation, bool isExport)
        {
            List<VendorQuotation> vendorQuotationList1 = new List<VendorQuotation>();
            vendorQuotation.TransDate = vendorQuotation.TransDate.ToLocalTime().Date;
            var vendorQuotationList = _repository.GetQuery<VendorQuotation>().Where(a =>
              (Guid.Empty == vendorQuotation.Id || a.Id == vendorQuotation.Id)
             && (Guid.Empty == vendorQuotation.QuotationRequestId || a.QuotationRequestId == vendorQuotation.QuotationRequestId)
             && (string.IsNullOrEmpty(vendorQuotation.TransNo) || a.TransNo == vendorQuotation.TransNo)
              && (string.IsNullOrEmpty(vendorQuotation.Remarks) || a.Remarks == vendorQuotation.Remarks) &&
               (vendorQuotation.TransDate <= DateTime.MinValue || vendorQuotation.TransDate <= a.TransDate)
               && (false == vendorQuotation.IsApproved || a.IsApproved == vendorQuotation.IsApproved)
               && (string.IsNullOrEmpty(vendorQuotation.Status) || a.Status == vendorQuotation.Status) && a.Active == "Y").OrderByDescending(a => a.CreatedDate).ToList();
            var vendorMasterList = _repository.GetQuery<VendorMaster>().Where(a => a.Active == "Y");
            var vendorQuoDetList = _repository.GetQuery<VendorQuotationDet>().Where(a => a.Active == "Y");
            var quotationReq = _repository.GetQuery<QuotationRequest>().Where(a => a.Active == "Y");
            vendorQuotationList.ForEach(vendorQuotation1 =>
            {
                var vendorMasterListById = vendorMasterList.FirstOrDefault(a => a.Id == vendorQuotation1.VendorMasterId);
                var vendorQuoDetListById = vendorQuoDetList.Where(a => a.VendorQuotationId == vendorQuotation1.Id).ToList();
                var quotationRequestById = quotationReq.FirstOrDefault(a => a.Id == vendorQuotation1.QuotationRequestId);
                vendorQuotation1.QuotationReqTransNo = quotationRequestById.TransNo;
                if (vendorQuoDetListById != null && vendorQuoDetListById.Count > 0)
                {
                    vendorQuotation1.vendorquotationDets = vendorQuoDetListById;
                    if (isExport)
                    {
                        var productMasterList = _repository.GetQuery<ProductMaster>().Where(a => a.Active == "Y");
                        var unitMasterList = _repository.GetQuery<ProdUnitMaster>().Where(a => a.Active == "Y");
                        vendorQuotation1.vendorquotationDets.ForEach(vendorQuoDet =>
                        {
                            var productNameById = productMasterList.FirstOrDefault(a => a.Id == vendorQuoDet.ProductMasterId);
                            var unitNameById = unitMasterList.FirstOrDefault(a => a.Id == vendorQuoDet.UnitMasterId);
                            vendorQuoDet.ProductName = productNameById?.ProdDescription;
                            vendorQuoDet.UnitName = unitNameById?.UnitName;
                            vendorQuoDet.Remark = vendorQuoDet.Remarks;
                        });
                    }
                }
                if (quotationRequestById != null)
                {
                    vendorQuotation1.quotationRequest = quotationRequestById;

                }
                if (vendorMasterListById != null)
                {
                    vendorQuotation1.vendaorData = vendorMasterListById;
                    vendorQuotation1.VendorName = vendorMasterListById.Name;

                }
            });
            if (vendorQuotation != null && vendorQuotation.IsApproved == false && vendorQuotation.Status == "PURTRNSTSSUBMITTED")
            {
                vendorQuotationList.ForEach(vendorQuo =>
                {
                    if (vendorQuo.IsApproved == false)
                    {
                        vendorQuotationList1.Add(vendorQuo);
                    }

                });
                return vendorQuotationList1;
            }
            else
                return vendorQuotationList;
        }
        public List<VendorQuotationDet> GetVendorQuotationDetById(Guid vendorId)
        {
            return _repository.GetQuery<VendorQuotationDet>().Where(a => a.VendorQuotationId == vendorId && a.Active == "Y").ToList();
        }

        public AppResponse SaveVendorQuotationComment(VendorQuotationComment vendorQuotationComment)
        {
            InsertVendorQuotationComments(vendorQuotationComment, true);
            return new AppResponse() { Status = APPMessageKey.DATASAVESUCSS };
        }
        private void InsertVendorQuotationComments(VendorQuotationComment comment, bool saveChanges)
        {
            comment.Id = Guid.NewGuid();
            _repository.Add(comment);

            if (comment.AppDocuments != null && comment.AppDocuments.Count > 0)
            {
                foreach (var document in comment.AppDocuments)
                {
                    document.TransactionId = comment.Id;
                    document.TransactionType = "VENDORQUOCOMM";
                    document.Active = "Y";
                }

                var appDocumentBC = new AppDocumentBC(_logger, _repository);
                appDocumentBC.saveAppDocument(comment.AppDocuments, false);
            }
            if (saveChanges)
                _repository.SaveChanges();

        }

        public List<VendorQuotationComment> GetVendorQuotationComment(Guid Id)
        {
            var comments = _repository.GetQuery<VendorQuotationComment>().Where(a => a.VendorQuotationId == Id).ToList();
            var commentIds = comments.Select(a => a.Id).Distinct();

            var User = _repository.GetQuery<UserMaster>().ToList();

            var appDoucuments = _repository.GetQuery<AppDocument>().Where(a => a.Active == "Y"
                    && a.TransactionType == "VENDORQUOCOMM" && commentIds.Contains(a.TransactionId)).ToList();

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
        public List<AppDocument> GetVendorQuotationAttachments(Guid Id)
        {
            var usemaster = _repository.GetQuery<UserMaster>().ToList();

            var comments = _repository.GetQuery<VendorQuotationComment>().Where(a => a.VendorQuotationId == Id && a.Active == "Y").ToList();

            var fileAttachmentIds = new List<Guid>();
            fileAttachmentIds.AddRange(comments.Select(a => a.Id).Distinct().ToList());
            fileAttachmentIds.Add(Id);

            var appDoucuments = _repository.GetQuery<AppDocument>().Where(a => a.Active == "Y"
                    && (a.TransactionType == "VENDORQUOCOMM" || a.TransactionType == "VENDORQUOTATION") && fileAttachmentIds.Contains(a.TransactionId))
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
        public List<VendorQuotationStatusHist> GetVendorQuotationStatusHistory(Guid Id)
        {
            var usemaster = _repository.GetQuery<UserMaster>().Where(a => a.Active == "Y").ToList();
            var approvals = _repository.GetQuery<VendorQuotationStatusHist>().Where(a => a.VendorQuotationId == Id).OrderByDescending(a => a.CreatedDate).ToList();
            approvals.ForEach(status =>
            {
                var userName = usemaster.Where(a => a.Id == status.CreatedBy).FirstOrDefault();
                if (userName != null)
                    status.UserName = userName.FirstName + " " + userName.LastName;
            });

            return approvals.OrderByDescending(a => a.CreatedDate).ToList();
        }
        public List<VendorQuotationHist> GetVendorQuotationHistory(Guid Id)
        {
            var usemaster = _repository.GetQuery<UserMaster>().ToList();
            var vendorQuotationHist = _repository.GetQuery<VendorQuotationHist>().Where(a => a.VendorQuotationId == Id).OrderByDescending(a => a.CreatedDate).ToList();
            vendorQuotationHist.ForEach(Hsty =>
            {
                var userName = usemaster.Where(a => a.Id == Hsty.CreatedBy).FirstOrDefault();
                if (userName != null)
                    Hsty.UserName = userName.FirstName + " " + userName.LastName;
            });
            return vendorQuotationHist.OrderByDescending(a => a.CreatedDate).ToList();
        }
        public AppResponse SaveVendorQuotationDetComment(VendorQuotationDetComment vendorQuotationDetComment)
        {
            InsertVendorQuoDetComments(vendorQuotationDetComment, true);
            return new AppResponse() { Status = APPMessageKey.DATASAVESUCSS };
        }
        private void InsertVendorQuoDetComments(VendorQuotationDetComment comment, bool saveChanges)
        {
            comment.Id = Guid.NewGuid();
            _repository.Add(comment);

            if (comment.AppDocuments != null && comment.AppDocuments.Count > 0)
            {
                foreach (var document in comment.AppDocuments)
                {
                    document.TransactionId = comment.Id;
                    document.TransactionType = "VENDORQUODET";
                    document.Active = "Y";
                }

                var appDocumentBC = new AppDocumentBC(_logger, _repository);
                appDocumentBC.saveAppDocument(comment.AppDocuments, false);
            }
            if (saveChanges)
                _repository.SaveChanges();

        }
        public List<VendorQuotationDetComment> GetVendorQuoDetComment(Guid Id)
        {
            var comments = _repository.GetQuery<VendorQuotationDetComment>().Where(a => a.VendorQuotationDetId == Id).ToList();
            var commentIds = comments.Select(a => a.Id).Distinct();

            var User = _repository.GetQuery<UserMaster>().ToList();

            var appDoucuments = _repository.GetQuery<AppDocument>().Where(a => a.Active == "Y"
                    && a.TransactionType == "VENDORQUODET" && commentIds.Contains(a.TransactionId)).ToList();

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
        public List<AppDocument> GetVendorQuoDetAttachments(Guid Id)
        {
            var usemaster = _repository.GetQuery<UserMaster>().ToList();

            var comments = _repository.GetQuery<VendorQuotationDetComment>().Where(a => a.VendorQuotationDetId == Id && a.Active == "Y").ToList();

            var fileAttachmentIds = new List<Guid>();
            fileAttachmentIds.AddRange(comments.Select(a => a.Id).Distinct().ToList());
            fileAttachmentIds.Add(Id);

            var appDoucuments = _repository.GetQuery<AppDocument>().Where(a => a.Active == "Y"
                    && (a.TransactionType == "VENDORQUODET") && fileAttachmentIds.Contains(a.TransactionId))
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
        public List<VendorQuotationDetHist> GetVendorQuoDetHistory(Guid Id)
        {
            var usemaster = _repository.GetQuery<UserMaster>().ToList();
            var vendorQuotationDetHist = _repository.GetQuery<VendorQuotationDetHist>().Where(a => a.VendorQuotationDetId == Id).OrderByDescending(a => a.CreatedDate).ToList();
            vendorQuotationDetHist.ForEach(Hsty =>
            {
                var userName = usemaster.Where(a => a.Id == Hsty.CreatedBy).FirstOrDefault();
                if (userName != null)
                    Hsty.UserName = userName.FirstName + " " + userName.LastName;
            });
            return vendorQuotationDetHist.OrderByDescending(a => a.CreatedDate).ToList();
        }

    }
}
