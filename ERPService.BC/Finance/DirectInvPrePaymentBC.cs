using ERPService.Common;
using ERPService.Common.Interfaces;
using ERPService.Common.Shared;
using ERPService.Data;
using ERPService.DataModel.CTO;
using ERPService.DataModel.DTO;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Caching;

namespace ERPService.BC
{
    public class DirectInvPrePaymentBC
    {
        private ILogger _logger;
        private IRepository _repository;
        private UserContext _userContext;

        public DirectInvPrePaymentBC(ILogger logger, IRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public DirectInvPrePayment GetDirectInvPrePaymentById(Guid invoiceId)
        {
            var allocheader = _repository.Get<DirectInvPrePayment>(a => a.Id == invoiceId);
            return allocheader;

        }

        public List<DirectInvPrePayment> GetDirectInvPrePaymentList(DirectInvSearch search, bool isExport = false)
        {


            search.FromInvDate = search.FromInvDate.ToLocalTime().Date;
            search.ToInvDate = search.ToInvDate.ToLocalTime().Date;

            List<DirectInvPrePayment> directInvoice = null;

            var vendorBC = new VendorMasterBC(_logger, _repository, _userContext);
            var vendorList = vendorBC.GetVendorMasterList();

            directInvoice = _repository.GetQuery<DirectInvPrePayment>().Where(a =>
                (string.IsNullOrEmpty(search.FinYear) || a.FinYear.Contains(search.FinYear))
              && (search.OrgId == Guid.Empty || search.OrgId == a.OrgId)
                && (search.DocumentNo == 0 || a.DocumentNo == search.DocumentNo)
              && (search.FromDocDate <= DateTime.MinValue || a.DocumentDate >= search.FromDocDate || search.FromDocDate == null)
              && (search.ToDocDate <= DateTime.MinValue || a.DocumentDate <= search.ToDocDate || search.ToDocDate == null)
              && a.Active == "Y").OrderByDescending(a=>a.CreatedDate).ToList();

            foreach (var item in directInvoice)
            {
                var vendor = vendorList.FirstOrDefault(a => a.Id == item.VendorMasterId);
                if (vendor != null)
                    item.VendorName = (string.IsNullOrEmpty(vendor.Title) ? "" : vendor.Title + " ") + vendor.Name;
            }
            if (search.LedgerCode.Count > 0 && search.LedgerCode != null)
            {
                var filterLedger = directInvoice.Where(a => search.LedgerCode.Contains(a.LedgerCode)).ToList();
                directInvoice = filterLedger;
            }
            if (search.Status.Count > 0 && search.Status != null)
            {
                var filterStatus = directInvoice.Where(a => search.Status.Contains(a.Status)).ToList();
                directInvoice = filterStatus;
            }
            if (search.CostCenterCode.Count > 0 && search.CostCenterCode != null)
            {
                var filterCostCenterCode = directInvoice.Where(a => search.CostCenterCode.Contains(a.CostCenterCode)).ToList();
                directInvoice = filterCostCenterCode;
            }
            if (search.VendorMasterId.Count > 0 && search.VendorMasterId != null)
            {
                var filterVendor = directInvoice.Where(a => search.VendorMasterId.Contains(a.VendorMasterId)).ToList();
                directInvoice = filterVendor;
            }
            return directInvoice;

        }

        public List<DirectInvPrePaymentDue> GetPrePaymentDues(DirectInvPrePaydueSearch search)
        {
            var prePayments = _repository.GetQuery<DirectInvPrePayment>();

            var queryBal = (from hdr in prePayments
                            where
                            hdr.Active == "Y"
                            && (search.FinYear == hdr.FinYear)
                            && (search.VendorId == Guid.Empty || search.VendorId == hdr.VendorMasterId)
                            && hdr.DueAmount > 0
                            && hdr.Status == "APPROVED"
                            select new DirectInvPrePaymentDue
                            {
                                DueAmount = hdr.DueAmount,
                                VendorId = hdr.VendorMasterId,
                                FinYear = hdr.FinYear,
                                OrgId = hdr.OrgId,
                                DirInvPrePaymentId = hdr.Id,
                                InvoiceDate = hdr.InvoiceDate,
                                InvoiceNo = hdr.InvoiceNo,
                                LedgerCode = hdr.LedgerCode,
                            }).ToList();

            return queryBal;
        }
        public AppResponse SaveDirectInvPrePayment(DirectInvPrePayment invoice, bool SaveChanges)
        {
            AppResponse appResponse = new AppResponse();
            List<String> validationMessages = new List<string>();

            bool validation = true;
            //Set Local time
            invoice.InvoiceDate = invoice.InvoiceDate.ToLocalTime();
            invoice.DocumentDate = invoice.DocumentDate.ToLocalTime();

            if (invoice.OrgId == Guid.Empty
               || invoice.VendorMasterId == Guid.Empty
               || invoice.InvoiceNo == string.Empty
               || invoice.CostCenterCode == string.Empty
               || invoice.Amount <= 0
               || invoice.LedgerCode == 0
               || invoice.InvoiceDate <= DateTime.MinValue
               || invoice.DocumentDate <= DateTime.MinValue)
            {
                appResponse.Status = APPMessageKey.MANDMISSING;
                appResponse.Messages = new List<string>();
                appResponse.Messages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.MANDMISSING]);
                return appResponse;
            }

