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
    public class QuotationRequestBC
    {
        private ILogger _logger;
        private IRepository _repository;
        private UserContext _userContext;
        private ObjectCache cache = MemoryCache.Default;
        private object search;
        private byte[] buff;

        public QuotationRequestBC(ILogger logger, IRepository repository, UserContext userContext)
        {
            _logger = logger;
            _repository = repository;
            _userContext = userContext;
        }

        public QuotationRequest GetQuotationRequestByTrans(string Trans)
        {
            // var quotationRequest = _repository.GetById<QuotationRequest>(requestId);

            var quotationRequest = _repository.GetQuery<QuotationRequest>().Where(a => a.TransNo == Trans && a.Active == "Y").FirstOrDefault();

            if (quotationRequest != null)
            {
                quotationRequest.QuotationReqDet = _repository.GetQuery<QuotationReqDet>().Where(a => a.Active == "Y" && a.QuotationRequestId == quotationRequest.Id).ToList();
                quotationRequest.QuotaReqVendorDets = _repository.GetQuery<QuotaReqVendorDet>().Where(a => a.QuotationRequestId == quotationRequest.Id).ToList();


                var languageBC = new LangMasterBC(_logger, _repository);
                var units = languageBC.GetLangBasedDataForProdUnitMaster(_userContext.Language);
                var products = languageBC.GetLangBasedDataForProductMaster(_userContext.Language);


                foreach (var det in quotationRequest.QuotationReqDet)
                {
                    var product = products.FirstOrDefault(a => a.Id == det.ProductMasterId);
                    var unit = units.FirstOrDefault(a => a.Id == det.UnitMasterId);
                    det.ProductName = product != null ? product.ProdDescription : "";
                    det.UnitName = unit != null ? unit.UnitName : "";
                }

            }
            return quotationRequest;
        }
        public QuotationRequest GetQuotationRequestById(Guid requestId)
        {
            var quotationRequest = _repository.GetById<QuotationRequest>(requestId);
            if (quotationRequest != null)
            {
                quotationRequest.QuotationReqDet = _repository.GetQuery<QuotationReqDet>().Where(a => a.Active == "Y" && a.QuotationRequestId == quotationRequest.Id).ToList();
                quotationRequest.QuotaReqVendorDets = _repository.GetQuery<QuotaReqVendorDet>().Where(a => a.Active == "Y" && a.QuotationRequestId == quotationRequest.Id).ToList();

                var languageBC = new LangMasterBC(_logger, _repository);
                var units = languageBC.GetLangBasedDataForProdUnitMaster(_userContext.Language);
                var products = languageBC.GetLangBasedDataForProductMaster(_userContext.Language);

                foreach (var det in quotationRequest.QuotationReqDet)
                {
                    var product = products.FirstOrDefault(a => a.Id == det.ProductMasterId);
                    var unit = units.FirstOrDefault(a => a.Id == det.UnitMasterId);
                    det.ProductName = product != null ? product.ProdDescription : "";
                    det.UnitName = unit != null ? unit.UnitName : "";
                }
            }
            return quotationRequest;
        }

        public AppResponse SaveQuotationRequest(QuotationRequest quotationRequest)
        {
            List<String> validationMessages = new List<string>();
            AppResponse appResponse = new AppResponse();
            bool validation = true;

            if (quotationRequest.TransDate <= DateTime.MinValue)
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.MANDMISSING]);
                validation = false;
            }

            if (quotationRequest.QuotationReqDet == null || quotationRequest.QuotationReqDet.Count == 0)
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.NOTDETROWEXIST]);
                validation = false;
            }
            else
            {
                var childValid = true;
                var childMessages = ValidateChildRecords(quotationRequest.QuotationReqDet, out childValid);
                if (!childValid)
                {
                    validation = false;
                    validationMessages.AddRange(childMessages);
                }
                if (quotationRequest.Active == "Y")
                {
                    var oldVendordet = _repository.GetQuery<QuotaReqVendorDet>().Where(a => a.QuotationRequestId == quotationRequest.Id).ToList();
                    foreach (var item in oldVendordet)
                    {
                        _repository.Delete(item);
                    }
                }
            }
            if (quotationRequest.Id == Guid.Empty)
            {
                var quotationRequestTransNoAndSeqNo = AppGeneralMethods.TranstypeSeqNumber("quotationRequestTransType", _repository);
                quotationRequest.TransNo = quotationRequestTransNoAndSeqNo.Item1;
                quotationRequest.SeqNo = quotationRequestTransNoAndSeqNo.Item2;
            }

            if (quotationRequest != null && validation)
            {
                quotationRequest.TransDate = quotationRequest.TransDate.ToLocalTime().Date;
                quotationRequest.Status = "PURTRNSTSSUBMITTED";
                InsertUpdateQutationRequest(quotationRequest);
                appResponse.Status = APPMessageKey.DATASAVESUCSS;
                appResponse.ReferenceId = quotationRequest.Id;
            }
            else
            {
                appResponse.Status = APPMessageKey.ONEORMOREERR;
                appResponse.Messages = validationMessages;
            }
            return appResponse;
        }
        private void InsertUpdateQutationRequest(QuotationRequest quotationRequest)
        {

            var histories = new List<AppAudit>();

            if (quotationRequest.Id == Guid.Empty)
            {
                histories.Add(new AppAudit() { FieldName = "APP_RECORD_CREATED", NewValue = "", OldValue = "" });
                quotationRequest.Id = Guid.NewGuid();
                _repository.Add(quotationRequest, false);
            }
            else
            {
                var OldValue = _repository.GetById<QuotationRequest>(quotationRequest.Id);
                histories = AuditUtility.GetAuditableObject(OldValue, quotationRequest);
                _repository.Update(quotationRequest, false);
            }

            if (histories != null)
            {
                histories.ForEach(Hdr =>
                {
                    var Hist = new QuotationRequestHist()
                    {
                        QuotationRequestId = quotationRequest.Id,
                        FieldName = Hdr.FieldName,
                        PrevValue = Hdr.OldValue,
                        CurrentValue = Hdr.NewValue,
                        Active = "Y",
                        Id = Guid.NewGuid()

                    };
                    _repository.Add(Hist);
                });

            }

            foreach (var det in quotationRequest.QuotationReqDet)
            {
                det.QuotationRequestId = quotationRequest.Id;
                InsertUpdateQuotationRequestDet(det, false);
            }
            foreach (var vendor in quotationRequest.QuotaReqVendorDets)
            {
                vendor.QuotationRequestId = quotationRequest.Id;
                InsertUpdateQuotaReqVendorDet(vendor, false);
            }

            var statusHistory = new QuotationReqStatusHist()
            {
                Active = "Y",
                Comments = quotationRequest.Remarks,
                Status = "PURTRNSTSSUBMITTED",
                QuotationRequestId = quotationRequest.Id,
            };

            InsertUpdateQuotatonRequestApproval(statusHistory, false);

            _repository.SaveChanges();
        }
        private void InsertUpdateQuotatonRequestApproval(QuotationReqStatusHist statusHis, bool saveChanges)
        {
            if (statusHis.Id == Guid.Empty)
            {
                statusHis.Id = Guid.NewGuid();
                _repository.Add(statusHis, false);
            }
            else
                _repository.Update(statusHis, false);

            var comment = new QuotationRequestComment()
            {
                Active = "Y",
                Comments = statusHis.Comments,
                Id = Guid.NewGuid(),
                QuotationRequestId = statusHis.QuotationRequestId,
            };

            if (saveChanges)
                _repository.SaveChanges();
        }
        private void InsertUpdateQuotaReqVendorDet(QuotaReqVendorDet vendor, bool saveChanges)
        {
            if (vendor.Id == Guid.Empty)
            {
                vendor.Id = Guid.NewGuid();
                _repository.Add(vendor, false);
            }
            else
                _repository.Update(vendor, false);

            if (saveChanges)
                _repository.SaveChanges();
        }
        private void InsertUpdateQuotationRequestDet(QuotationReqDet det, bool saveChanges)
        {
            var detailHistory = new List<AppAudit>();
            if (det.Id != Guid.Empty)
            {
                var OldDet = _repository.GetById<QuotationReqDet>(det.Id);
                detailHistory = AuditUtility.GetAuditableObject<QuotationReqDet>(OldDet, det);
            }
            else
                detailHistory.Add(new AppAudit() { FieldName = "APP_RECORD_CREATED", NewValue = "", OldValue = "" });

            if (det.Id == Guid.Empty)
            {
                det.Id = Guid.NewGuid();
                _repository.Add(det, false);
            }
            else
            {
                _repository.Update(det, false);
            }
            if (detailHistory != null && detailHistory.Count > 0)
            {
                detailHistory.ForEach(Hdr =>
                {
                    var Hist = new QuotationReqDetHist()
                    {
                        Id = Guid.NewGuid(),
                        QuotationReqDetId = det.Id,
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
        private List<string> ValidateChildRecords(List<QuotationReqDet> qutotionReqDets, out bool validation)
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
        public List<VendorProduct> GetVendorproductList()
        {
            return _repository.GetQuery<VendorProduct>().Where(a => a.Active == "Y").ToList();
        }
        public List<QuotationRequest> GetQuotationRequestList(ServiceRequestSearch search, bool isExport = false)
        {
            if (search.FromTransDate <= DateTime.MinValue && search.ToTransDate <= DateTime.MinValue)
            {
                search.FromTransDate = DateTime.Now.AddMonths(-1);
                search.ToTransDate = DateTime.Now;
            }
            search.FromTransDate = search.FromTransDate.ToLocalTime().Date;
            search.ToTransDate = search.ToTransDate.ToLocalTime().Date;

            var quotation = _repository.GetQuery<QuotationRequest>().Where(a => (search.FromTransDate <= DateTime.MinValue || a.TransDate >= search.FromTransDate)
             && (search.ToTransDate <= DateTime.MinValue || a.TransDate <= search.ToTransDate)
           && (string.IsNullOrEmpty(search.Status) || a.Status == search.Status)
             && a.Active == "Y").OrderByDescending(a => a.TransNo).ToList();
            if (quotation != null & quotation.Count > 0)
            {
                foreach (var master in quotation)
                {
                    master.QuotationReqDet = _repository.GetQuery<QuotationReqDet>().Where(a => a.QuotationRequestId == master.Id && a.Active == "Y").ToList();
                    master.QuotaReqVendorDets = _repository.GetQuery<QuotaReqVendorDet>().Where(a => a.QuotationRequestId == master.Id && a.Active == "Y").ToList();
                }
            }
            if (isExport)
            {
                var languageBC = new LangMasterBC(_logger, _repository);
                var units = languageBC.GetLangBasedDataForProdUnitMaster(_userContext.Language);
                var products = languageBC.GetLangBasedDataForProductMaster(_userContext.Language);

                foreach (var hdr in quotation)
                {
                    foreach (var det in hdr.QuotationReqDet)
                    {

                        var product = products.FirstOrDefault(a => a.Id == det.ProductMasterId);
                        var unit = units.FirstOrDefault(a => a.Id == det.UnitMasterId);
                        det.ProductName = product != null ? product.ProdDescription : "";
                        det.UnitName = unit != null ? unit.UnitName : "";
                        det.Remark = det.Remarks;
                    }
                }

            }
            return quotation;
        }
        public AppResponse SendVendorDetailsMail(Guid quotationId)
        {
            VendorMailSend vendorMailSend = new VendorMailSend();
            List<QuotationReqDet> QuotationReqDet = new List<QuotationReqDet>();
            var quotionReq = _repository.GetById<QuotationRequest>(quotationId);
            QuotationReqDet = _repository.GetQuery<QuotationReqDet>().Where(a => a.Active == "Y" && a.QuotationRequestId == quotationId).ToList();
            var vendorProd = _repository.GetQuery<QuotaReqVendorDet>().Where(q => q.Active == "Y" && q.QuotationRequestId == quotationId).Select(s => s.VendorMasterId).ToList();
            vendorMailSend.QuotionReqNo = quotionReq.TransNo;
            vendorMailSend.QuotationReqDate = quotionReq.TransDate;
            vendorMailSend.Remarks = quotionReq.Remarks;
            vendorMailSend.ExportHeaderText = "Quotation Request";

            var vendorList = _repository.List<VendorMaster>(q => q.Active == "Y");
            var vendorContact = _repository.List<VendorContact>(q => q.Active == "Y");

            if (quotionReq != null)
            {

                if (vendorProd != null)
                {

                    var UserMail = _repository.GetQuery<UserMaster>().Where(a => a.Active == "Y").ToList();
                    var approvalData = _repository.GetQuery<QuotationReqStatusHist>().Where(a => a.Status == "PURTRNSTSAPPROVED" && a.QuotationRequestId == quotationId).Select(a => a.CreatedBy);
                    vendorProd.ForEach(vendorProdId =>
                    {

                        var VendorMailDetData = new VendorMailDet();
                        var vendorResult = vendorList.FirstOrDefault(a => a.Active == "Y" && a.Id == vendorProdId);

                        if (vendorResult != null)
                        {
                            VendorMailDetData.Name = vendorResult.Name;
                            VendorMailDetData.VendorName = $"{vendorResult.Title} {" "} {vendorResult.Name}";
                            VendorMailDetData.ToMail = vendorResult.Email;


                            VendorMailDetData.CCMail = vendorContact.Where(a => a.Active == "Y" && a.VendorMasterId == vendorProdId).Select(a => a.EmailId).ToList();
                            var UserMailId = UserMail.FirstOrDefault(a => a.Id == quotionReq.CreatedBy);
                            var ApproveMail = UserMail.FirstOrDefault(a => approvalData.Contains(a.Id));

                            if (UserMailId.EmailId != null)
                                VendorMailDetData.CCMail.Add(UserMailId.EmailId);
                            if (ApproveMail.EmailId != null)
                                VendorMailDetData.CCMail.Add(ApproveMail.EmailId);

                        }
                        vendorMailSend.VendorMailDets.Add(VendorMailDetData);


                    });
                }
            }

            if (QuotationReqDet != null)
            {
                var languageBC = new LangMasterBC(_logger, _repository);
                var units = languageBC.GetLangBasedDataForProdUnitMaster(_userContext.Language);
                var products = languageBC.GetLangBasedDataForProductMaster(_userContext.Language);

                foreach (var det in QuotationReqDet)
                {
                    var product = products.FirstOrDefault(a => a.Id == det.ProductMasterId);
                    var unit = units.FirstOrDefault(a => a.Id == det.UnitMasterId);
                    det.ProductName = product != null ? product.ProdDescription : "";
                    det.UnitName = unit != null ? unit.UnitName : "";
                }
            }

            if (vendorMailSend != null && vendorMailSend.VendorMailDets != null)
            {
                vendorMailSend.VendorMailDets.ForEach(mail =>
                {
                    var vendorProduct = _repository.GetQuery<VendorMaster>().FirstOrDefault(q => q.Name == mail.Name);
                    if (vendorProduct != null)
                    {
                        var productIds = _repository.List<VendorProduct>(q => q.VendorMasterId == vendorProduct.Id).Select(s => s.ProductMasterId);
                        var gridData = QuotationReqDet.Where(q => productIds.Contains(q.ProductMasterId)).ToList();

                        int SNo = 1;

                        if (gridData != null)
                        {
                            gridData.ForEach(data =>
                            {
                                data.SNo = SNo;
                                SNo++;
                            });
                        }

                        var file = new QuotationDetSendMail<QuotationReqDet>
                        {
                            gridData = gridData,
                            _headerText = vendorMailSend.ExportHeaderText,
                            _repository = _repository,
                            _userContext = _userContext,
                            _logger = _logger,
                            vendorMailSend = vendorMailSend,

                        };
                        buff = file.getByte();

                        Guid guid = Guid.NewGuid();
                        string key = guid.ToString();

                        var stoemail = mail.ToMail;
                        var sccmail = mail.CCMail;
                        var femail = ERPSettings.APPSYSTEMSETTINGS[APPSystemsettingsKey.SENDEREMAILID];
                        var VendorEmailTemlate = ERPSettings.QuotationRequestMail;
                        string body = string.Empty;
                        var root = AppDomain.CurrentDomain.BaseDirectory; using (var reader = new StreamReader(root + VendorEmailTemlate))
                        {
                            string readFile = reader.ReadToEnd();
                            string StrContent = string.Empty;
                            StrContent = readFile;
                            StrContent = StrContent.Replace("#VENDORNAME#", mail.VendorName);
                            StrContent = StrContent.Replace("#LASTDATE#", vendorMailSend.QuotationReqDate.AddMonths(1).ToLongDateString());
                            body = StrContent.ToString();
                        }

                        MailMessage mailMessage = new MailMessage(femail, stoemail);
                        mailMessage.Subject = ERPSettings.QuotationRequestSubject;
                        foreach (string CCEmail in mail.CCMail)
                        {
                            mailMessage.CC.Add(new MailAddress(CCEmail));
                        }
                        mailMessage.Body = body;
                        mailMessage.IsBodyHtml = true;
                        mailMessage.Attachments.Add(new Attachment(new MemoryStream(buff), "Quotation_Request.pdf"));
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

        public AppResponse ApproveQuotationRequest(QuotationRequest quotationRequest)
        {
            List<String> validationMessages = new List<string>();
            AppResponse appResponse = new AppResponse();
            bool validation = true;

            if (quotationRequest.Id == Guid.Empty || string.IsNullOrEmpty(quotationRequest.ApproverRemarks))
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.MANDMISSING]);
                validation = false;
            }


            if (validation)
            {
                var statusHistory = new QuotationReqStatusHist()
                {
                    Active = "Y",
                    Comments = quotationRequest.ApproverRemarks,
                    Status = quotationRequest.Status,
                    QuotationRequestId = quotationRequest.Id,
                };

                if (quotationRequest.Status == "PURTRNSTSAPPROVED")
                {
                    appResponse.ReferenceId = quotationRequest.Id;
                    _repository.Update(quotationRequest, false);
                    InsertUpdateQuotationRequestApproval(statusHistory, false);
                    _repository.SaveChanges();
                    appResponse.Status = APPMessageKey.DATASAVESUCSS;
                }

                else
                {
                    _repository.Update(quotationRequest, false);
                    InsertUpdateQuotationRequestApproval(statusHistory, false);
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

        private void InsertUpdateQuotationRequestApproval(QuotationReqStatusHist statusHis, bool saveChanges)
        {
            if (statusHis.Id == Guid.Empty)
            {
                statusHis.Id = Guid.NewGuid();
                _repository.Add(statusHis, false);
            }
            else
                _repository.Update(statusHis, false);

            var comment = new QuotationRequestComment()
            {
                Active = "Y",
                Comments = statusHis.Comments,
                Id = Guid.NewGuid(),
                QuotationRequestId = statusHis.QuotationRequestId,
            };


            if (saveChanges)
                _repository.SaveChanges();
        }
        public List<QuotationReqDetComment> GetQuotationReqDetComment(Guid id)
        {
            var comments = _repository.GetQuery<QuotationReqDetComment>().Where(a => a.QuotationReqDetId == id).ToList();
            var commentIds = comments.Select(a => a.Id).Distinct();

            var User = _repository.GetQuery<UserMaster>().ToList();

            var appDoucuments = _repository.GetQuery<AppDocument>().Where(a => a.Active == "Y"
                    && a.TransactionType == "QUOTATIONDETCOMM" && commentIds.Contains(a.TransactionId)).ToList();

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
        public AppResponse SaveQuotationReqDetComment(QuotationReqDetComment quotationReqDetComment)
        {
            InsertQuotationReqDetComments(quotationReqDetComment, true);
            return new AppResponse() { Status = APPMessageKey.DATASAVESUCSS };
        }
        private void InsertQuotationReqDetComments(QuotationReqDetComment comment, bool saveChanges)
        {
            comment.Id = Guid.NewGuid();
            _repository.Add(comment);

            if (comment.AppDocuments != null && comment.AppDocuments.Count > 0)
            {
                foreach (var document in comment.AppDocuments)
                {
                    document.TransactionId = comment.Id;
                    document.TransactionType = "QUOTATIONDETCOMM";
                    document.Active = "Y";
                }
                var appDocumentBC = new AppDocumentBC(_logger, _repository);
                appDocumentBC.saveAppDocument(comment.AppDocuments, false);
            }
            if (saveChanges)
                _repository.SaveChanges();

        }
        public List<QuotationReqDetHist> GetQuotationReqDetHistory(Guid id)
        {
            var usemaster = _repository.GetQuery<UserMaster>().ToList();
            var purchaseorderDetIdHist = _repository.GetQuery<QuotationReqDetHist>().Where(a => a.QuotationReqDetId == id).OrderByDescending(a => a.CreatedDate).ToList();
            purchaseorderDetIdHist.ForEach(Hsty =>
            {
                var userName = usemaster.Where(a => a.Id == Hsty.CreatedBy).FirstOrDefault();
                if (userName != null)
                    Hsty.UserName = userName.FirstName + " " + userName.LastName;
            });
            return purchaseorderDetIdHist.OrderByDescending(a => a.CreatedDate).ToList();
        }
        public AppResponse SaveQuotationRequestComment(QuotationRequestComment purchaseOrd)
        {
            InsertQuotationReqComments(purchaseOrd, true);
            return new AppResponse() { Status = APPMessageKey.DATASAVESUCSS };
        }
        private void InsertQuotationReqComments(QuotationRequestComment comment, bool saveChanges)
        {
            comment.Id = Guid.NewGuid();
            _repository.Add(comment);

            if (comment.AppDocuments != null && comment.AppDocuments.Count > 0)
            {
                foreach (var document in comment.AppDocuments)
                {
                    document.TransactionId = comment.Id;
                    document.TransactionType = "QUOTATIONREQCOMM";
                    document.Active = "Y";
                }

                var appDocumentBC = new AppDocumentBC(_logger, _repository);
                appDocumentBC.saveAppDocument(comment.AppDocuments, false);
            }
            if (saveChanges)
                _repository.SaveChanges();

        }
        public List<QuotationRequestComment> GetQuotationReqComment(Guid Id)
        {
            var comments = _repository.GetQuery<QuotationRequestComment>().Where(a => a.QuotationRequestId == Id).ToList();
            var commentIds = comments.Select(a => a.Id).Distinct();

            var User = _repository.GetQuery<UserMaster>().ToList();

            var appDoucuments = _repository.GetQuery<AppDocument>().Where(a => a.Active == "Y"
                    && a.TransactionType == "QUOTATIONREQCOMM" && commentIds.Contains(a.TransactionId)).ToList();

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
        public List<AppDocument> GetQuotationReqAttachments(Guid Id)
        {
            var usemaster = _repository.GetQuery<UserMaster>().ToList();

            var comments = _repository.GetQuery<QuotationRequestComment>().Where(a => a.QuotationRequestId == Id && a.Active == "Y").ToList();

            var fileAttachmentIds = new List<Guid>();
            fileAttachmentIds.AddRange(comments.Select(a => a.Id).Distinct().ToList());
            fileAttachmentIds.Add(Id);

            var appDoucuments = _repository.GetQuery<AppDocument>().Where(a => a.Active == "Y"
                    && (a.TransactionType == "QUOTATIONREQCOMM" || a.TransactionType == "QUOTATIONREQ") && fileAttachmentIds.Contains(a.TransactionId))
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
        public List<AppDocument> GetQuotationReqDetAttachments(Guid Id)
        {
            var usemaster = _repository.GetQuery<UserMaster>().ToList();

            var comments = _repository.GetQuery<QuotationReqDetComment>().Where(a => a.QuotationReqDetId == Id && a.Active == "Y").ToList();

            var fileAttachmentIds = new List<Guid>();
            fileAttachmentIds.AddRange(comments.Select(a => a.Id).Distinct().ToList());
            fileAttachmentIds.Add(Id);

            var appDoucuments = _repository.GetQuery<AppDocument>().Where(a => a.Active == "Y"
                    && (a.TransactionType == "QUOTATIONDETCOMM" || a.TransactionType == "QUOTATIONREQ") && fileAttachmentIds.Contains(a.TransactionId))
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
        public List<QuotationReqStatusHist> GetQuotationReqStatusHistory(Guid Id)
        {
            var usemaster = _repository.GetQuery<UserMaster>().Where(a => a.Active == "Y").ToList();
            var approvals = _repository.GetQuery<QuotationReqStatusHist>().Where(a => a.QuotationRequestId == Id).OrderByDescending(a => a.CreatedDate).ToList();
            approvals.ForEach(status =>
            {
                var userName = usemaster.Where(a => a.Id == status.CreatedBy).FirstOrDefault();
                if (userName != null)
                    status.UserName = userName.FirstName + " " + userName.LastName;
            });

            return approvals.OrderByDescending(a => a.CreatedDate).ToList();
        }
        public List<QuotationRequestHist> GetQuotationReqHistory(Guid Id)
        {
            var usemaster = _repository.GetQuery<UserMaster>().ToList();
            var purchaseorderHist = _repository.GetQuery<QuotationRequestHist>().Where(a => a.QuotationRequestId == Id).OrderByDescending(a => a.CreatedDate).ToList();
            purchaseorderHist.ForEach(Hsty =>
            {
                var userName = usemaster.Where(a => a.Id == Hsty.CreatedBy).FirstOrDefault();
                if (userName != null)
                    Hsty.UserName = userName.FirstName + " " + userName.LastName;
            });
            return purchaseorderHist.OrderByDescending(a => a.CreatedDate).ToList();
        }
    }
}
