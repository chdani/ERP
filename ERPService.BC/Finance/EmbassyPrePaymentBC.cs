
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
    public class EmbassyPrePaymentBC
    {
        private ILogger _logger;
        private IRepository _repository;
        public EmbassyPrePaymentBC(ILogger logger, IRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public EmbPrePaymentHdr GetEmbPrePaymentById(Guid embPrePaymentId)
        {
            List<EmbPrePaymentInvDet> embPrePaymentInvDets = new List<EmbPrePaymentInvDet>();
            var hdr = _repository.Get<EmbPrePaymentHdr>(a => a.Id == embPrePaymentId);
            hdr.EmbPrePaymentEmbDet = _repository.GetQuery<EmbPrePaymentEmbDet>().Where(a => a.EmbPrePaymentHdrId == embPrePaymentId && a.Active == "Y").OrderByDescending(a => a.CreatedDate).ToList();
            var embIds = hdr.EmbPrePaymentEmbDet.Select(a => a.Id).ToList();
            var invDetails = _repository.GetQuery<EmbPrePaymentInvDet>().Where(a => embIds.Contains(a.EmbPrePaymentEmbDetId) && a.Active == "Y");
            foreach (var embDet in hdr.EmbPrePaymentEmbDet)
            {
                var invDet = invDetails.Where(a => a.EmbPrePaymentEmbDetId == embDet.Id).OrderByDescending(a => a.CreatedDate).ToList();
                if (invDet != null)
                    embDet.EmbPrePaymentInvDet = invDet;
            }
            return hdr;
        }

        public List<EmbPrePaymentEmbDet> GetEmbPrePaymentDetByHdrId(Guid embPrePaymentId)
        {
            return _repository.GetQuery<EmbPrePaymentEmbDet>().Where(a => a.EmbPrePaymentHdrId == embPrePaymentId && a.Active == "Y").ToList();
        }

        public List<EmbPrePaymentInvDet> GetEmbPrePayInvDetByEmbDetId(Guid embPrePaymenEmbDetId)
        {
            var LedgerCode = _repository.List<LedgerAccount>(a => a.Active == "Y");
            var invDet = _repository.GetQuery<EmbPrePaymentInvDet>().Where(a => a.EmbPrePaymentEmbDetId == embPrePaymenEmbDetId && a.Active == "Y").ToList();
            invDet.ForEach(q =>
            {
                var ledger = LedgerCode.FirstOrDefault(a => a.LedgerCode == q.LedgerCode);
                q.ledgerDecs = $"{ledger.LedgerCode}{"-"}{ledger.LedgerDesc}";
            });
            return invDet;
        }

        public List<EmbPrePaymentDue> GetPrePaymentDues(EmbPrePaymentDueSearch search)
        {
            var prePayEmbQuery = _repository.GetQuery<EmbPrePaymentEmbDet>();
            var prePayInvDetQry = _repository.GetQuery<EmbPrePaymentInvDet>();

            var queryBal = (from hdr in prePayEmbQuery
                            join det in prePayInvDetQry on hdr.Id equals det.EmbPrePaymentEmbDetId
                            where
                            hdr.Active == "Y" && det.Active == "Y"
                            && (search.FinYear == hdr.FinYear)
                            && (search.OrgId == hdr.OrgId)
                            && det.DueAmount > 0
                            select new { hdr, det }).ToList();

            var balanceList = new List<EmbPrePaymentDue>();
            foreach (var bal in queryBal)
            {
                var balance = balanceList.Where(a => a.Id == bal.hdr.Id).FirstOrDefault();
                if (balance == null)
                {
                    balance = new EmbPrePaymentDue()
                    {
                        Id = bal.hdr.Id,
                        FinYear = bal.hdr.FinYear,
                        OrgId = bal.hdr.OrgId,
                        DueDetail = new List<EmbPrePaymentDueDet>()
                    };
                    balanceList.Add(balance);
                }

                balance.DueDetail.Add(new EmbPrePaymentDueDet()
                {
                    DueAmount = bal.det.DueAmount,
                    DetailId = bal.det.Id,
                    InvDate = bal.det.InvDate,
                    InvNo = bal.det.InvNo,
                    Remarks = bal.det.Remarks,
                    TelexRef = bal.det.TelexRef
                });
            }
            return balanceList;
        }

        public List<EmbPrePaymentHdr> GetEmbPrePaymentList(EmbPrePaymenteSearch search, bool isExport)
        {
            search.FromBookDate = search.FromBookDate.ToLocalTime().Date;
            search.ToBookDate = search.ToBookDate.ToLocalTime().Date;

            var paymentList = _repository.GetQuery<EmbPrePaymentHdr>().Where(a =>
               (string.IsNullOrEmpty(search.FinYear) || a.FinYear.Contains(search.FinYear))
             && (search.OrgId == Guid.Empty || search.OrgId == a.OrgId)
             && (string.IsNullOrEmpty(search.BookNo) || a.BookNo.Contains(search.BookNo))
             && ((search.FromBookDate <= DateTime.MinValue || search.ToBookDate <= DateTime.MinValue)
                       || (a.BookDate >= search.FromBookDate && a.BookDate <= search.ToBookDate))
             && a.Active == "Y").OrderByDescending(a => a.CreatedDate).ToList();
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
            var empdetails = _repository.GetQuery<EmbPrePaymentEmbDet>().Where(a => a.Active == "Y");
            var empintdetail = _repository.GetQuery<EmbPrePaymentInvDet>().Where(a => a.Active == "Y");
            var LedgerCode = _repository.List<LedgerAccount>(a => a.Active == "Y");
            var embassyMasterBC = new EmbassyMasterBC(_logger, _repository);
            var embassyMaster = embassyMasterBC.GetEmbassyMasterList();
            var currencyMasterBC = new CurrencyMasterBC(_logger, _repository);
            var currencyMaster = currencyMasterBC.GetCurrencyMasterList();
            paymentList.ForEach(hdr =>
            {
                var EmbassyName = embassyMaster.Where(a => a.Id == hdr.EmbassyId).FirstOrDefault();
                var currencyName = currencyMaster.Where(a => a.Code == hdr.CurrencyCode).FirstOrDefault();
                hdr.Embassy = EmbassyName.Number.ToString() + '-' + EmbassyName.NameEng + '-' + EmbassyName.NameArabic;
                hdr.CurrencyDesc = currencyName == null ? "" : (currencyName.Code + '-' + currencyName.Name);
                var empdetail = empdetails.Where(a => a.EmbPrePaymentHdrId == hdr.Id).OrderByDescending(a => a.CreatedDate).ToList();
                empdetail.ForEach(dtl =>
                {
                    var invDet = empintdetail.Where(a => a.EmbPrePaymentEmbDetId == dtl.Id).OrderByDescending(a => a.CreatedDate).ToList();
                    invDet.ForEach(q =>
                    {
                        var ledger = LedgerCode.FirstOrDefault(a => a.LedgerCode == q.LedgerCode);
                        q.ledgerDecs = $"{ledger.LedgerCode}{"-"}{ledger.LedgerDesc}";
                    });
                    dtl.EmbPrePaymentInvDet = invDet;
                });

                hdr.EmbPrePaymentEmbDet = empdetail;
            });

            return paymentList;
        }

        public AppResponse SaveEmbPrePaymentList(List<EmbPrePaymentHdr> prePayHdr)
        {
            AppResponse appResponse = new AppResponse();
            foreach (var payment in prePayHdr)
            {
                appResponse = SaveEmbPrePayment(payment, false);
                if (appResponse.Status != APPMessageKey.DATASAVESUCSS)
                    break;
            }
            if (appResponse.Status == APPMessageKey.DATASAVESUCSS)
            {
                try
                {
                    _repository.SaveChanges();
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
        public AppResponse SaveEmbPrePayment(EmbPrePaymentHdr prePayHdr, bool saveChanges = true)
        {
            AppResponse appResponse = new AppResponse();
            List<String> validationMessages = new List<string>();

            bool validation = true;
            if (prePayHdr == null)
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.PLZPROVIDEVALIDDATA]);
                validation = false;
            }

            if (prePayHdr.EmbPrePaymentEmbDet == null || prePayHdr.EmbPrePaymentEmbDet.Count == 0)
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.PLZPROVIDEEMBDATA]);
                validation = false;
            }

            if (prePayHdr.BookDate <= DateTime.MinValue || string.IsNullOrEmpty(prePayHdr.BookNo)
                || string.IsNullOrEmpty(prePayHdr.FinYear))
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.MANDMISSING]);
                validation = false;
            }


            var embPrePayment = _repository.GetQuery<EmbPrePaymentHdr>().Where(a => a.Id != prePayHdr.Id &&
            (a.BookNo == prePayHdr.BookNo) && a.Active == "Y").FirstOrDefault();
            if (embPrePayment != null)
            {
                validationMessages.Add(String.Format(ERPExceptions.APP_MESSAGES[APPMessageKey.DUPBOOKNUMBER], prePayHdr.BookNo));
                validation = false;
            }

            if (!ValidatePrePaymentEmbDet(prePayHdr.EmbPrePaymentEmbDet.ToList(), validationMessages))
                validation = false;

            if (!prePayHdr.UserConsent && validation)
            {
                var listOfTelex = new List<string>();
                var listOfIds = new List<Guid>();

                foreach (var emb in prePayHdr.EmbPrePaymentEmbDet)
                {
                    listOfTelex.AddRange(emb.EmbPrePaymentInvDet.Select(a => a.TelexRef.ToUpper()).Distinct());
                    listOfIds.AddRange(emb.EmbPrePaymentInvDet.Select(a => a.Id).Distinct());

                }
                var existingInvDet = _repository.GetQuery<EmbPrePaymentInvDet>().Where(a => listOfTelex.Contains(a.TelexRef.ToUpper()) && !listOfIds.Contains(a.Id));
                if (existingInvDet.Count() > 0)
                {
                    var dupTelex = existingInvDet.Select(a => a.TelexRef.ToUpper()).Distinct();
                    appResponse.Status = APPMessageKey.DUPLICATETELEX;
                    appResponse.Messages = new List<string>();
                    appResponse.Messages.Add(string.Join(",", dupTelex));
                    return appResponse;
                }

            }
            if (validation)
            {
                prePayHdr.BookDate = prePayHdr.BookDate.ToLocalTime();
                InsertUpdateEmbPrePayment(prePayHdr, saveChanges);
                appResponse.Status = APPMessageKey.DATASAVESUCSS;
            }
            else
            {
                appResponse.Status = APPMessageKey.ONEORMOREERR;
                appResponse.Messages = validationMessages;
            }
            return appResponse;
        }
        private bool ValidatePrePaymentEmbDet(List<EmbPrePaymentEmbDet> details, List<string> validations)
        {
            var validation = true;
            foreach (var det in details)
            {

                if (validation)
                    validation = ValidatePrePaymentInvDet(det.EmbPrePaymentInvDet.ToList(), validations);

                if (!validation) return validation;

            }
            return validation;
        }

        private bool ValidatePrePaymentInvDet(List<EmbPrePaymentInvDet> details, List<string> validations)
        {
            var validation = true;
            foreach (var det in details)
            {
                if (det.Amount < 0 || det.InvDate <= DateTime.MinValue)
                {
                    validation = false;
                    validations.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.PLZFILLNECESSINVDET]);
                    break;
                }
            }
            return validation;
        }


        private void InsertUpdateEmbPrePayment(EmbPrePaymentHdr embPrePayment, bool saveChanges)
        {
            if (embPrePayment.Id == Guid.Empty)
            {
                embPrePayment.Id = Guid.NewGuid();
                _repository.Add(embPrePayment, false);
                if (embPrePayment.AppDocuments != null && embPrePayment.AppDocuments.Count > 0)
                {
                    foreach (var appDocument in embPrePayment.AppDocuments)
                    {
                        appDocument.TransactionId = embPrePayment.Id;
                    }
                    var appDocumentBC = new AppDocumentBC(_logger, _repository);
                    var AppDocument = appDocumentBC.saveAppDocument(embPrePayment.AppDocuments, false);
                }

                if (embPrePayment.EmbPrePaymentEmbDet != null)
                {
                    foreach (var det in embPrePayment.EmbPrePaymentEmbDet)
                    {
                        det.EmbPrePaymentHdrId = embPrePayment.Id;
                        InsertUpdateEmbPrePaymentEmbDet(det, embPrePayment.Status, false);
                    }
                }
            }
            else
            {
                _repository.Update(embPrePayment, false);
                if (embPrePayment.AppDocuments != null && embPrePayment.AppDocuments.Count > 0)
                {
                    foreach (var appDocument in embPrePayment.AppDocuments)
                    {
                        appDocument.TransactionId = embPrePayment.Id;
                    }
                    var appDocumentBC = new AppDocumentBC(_logger, _repository);
                    var AppDocument = appDocumentBC.saveAppDocument(embPrePayment.AppDocuments, false);
                }
                if (embPrePayment.EmbPrePaymentEmbDet != null)
                {
                    foreach (var det in embPrePayment.EmbPrePaymentEmbDet)
                    {
                        det.EmbPrePaymentHdrId = embPrePayment.Id;
                        InsertUpdateEmbPrePaymentEmbDet(det, embPrePayment.Status, false);
                    }
                }
            }
            InsertAuditForEmbPrePaymentHdr(embPrePayment);
            InsertEmbPrePaymentHdrStatusHis(embPrePayment);

            if (saveChanges)
                _repository.SaveChanges();
        }

        private void InsertUpdateEmbPrePaymentEmbDet(EmbPrePaymentEmbDet embPrePayEmbDet, string paymentStatus, bool saveChanges)
        {
            if (embPrePayEmbDet.Id == Guid.Empty)
            {
                embPrePayEmbDet.Id = Guid.NewGuid();
                _repository.Add(embPrePayEmbDet, false);
                if (embPrePayEmbDet.AppDocuments != null && embPrePayEmbDet.AppDocuments.Count > 0)
                {
                    foreach (var appDocument in embPrePayEmbDet.AppDocuments)
                    {
                        appDocument.TransactionId = embPrePayEmbDet.Id;
                    }
                    var appDocumentBC = new AppDocumentBC(_logger, _repository);
                    var AppDocument = appDocumentBC.saveAppDocument(embPrePayEmbDet.AppDocuments, false);
                }

                foreach (var det in embPrePayEmbDet.EmbPrePaymentInvDet)
                {
                    det.EmbPrePaymentEmbDetId = embPrePayEmbDet.Id;
                    InsertUpadtePrePyamentInvDet(det, paymentStatus, false);
                }

            }
            else
            {
                _repository.Update(embPrePayEmbDet, false);
                if (embPrePayEmbDet.AppDocuments != null && embPrePayEmbDet.AppDocuments.Count > 0)
                {
                    foreach (var appDocument in embPrePayEmbDet.AppDocuments)
                    {
                        appDocument.TransactionId = embPrePayEmbDet.Id;
                    }
                    var appDocumentBC = new AppDocumentBC(_logger, _repository);
                    var AppDocument = appDocumentBC.saveAppDocument(embPrePayEmbDet.AppDocuments, false);
                }

                foreach (var det in embPrePayEmbDet.EmbPrePaymentInvDet)
                {
                    det.EmbPrePaymentEmbDetId = embPrePayEmbDet.Id;
                    InsertUpadtePrePyamentInvDet(det, paymentStatus, false);
                }
            }
            InsertAuditForEmbPrePaymentEmbDet(embPrePayEmbDet);
            if (saveChanges)
                _repository.SaveChanges();
        }
        private void InsertUpadtePrePyamentInvDet(EmbPrePaymentInvDet detail, string paymentStatus, bool saveChanges)
        {
            detail.InvDate = detail.InvDate.ToLocalTime();
            if (detail.Id == Guid.Empty)
            {
                detail.Id = Guid.NewGuid();
                if (detail.AppDocuments != null && detail.AppDocuments.Count > 0)
                {
                    foreach (var appDocument in detail.AppDocuments)
                    {
                        appDocument.TransactionId = detail.Id;
                    }
                    var appDocumentBC = new AppDocumentBC(_logger, _repository);
                    var AppDocument = appDocumentBC.saveAppDocument(detail.AppDocuments, false);
                }
                _repository.Add(detail);
            }
            else
            {
                if (detail.AppDocuments != null && detail.AppDocuments.Count > 0)
                {
                    foreach (var appDocument in detail.AppDocuments)
                    {
                        appDocument.TransactionId = detail.Id;
                    }
                    var appDocumentBC = new AppDocumentBC(_logger, _repository);
                    var AppDocument = appDocumentBC.saveAppDocument(detail.AppDocuments, false);
                }
                _repository.Update(detail);
            }

            InsertAuditForEmbPrePaymentInvDet(detail);
            if (saveChanges)
                _repository.SaveChanges();
        }

        public AppResponse ApproveRejectEmbPrePayment(EmbPrePaymentHdr hdr)
        {

            if (hdr.Status == "APPROVED")
            {
                foreach (var emb in hdr.EmbPrePaymentEmbDet)
                {
                    foreach (var detail in emb.EmbPrePaymentInvDet)
                    {
                        var ledgerBal = _repository.GetQuery<LedgerBalanceDraft>().FirstOrDefault(a => a.TransactionId == detail.Id
                                    && a.TransactionType == ERPTransaction.EMB_PRE_PAYMENT && a.Active == "Y");
                        if (ledgerBal != null)
                        {
                            ledgerBal.Active = "N";
                            _repository.Update(ledgerBal, false);
                        }
                        if (detail.Active == "Y")
                        {
                            var ledgerBalance = new LedgerBalanceDraft()
                            {
                                FinYear = detail.FinYear,
                                Amount = detail.Amount,
                                //LedgerCode = detail.led,
                                LedgerDate = detail.InvDate,
                                Active = "Y",
                                TransactionId = detail.Id,
                                OrgId = detail.OrgId,
                                TransactionType = ERPTransaction.EMB_PRE_PAYMENT,
                                Id = Guid.NewGuid(),
                            };
                            _repository.Add(ledgerBalance, false);
                        }
                    }
                }
            }

            hdr.EmbPrePaymentEmbDet = null;
            InsertUpdateEmbPrePayment(hdr, true);

            var appResponse = new AppResponse();
            appResponse.Status = APPMessageKey.DATASAVESUCSS;

            return appResponse;
        }
        private void InsertAuditForEmbPrePaymentHdr(EmbPrePaymentHdr hdrInfo)
        {
            var prevValue = _repository.GetById<EmbPrePaymentHdr>(hdrInfo.Id);
            List<AppAudit> changedValues = new List<AppAudit>();
            if (prevValue != null)
                changedValues = AuditUtility.GetAuditableObject<EmbPrePaymentHdr>(prevValue, hdrInfo);
            else
            {
                changedValues = new List<AppAudit>();
                changedValues.Add(new AppAudit() { FieldName = "APP_RECORD_CREATED", NewValue = "", OldValue = "" });
                if (!string.IsNullOrWhiteSpace(hdrInfo.Remarks))
                    InsertEmbPrePaymentHdrComments(hdrInfo, hdrInfo.Remarks);

            }
            foreach (var change in changedValues)
            {
                var empreHist = new EmbPrePaymentHdrHist()
                {
                    Active = "Y",
                    CurrentValue = change.NewValue,
                    PrevValue = change.OldValue,
                    Id = Guid.NewGuid(),
                    FieldName = change.FieldName,
                    EmbPrePaymentHdrId = hdrInfo.Id
                };
                _repository.Add(empreHist);
                if (change.FieldName == "EMB_PAYMENT_REMARKS" || change.FieldName == "APPROVER_REMARKS")
                    InsertEmbPrePaymentHdrComments(hdrInfo, change.NewValue);
            }
        }

        private void InsertEmbPrePaymentHdrStatusHis(EmbPrePaymentHdr hdrInfo)
        {
            var empreHist = new EmbPrePaymentHdrStatusHist()
            {
                Active = "Y",
                Status = hdrInfo.Status,
                Comments = hdrInfo.ApproverRemarks,
                EmbPrePaymentHdrId = hdrInfo.Id,
                Id = Guid.NewGuid()
            };
            _repository.Add(empreHist);
        }

        private void InsertEmbPrePaymentHdrComments(EmbPrePaymentHdr hdrInfo, string updatedRemarks)
        {
            if (!string.IsNullOrEmpty(updatedRemarks))
            {
                var empreHist = new EmbPrePaymentHdrComment()
                {
                    Active = "Y",
                    Comments = updatedRemarks,
                    EmbPrePaymentHdrId = hdrInfo.Id,
                    Id = Guid.NewGuid()
                };
                _repository.Add(empreHist);
            }
        }

        private void InsertAuditForEmbPrePaymentEmbDet(EmbPrePaymentEmbDet embDet)
        {
            var prevValue = _repository.GetById<EmbPrePaymentEmbDet>(embDet.Id);
            List<AppAudit> changedValues = new List<AppAudit>();
            if (prevValue != null)
                changedValues = AuditUtility.GetAuditableObject<EmbPrePaymentEmbDet>(prevValue, embDet);
            else
            {
                changedValues = new List<AppAudit>();
                changedValues.Add(new AppAudit() { FieldName = "APP_RECORD_CREATED", NewValue = "", OldValue = "" });
                if (!string.IsNullOrWhiteSpace(embDet.Remarks))
                    InsertEmbPrePaymentEmDetComments(embDet, embDet.Remarks);
            }
            foreach (var change in changedValues)
            {
                var empreHist = new EmbPrePaymentEmbDetHist()
                {
                    Active = "Y",
                    CurrentValue = change.NewValue,
                    PrevValue = change.OldValue,
                    Id = Guid.NewGuid(),
                    FieldName = change.FieldName,
                    EmbPrePaymentEmbDetId = embDet.Id
                };
                _repository.Add(empreHist);
                if (change.FieldName == "EMB_PAYMENT_REMARKS" || change.FieldName == "APPROVER_REMARKS")
                    InsertEmbPrePaymentEmDetComments(embDet, change.NewValue);
            }
        }

        private void InsertEmbPrePaymentEmDetComments(EmbPrePaymentEmbDet embDet, string updatedRemarks)
        {
            if (!string.IsNullOrEmpty(updatedRemarks))
            {
                var embDetComment = new EmbPrePaymentEmbDetComment()
                {
                    Active = "Y",
                    Comments = updatedRemarks,
                    EmbPrePaymentEmbDetId = embDet.Id,
                    Id = Guid.NewGuid()
                };
                _repository.Add(embDetComment);
            }
        }
        private void InsertAuditForEmbPrePaymentInvDet(EmbPrePaymentInvDet invDet)
        {
            var prevValue = _repository.GetById<EmbPrePaymentInvDet>(invDet.Id);
            List<AppAudit> changedValues = new List<AppAudit>();
            if (prevValue != null)
                changedValues = AuditUtility.GetAuditableObject<EmbPrePaymentInvDet>(prevValue, invDet);
            else
            {
                changedValues = new List<AppAudit>();
                changedValues.Add(new AppAudit() { FieldName = "APP_RECORD_CREATED", NewValue = "", OldValue = "" });
                if (!string.IsNullOrWhiteSpace(invDet.Remarks))
                    InsertEmbPrePaymentInvDetComments(invDet, invDet.Remarks);
            }
            foreach (var change in changedValues)
            {
                var empreHist = new EmbPrePaymentInvDetHist()
                {
                    Active = "Y",
                    CurrentValue = change.NewValue,
                    PrevValue = change.OldValue,
                    Id = Guid.NewGuid(),
                    FieldName = change.FieldName,
                    EmbPrePaymentInvDetId = invDet.Id
                };
                _repository.Add(empreHist);
                if (change.FieldName == "EMB_PAYMENT_REMARKS" || change.FieldName == "APPROVER_REMARKS")
                    InsertEmbPrePaymentInvDetComments(invDet, change.NewValue);
            }
        }

        private void InsertEmbPrePaymentInvDetComments(EmbPrePaymentInvDet invDet, string updatedRemarks)
        {
            if (!string.IsNullOrEmpty(updatedRemarks))
            {
                var invDetComment = new EmbPrePaymentInvDetComment()
                {
                    Active = "Y",
                    Comments = updatedRemarks,
                    EmbPrePaymentInvDetId = invDet.Id,
                    Id = Guid.NewGuid()
                };
                _repository.Add(invDetComment);
            }
        }
    }

}