            var existInvoice = _repository.GetQuery<DirectInvPrePayment>().Where(a => a.Id != invoice.Id &&
                a.InvoiceNo == invoice.InvoiceNo && a.VendorMasterId == invoice.VendorMasterId).FirstOrDefault();
            if (existInvoice != null && !string.IsNullOrEmpty(existInvoice.InvoiceNo))
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.DUPINVNOVENDOR]);
                validation = false;
            }

            if (invoice.Id != Guid.Empty && invoice.Active != "N")
            {
                existInvoice = _repository.Get<DirectInvPrePayment>(a => a.Id == invoice.Id && a.Active == "Y" && a.DueAmount == a.Amount);
                if (existInvoice == null)
                {
                    validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.EDITNOTPOSTPAYINV]);
                    validation = false;
                }
            }
            invoice.DueAmount = invoice.Amount;

            if (validation)
            {
                appResponse.ReferenceId = InsertUpdateDirectInvPrePayment(invoice, SaveChanges);
                appResponse.Status = APPMessageKey.DATASAVESUCSS;
            }
            else
            {
                appResponse.Status = APPMessageKey.ONEORMOREERR;
                appResponse.Messages = validationMessages;
            }
            return appResponse;
        }

        private Guid InsertUpdateDirectInvPrePayment(DirectInvPrePayment invoice, bool SaveChanges)
        {

            List<AppAudit> HdrHistory = new List<AppAudit>();
            DirInvPrePayHist dirInvPrePayHist = new DirInvPrePayHist();

            if (invoice.Id != Guid.Empty)
            {
                var Oldhdr = _repository.GetById<DirectInvPrePayment>(invoice.Id);
                HdrHistory = AuditUtility.GetAuditableObject<DirectInvPrePayment>(Oldhdr, invoice);

            }
            else
                HdrHistory.Add(new AppAudit() { FieldName = "APP_RECORD_CREATED", NewValue = "", OldValue = "" });

            if (invoice.Id == Guid.Empty)
            {
                invoice.Id = Guid.NewGuid();
                invoice.Active = "Y";
                if (invoice.AppDocuments != null && invoice.AppDocuments.Count > 0)
                {
                    foreach (var appDocument in invoice.AppDocuments)
                    {
                        appDocument.TransactionId = invoice.Id;
                    }
                    var appDocumentBC = new AppDocumentBC(_logger, _repository);
                    var AppDocument = appDocumentBC.saveAppDocument(invoice.AppDocuments, false);
                }
                _repository.Add(invoice, false);
            }
            else
            {
                if (invoice.AppDocuments != null && invoice.AppDocuments.Count > 0)
                {
                    foreach (var appDocument in invoice.AppDocuments)
                    {
                        appDocument.TransactionId = invoice.Id;
                    }
                    var appDocumentBC = new AppDocumentBC(_logger, _repository);
                    var AppDocument = appDocumentBC.saveAppDocument(invoice.AppDocuments, false);
                }
                _repository.Update(invoice, false);
                if (invoice.Status == "APPROVED")
                {
                    var ledgerBal = _repository.GetQuery<LedgerBalanceDraft>().FirstOrDefault(a => a.TransactionId == invoice.Id
                         && a.TransactionType == ERPTransaction.DIRECT_INVOICE_PRE_PAYMENT && a.Active == "Y");
                    if (ledgerBal != null)
                    {
                        ledgerBal.Active = "N";
                        _repository.Update(ledgerBal, false);
                    }
                }
            }
            if (invoice.Active == "Y" && invoice.Status == "APPROVED")
            {
                var ledgerBalance = new LedgerBalanceDraft()
                {
                    FinYear = invoice.FinYear,
                    Amount = invoice.Amount,
                    LedgerCode = invoice.LedgerCode,
                    LedgerDate = invoice.InvoiceDate,
                    Active = "Y",
                    TransactionId = invoice.Id,
                    OrgId = invoice.OrgId,
                    TransactionType = ERPTransaction.DIRECT_INVOICE_PRE_PAYMENT,
                    Id = Guid.NewGuid(),
                };
                _repository.Add(ledgerBalance, false);
            }

            var statushist = new DirInvPrePayStatusHist();
            statushist.Id = invoice.Id;
            statushist.Status = invoice.Status;
            statushist.Comments = invoice.ApproverRemarks;
            statushist.Active = invoice.Active;
            var dirInvPrePayHistoryCommentsBC = new DirInvPrePayHistoryCommentsBC(_logger, _repository);
            dirInvPrePayHistoryCommentsBC.SaveDirInvPrePayStatusHist(statushist, false);
            if (HdrHistory != null)
            {
                HdrHistory.ForEach(Hdr =>
                {
                    dirInvPrePayHist = new DirInvPrePayHist()
                    {
                        DirectInvPrePaymentId = invoice.Id,
                        FieldName = Hdr.FieldName,
                        PrevValue = Hdr.OldValue,
                        CurrentValue = Hdr.NewValue,
                        Active = "Y"

                    };
                    dirInvPrePayHistoryCommentsBC.SaveDirInvPrePayHist(dirInvPrePayHist, false);
                });

            }
            if (SaveChanges)
                _repository.SaveChanges();

            return invoice.Id;
        }
    }
}