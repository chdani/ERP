using ERPService.Common;
using ERPService.Common.Interfaces;
using ERPService.Common.Shared;
using ERPService.DataModel.CTO;
using ERPService.DataModel.DTO;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

namespace ERPService.BC
{
    public class EmbPostPaymentBC
    {
        private ILogger _logger;
        private IRepository _repository;
        public EmbPostPaymentBC(ILogger logger, IRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public EmbPostPayment GetEmbPostPaymentById(Guid embPostPaymentId)
        {
            var hdr = _repository.Get<EmbPostPayment>(a => a.Id == embPostPaymentId);
            if (hdr != null)
            {
                hdr.EmbPostPaymentInvDet = _repository.GetQuery<EmbPostPaymentInvDet>().Where(a => a.EmbPostPaymentId == hdr.Id && a.Active == "Y").ToList();
                var prePayDetIds = hdr.EmbPostPaymentInvDet.Select(a => a.EmbPrePaymentInvDetId).ToList();
                var prePayInvDetails = _repository.GetQuery<EmbPrePaymentInvDet>().Where(a => prePayDetIds.Contains(a.Id)).ToList();
                foreach (var det in hdr.EmbPostPaymentInvDet)
                {
                    var prePayDet = prePayInvDetails.FirstOrDefault(a => a.Id == det.EmbPrePaymentInvDetId);
                    if (prePayDet != null)
                    {
                        det.EmbPrePaymentDueDet = new EmbPrePaymentDueDet()
                        {
                            DueAmount = prePayDet.Amount,
                            DetailId = prePayDet.Id,
                            InvDate = prePayDet.InvDate,
                            InvNo = prePayDet.InvNo,
                            Remarks = prePayDet.Remarks,
                            TelexRef = prePayDet.TelexRef
                        };
                    }
                }
            }
            return hdr;
        }

        public List<EmbPostPayment> GetEmbPostPaymentList(EmbPostPaymenteSearch search)
        {
            search.FromDate = search.FromDate.ToLocalTime().Date;
            search.ToDate = search.ToDate.ToLocalTime().Date;
            var paymentList = _repository.GetQuery<EmbPostPayment>().Where(a =>
               (string.IsNullOrEmpty(search.FinYear) || a.FinYear.Contains(search.FinYear))
             && (search.OrgId == Guid.Empty || search.OrgId == a.OrgId)
             && (string.IsNullOrEmpty(search.BookNo) || a.BookNo.Contains(search.BookNo))
             && (search.Amount <= 0 || a.Amount == search.Amount)
             && (search.CurrencyAmount <= 0 || a.Amount == search.CurrencyAmount)
             && ((search.FromDate <= DateTime.MinValue || search.ToDate <= DateTime.MinValue)
                       || (a.PaymentDate >= search.FromDate && a.PaymentDate <= search.ToDate))
             && a.Active == "Y").OrderByDescending(a=>a.CreatedDate).ToList();
            if (search.Status.Count > 0 && search.Status != null)
            {
                var filterStatus = paymentList.Where(a => search.Status.Contains(a.Status)).ToList();
                paymentList = filterStatus;
            }
            if (search.EmbassyId.Count > 0 && search.EmbassyId != null)
            {
                var filterEmbassy = paymentList.Where(a => search.EmbassyId.Contains(a.EmbassyId)).ToList();
                paymentList = filterEmbassy;
            }
            var det = _repository.GetQuery<EmbPostPaymentInvDet>();
            var embassyMasterBC = new EmbassyMasterBC(_logger, _repository);
            var embassyMaster = embassyMasterBC.GetEmbassyMasterList();
            var currencyMasterBC = new CurrencyMasterBC(_logger, _repository);
            var currencyMaster = currencyMasterBC.GetCurrencyMasterList();
            var LedgerAcct = _repository.GetQuery<LedgerAccount>();
            paymentList.ForEach(list =>
            {
                list.EmbPostPaymentInvDet = det.Where(a => a.EmbPostPaymentId == list.Id && a.Active == "Y").ToList();
                var EmbassyName = embassyMaster.Where(a => a.Id == list.EmbassyId).FirstOrDefault();
                var currencyName = currencyMaster.Where(a => a.Code == list.CurrencyCode).FirstOrDefault();
                var Ladger = LedgerAcct.Where(a => a.LedgerCode == list.LedgerCode).FirstOrDefault();
                list.Embassy = EmbassyName.Number.ToString() + '-' + EmbassyName.NameEng + '-' + EmbassyName.NameArabic;
                list.CurrencyDesc = currencyName == null ? "" : (currencyName.Code + '-' + currencyName.Name);
                list.Ledger = Ladger.LedgerCode + "-" + Ladger.LedgerDesc;
            });
            if (search.LedgerCode.Count > 0 && search.LedgerCode != null)
            {
                var all = paymentList.Where(a => search.LedgerCode.Contains(a.LedgerCode)).ToList();
                paymentList = all;
            }
            return paymentList;
        }

        public AppResponse SaveEmbPostPayment(EmbPostPayment embPayment)
        {

            AppResponse appResponse = new AppResponse();
            List<String> validationMessages = new List<string>();

            bool validation = true;
            if (embPayment == null)
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.PLZPROVIDEVALIDDATA]);
                validation = false;
            }


            if (embPayment.PaymentDate <= DateTime.MinValue || string.IsNullOrEmpty(embPayment.BookNo)
                || embPayment.Amount < 0 || embPayment.EmbassyId == Guid.Empty || string.IsNullOrEmpty(embPayment.FinYear))
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.MANDMISSING]);
                validation = false;
            }

            var embPostPayment = _repository.GetQuery<EmbPostPayment>().Where(a => a.Id != embPayment.Id &&
            (a.BookNo == embPayment.BookNo) && a.Active == "Y").FirstOrDefault();
            if (embPostPayment != null)
            {
                validationMessages.Add(String.Format(ERPExceptions.APP_MESSAGES[APPMessageKey.DUPBOOKNUMBER], embPayment.BookNo));
                validation = false;
            }
            decimal prevAmount = 0;

            if (embPayment.Id != Guid.Empty)
            {
                var prevValue = _repository.GetById<EmbPostPayment>(embPayment.Id);
                if (prevValue != null)
                    prevAmount = prevValue.Amount;
            }
            if (embPayment.EmbPostPaymentInvDet == null || embPayment.EmbPostPaymentInvDet.Count == 0)
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.PLZSECTSOMEINV]);
                validation = false;
            }
            if (embPayment.Status != "SUBMITTED")
            {
                validation = true;
            }

            if (validation)
            {
                var prePayInvDetIds = new List<Guid>();
                if (embPayment.EmbPostPaymentInvDet != null)
                    prePayInvDetIds = embPayment.EmbPostPaymentInvDet.Select(a => a.EmbPrePaymentInvDetId).ToList();

                var prePaymentInvDets = _repository.GetQuery<EmbPrePaymentInvDet>().Where(a => prePayInvDetIds.Contains(a.Id) && a.Active == "Y").ToList();
                if (prePaymentInvDets == null || prePayInvDetIds.Count != prePayInvDetIds.Count())
                {
                    validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.NOTSUFFBAL]);
                    validation = false;
                }
                else
                {
                    decimal amount = 0;
                    foreach (var det in prePaymentInvDets)
                        amount += det.DueAmount;

                    if (amount + prevAmount < embPayment.Amount)
                    {
                        validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.NOTSUFFBAL]);
                        validation = false;
                    }
                }

                if (validation)
                {
                    embPayment.PaymentDate = embPayment.PaymentDate.ToLocalTime();
                    appResponse.ReferenceId = InsertUpdateEmbPostPayment(embPayment, prePaymentInvDets);
                    appResponse.Status = APPMessageKey.DATASAVESUCSS;
                }
            }

            if (!validation)
            {
                appResponse.Status = APPMessageKey.ONEORMOREERR;
                appResponse.Messages = validationMessages;
            }
            return appResponse;
        }

        public AppResponse ApproveRejectEmbPostPayment(EmbPostPayment embPostPayment)
        {

            if (embPostPayment.Status == "APPROVED")
            {
                var prePayInvDetIds = new List<Guid>();
                if (embPostPayment.EmbPostPaymentInvDet != null)
                    prePayInvDetIds = embPostPayment.EmbPostPaymentInvDet.Select(a => a.EmbPrePaymentInvDetId).ToList();

                var prePayInvDetails = _repository.GetQuery<EmbPrePaymentInvDet>().Where(a => prePayInvDetIds.Contains(a.Id) && a.Active == "Y").ToList();

                var invIds = prePayInvDetails.Select(a => a.Id).ToList();

                var ledgerDrafts = _repository.GetQuery<LedgerBalanceDraft>().Where(a => invIds.Contains(a.TransactionId)
                    && a.TransactionType == ERPTransaction.EMB_PRE_PAYMENT && a.Active == "Y").ToList();

                foreach (var ledgerDraft in ledgerDrafts)
                {
                    var inv = prePayInvDetails.First(a => a.Id == ledgerDraft.TransactionId);
                    if (inv != null)
                    {
                        ledgerDraft.Amount = 0;
                        _repository.Update(ledgerDraft);
                    }
                }

                if (embPostPayment.Active == "Y")
                {
                    var ledgerBalance = new LedgerBalance()
                    {
                        FinYear = embPostPayment.FinYear,
                        Debit = embPostPayment.Amount,
                        Credit = 0,
                        LedgerCode = embPostPayment.LedgerCode,
                        LedgerDate = embPostPayment.PaymentDate,
                        Active = "Y",
                        OrgId = embPostPayment.OrgId,
                        TransactionId = embPostPayment.Id,
                        TransactionType = ERPTransaction.EMB_POST_PAYMENT,
                        Id = Guid.NewGuid(),
                        IsCommitted = true
                    };
                    _repository.Add(ledgerBalance, false);
                }
            }

            embPostPayment.EmbPostPaymentInvDet = null;
            InsertUpdateEmbPostPayment(embPostPayment, null);

            var appResponse = new AppResponse();
            appResponse.Status = APPMessageKey.DATASAVESUCSS;

            return appResponse;
        }

        private Guid InsertUpdateEmbPostPayment(EmbPostPayment embPostPayment, List<EmbPrePaymentInvDet> prePayInvDetails)
        {

            if (embPostPayment.Id == Guid.Empty)
            {
                embPostPayment.Id = Guid.NewGuid();
                if (embPostPayment.AppDocuments != null && embPostPayment.AppDocuments.Count > 0)
                {
                    foreach (var appDocument in embPostPayment.AppDocuments)
                    {
                        appDocument.TransactionId = embPostPayment.Id;
                    }
                    var appDocumentBC = new AppDocumentBC(_logger, _repository);
                    var AppDocument = appDocumentBC.saveAppDocument(embPostPayment.AppDocuments, false);
                }
                _repository.Add(embPostPayment, false);
                if (prePayInvDetails != null)
                {
                    foreach (var det in prePayInvDetails)
                        det.DueAmount = 0;
                }
            }
            else
            {
                if (prePayInvDetails != null)
                {
                    //Revert all the invoices due amount 
                    foreach (var det in prePayInvDetails)
                        det.DueAmount = det.Amount;
                }
                if (embPostPayment.AppDocuments != null && embPostPayment.AppDocuments.Count > 0)
                {
                    foreach (var appDocument in embPostPayment.AppDocuments)
                    {
                        appDocument.TransactionId = embPostPayment.Id;
                    }
                    var appDocumentBC = new AppDocumentBC(_logger, _repository);
                    var AppDocument = appDocumentBC.saveAppDocument(embPostPayment.AppDocuments, false);
                }
                _repository.Update(embPostPayment, false);

                //Dont update due amount for deleted records
                if (embPostPayment.Active == "Y" && prePayInvDetails != null)
                {
                    foreach (var det in prePayInvDetails)
                    {
                        var currentInvDet = embPostPayment.EmbPostPaymentInvDet.FirstOrDefault(a => a.EmbPrePaymentInvDetId == det.Id && a.Active == "Y");
                        if (currentInvDet != null)
                            det.DueAmount = 0;
                    }
                }

            }

            if (embPostPayment.EmbPostPaymentInvDet != null)
            {
                foreach (var det in embPostPayment.EmbPostPaymentInvDet)
                {
                    det.EmbPostPaymentId = embPostPayment.Id;
                    if (det.Id != Guid.Empty)
                        _repository.Update(det);
                    else
                    {
                        det.Id = Guid.NewGuid();
                        _repository.Add(det);
                    }
                }
            }
            InsertAuditForEmbPostPayment(embPostPayment);
            InsertEmbPostPaymentStatusHis(embPostPayment);
            _repository.SaveChanges();

            return embPostPayment.Id;
        }

        private void InsertAuditForEmbPostPayment(EmbPostPayment hdrInfo)
        {
            var prevValue = _repository.GetById<EmbPostPayment>(hdrInfo.Id);
            List<AppAudit> changedValues = new List<AppAudit>();
            if (prevValue != null)
                changedValues = AuditUtility.GetAuditableObject<EmbPostPayment>(prevValue, hdrInfo);
            else
            {
                changedValues = new List<AppAudit>();
                changedValues.Add(new AppAudit() { FieldName = "APP_RECORD_CREATED", NewValue = "", OldValue = "" });
            }
            foreach (var change in changedValues)
            {
                var empreHist = new EmbPostPaymentHist()
                {
                    Active = "Y",
                    CurrentValue = change.NewValue,
                    PrevValue = change.OldValue,
                    Id = Guid.NewGuid(),
                    FieldName = change.FieldName,
                    EmbPostPaymentId = hdrInfo.Id
                };
                _repository.Add(empreHist);
                if (change.FieldName == "APPROVER_REMARKS")
                    InsertEmbPostPaymentComments(hdrInfo, change.NewValue);
            }
        }

        private void InsertEmbPostPaymentStatusHis(EmbPostPayment hdrInfo)
        {
            var empPostStsHist = new EmbPostPaymentStatusHist()
            {
                Active = "Y",
                Status = hdrInfo.Status,
                Comments = hdrInfo.ApproverRemarks,
                EmbPostPaymentId = hdrInfo.Id,
                Id = Guid.NewGuid()
            };
            _repository.Add(empPostStsHist);
        }

        private void InsertEmbPostPaymentComments(EmbPostPayment hdrInfo, string updatedRemarks)
        {
            if (!string.IsNullOrEmpty(updatedRemarks))
            {
                var embPostComments = new EmbPostPaymentComment()
                {
                    Active = "Y",
                    Comments = updatedRemarks,
                    EmbPostPaymentId = hdrInfo.Id,
                    Id = Guid.NewGuid()
                };
                _repository.Add(embPostComments);
            }
        }

        ///////////Histroy && Comments /////


        public List<EmbPostPaymentComment> SaveEmbPostPaymentHdrHistComment(EmbPostPaymentComment postPaymentComment)
        {

            var appDocument1 = new AppDocument();
            var appDocumentList = new List<AppDocument>();
            postPaymentComment.Active = "Y";

            InsertUpdateComment(postPaymentComment);
            var id = postPaymentComment.Id;
            if (postPaymentComment.AppDocuments != null && postPaymentComment.AppDocuments.Count > 0)
            {
                foreach (var item in postPaymentComment.AppDocuments)
                {
                    appDocument1 = new AppDocument
                    {
                        Id = item.Id,
                        TransactionId = id,
                        TransactionType = item.TransactionType,
                        ExpiryDate = DateTime.Now,
                        FileContent = item.FileContent,
                        UniqueNumber = item.DocumentType,
                        DocumentType = item.DocumentType,
                        FileName = item.FileName,
                        CreatedBy = item.CreatedBy,
                        CreatedDate = item.CreatedDate,
                        ModifiedBy = item.ModifiedBy,
                        ModifiedDate = item.ModifiedDate,
                    };
                    appDocumentList.Add(appDocument1);
                }
                var appDocumentBC = new AppDocumentBC(_logger, _repository);
                appDocumentBC.saveAppDocument(appDocumentList, false);
            }


            _repository.SaveChanges();


            return GetEmbPostPaymentHdrHistComment(postPaymentComment.EmbPostPaymentId);
        }
        public List<EmbPostPaymentComment> GetEmbPostPaymentHdrHistComment(Guid Id)
        {
            var Comments = _repository.GetQuery<EmbPostPaymentComment>().Where(a => a.EmbPostPaymentId == Id).ToList();
            var commentIds = Comments.Select(a => a.Id).Distinct();

            var User = _repository.GetQuery<UserMaster>().ToList();

            var appDoucuments = _repository.GetQuery<AppDocument>().Where(a => a.Active == "Y"
                    && a.TransactionType == "EMBPOSTCOMMENTS" && commentIds.Contains(a.TransactionId)).ToList();

            Comments.ForEach(ele =>
            {
                var userName = User.Where(a => a.Id == ele.CreatedBy).FirstOrDefault();
                var appDoucument = appDoucuments.Where(a => a.TransactionId == ele.Id).ToList();
                ele.Id = ele.Id;
                ele.EmbPostPaymentId = ele.EmbPostPaymentId;
                ele.Comments = ele.Comments;
                ele.CreatedDate = ele.CreatedDate;
                if (userName != null)
                    ele.UserName = userName.FirstName + " " + userName.LastName;
                if (appDoucument != null)
                {
                    foreach (var doc in appDoucument)
                        doc.FileContent = null;

                    ele.AppDocuments = appDoucument;
                }

            });

            return Comments.OrderByDescending(a => a.CreatedDate).ToList();
        }
        private void InsertUpdateComment(EmbPostPaymentComment budgAllocHdrComment1)
        {
            try
            {
                if (budgAllocHdrComment1.Id == Guid.Empty)
                {
                    budgAllocHdrComment1.Id = Guid.NewGuid();
                    _repository.Add(budgAllocHdrComment1, true);

                }
                else
                {

                    _repository.Update(budgAllocHdrComment1, true);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<EmbPostPaymentStatusHist> GetEmbPostPaymentHistoryStatus(Guid id)
        {
            var usemaster = _repository.GetQuery<UserMaster>().Where(a => a.Active == "Y").ToList();
            var EmbPrePaymentHdr = _repository.GetQuery<EmbPostPaymentStatusHist>().Where(a => a.EmbPostPaymentId == id).OrderByDescending(a => a.CreatedDate).ToList();
            EmbPrePaymentHdr.ForEach(Status =>
            {
                var userName = usemaster.Where(a => a.Id == Status.CreatedBy).FirstOrDefault();
                Status.Id = Status.Id;
                Status.EmbPostPaymentId = Status.EmbPostPaymentId;
                Status.Comments = Status.Comments;
                Status.Status = Status.Status;
                Status.CreatedDate = Status.CreatedDate;
                if (userName != null)
                    Status.UserName = userName.FirstName + " " + userName.LastName;
            });

            return EmbPrePaymentHdr;
        }

        public List<EmbPostPaymentHist> GetEmbPostPaymentHistory(Guid id)
        {
            var usemaster = _repository.GetQuery<UserMaster>().Where(a => a.Active == "Y").ToList();
            var EmbPrePaymentHdr = _repository.GetQuery<EmbPostPaymentHist>().Where(a => a.EmbPostPaymentId == id).OrderByDescending(a => a.CreatedDate).ToList();
            EmbPrePaymentHdr.ForEach(Hsty =>
            {
                var userName = usemaster.Where(a => a.Id == Hsty.CreatedBy).FirstOrDefault();
                Hsty.Id = Hsty.Id;
                Hsty.EmbPostPaymentId = Hsty.EmbPostPaymentId;
                Hsty.FieldName = Hsty.FieldName;
                Hsty.PrevValue = Hsty.PrevValue;
                Hsty.CurrentValue = Hsty.CurrentValue;
                Hsty.CreatedDate = Hsty.CreatedDate;
                if (userName != null)
                    Hsty.UserName = userName.FirstName + " " + userName.LastName;
            });
            return EmbPrePaymentHdr;
        }

        public List<AppDocument> GetEmbPostPaymentAttachments(Guid id)
        {
            var usemaster = _repository.GetQuery<UserMaster>().ToList();

            var Comments = _repository.GetQuery<EmbPostPaymentComment>().Where(a => a.EmbPostPaymentId == id && a.Active == "Y").ToList();

            var fileAttachmentIds = new List<Guid>();
            fileAttachmentIds.AddRange(Comments.Select(a => a.Id).Distinct().ToList());
            fileAttachmentIds.Add(id);
            var appDoucuments = _repository.GetQuery<AppDocument>().Where(a => a.Active == "Y" && a.TransactionType == "EMBPOSTCOMMENTS"
                  && fileAttachmentIds.Contains(a.TransactionId))
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
    }
}