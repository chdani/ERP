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
    public class DirectInvPostPaymentBC
    {
        private ILogger _logger;
        private IRepository _repository;
        private UserContext _userContext;

        public DirectInvPostPaymentBC(ILogger logger, IRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public DirectInvPostPayment GetDirectInvPostPaymentById(Guid invoiceId)
        {
            var allocheader = _repository.Get<DirectInvPostPayment>(a => a.Id == invoiceId);
            return allocheader;

        }

        public List<DirectInvPostPayment> GetDirectInvPostPaymentList(DirectInvSearch search, bool isExport = false)
        {

            search.FromInvDate = search.FromInvDate.ToLocalTime().Date;
            search.ToInvDate = search.ToInvDate.ToLocalTime().Date;

            List<DirectInvPostPayment> directInvoice = null;

            var vendorBc = new VendorMasterBC(_logger, _repository, _userContext);
            var vendorList = vendorBc.GetVendorMasterList();

            directInvoice = _repository.GetQuery<DirectInvPostPayment>().Where(a =>
                (string.IsNullOrEmpty(search.FinYear) || a.FinYear.Contains(search.FinYear))
              && (search.OrgId == Guid.Empty || search.OrgId == a.OrgId)
              && (string.IsNullOrEmpty(search.InvoiceNo) || a.InvoiceNo.Contains(search.InvoiceNo))
          && (search.FromInvDate <= DateTime.MinValue || a.InvoiceDate >= search.FromInvDate || search.FromInvDate == null)
              && (search.ToInvDate <= DateTime.MinValue || a.InvoiceDate <= search.ToInvDate || search.ToInvDate == null)
              && a.Active == "Y").OrderByDescending(a=>a.CreatedDate).ToList();

            foreach (var item in directInvoice)
            {
                var vendor = vendorList.Where(a => a.Id == item.VendorMasterId).FirstOrDefault();
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

        public AppResponse SaveDirectInvPostPayment(DirectInvPostPayment invoice)
        {
            AppResponse appResponse = new AppResponse();
            List<String> validationMessages = new List<string>();

            bool validation = true;
            //Set Local time
            invoice.InvoiceDate = invoice.InvoiceDate.ToLocalTime();
            invoice.DocumentDate = invoice.DocumentDate.ToLocalTime();

            if (invoice.OrgId == Guid.Empty
               || invoice.VendorMasterId == Guid.Empty
               || invoice.DirInvPrePaymentId == Guid.Empty
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

            DirectInvPostPayment existInvoice = null;
            decimal preAmount = 0;

            if (invoice.Id != Guid.Empty && invoice.Active != "N")
            {
                existInvoice = _repository.GetQuery<DirectInvPostPayment>().Where(a => a.Id != invoice.Id && a.InvoiceNo == invoice.InvoiceNo && a.VendorMasterId == invoice.VendorMasterId).FirstOrDefault();

                if (existInvoice != null && !string.IsNullOrEmpty(existInvoice.InvoiceNo) && invoice.Status == "SUBMITTED")
                {
                    validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.DUPINVNOVENDOR]);
                    validation = false;
                }

                existInvoice = _repository.GetById<DirectInvPostPayment>(invoice.Id);
                if (existInvoice != null)
                    preAmount = existInvoice.Amount;
            }


            var prePayment = _repository.Get<DirectInvPrePayment>(a => a.Id == invoice.DirInvPrePaymentId && a.Active == "Y");
            if (prePayment == null || (invoice.Active == "Y" && (prePayment.DueAmount + preAmount) < invoice.Amount))
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.PREPAYNOSUFFBAL]);
                validation = false;

            }

            if (validation)
            {
                appResponse.ReferenceId = InsertUpdateDirectInvPostPayment(invoice, prePayment, preAmount);
                appResponse.Status = APPMessageKey.DATASAVESUCSS;
            }
            else
            {
                appResponse.Status = APPMessageKey.ONEORMOREERR;
                appResponse.Messages = validationMessages;
            }
            return appResponse;
        }

        private Guid InsertUpdateDirectInvPostPayment(DirectInvPostPayment invoice, DirectInvPrePayment prePayment, decimal preAmount)
        {

            List<AppAudit> HdrHistory = new List<AppAudit>();
            DirInvPostPayHist dirInvPostPayHist = new DirInvPostPayHist();

            if (invoice.Id == Guid.Empty)
            {
                HdrHistory.Add(new AppAudit() { FieldName = "APP_RECORD_CREATED", NewValue = "", OldValue = "" });
                invoice.Id = Guid.NewGuid();
                invoice.Active = "Y";
                prePayment.DueAmount -= invoice.Amount;
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

                var Oldhdr = _repository.GetById<DirectInvPostPayment>(invoice.Id);
                HdrHistory = AuditUtility.GetAuditableObject<DirectInvPostPayment>(Oldhdr, invoice);

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

                if (invoice.Active == "Y")
                    prePayment.DueAmount = (prePayment.DueAmount + preAmount) - invoice.Amount;
                else
                    prePayment.DueAmount += preAmount;

                if (invoice.Status == "APPROVED")
                {
                    var ledgerBal = _repository.GetQuery<LedgerBalance>().FirstOrDefault(a => a.TransactionId == invoice.Id
                    && a.TransactionType == ERPTransaction.DIRECT_INVOICE_POST_PAYMENT && a.Active == "Y");
                    if (ledgerBal != null)
                    {
                        ledgerBal.Active = "N";
                        _repository.Update(ledgerBal, false);
                    }
                }
            }
            _repository.Update(prePayment);

            if (invoice.Status == "APPROVED")
            {
                var ledgerDraft = _repository.Get<LedgerBalanceDraft>(a => a.TransactionId == prePayment.Id
                        && a.TransactionType == ERPTransaction.DIRECT_INVOICE_POST_PAYMENT && a.Active == "Y");
                if (ledgerDraft != null)
                {
                    ledgerDraft.Amount = prePayment.DueAmount;
                    _repository.Update(ledgerDraft);
                }
                if (invoice.Active == "Y")
                {
                    var ledgerBalance = new LedgerBalance()
                    {
                        FinYear = invoice.FinYear,
                        Debit = invoice.Amount,
                        Credit = 0,
                        LedgerCode = invoice.LedgerCode,
                        LedgerDate = invoice.InvoiceDate,
                        Active = "Y",
                        TransactionId = invoice.Id,
                        OrgId = invoice.OrgId,
                        TransactionType = ERPTransaction.DIRECT_INVOICE_POST_PAYMENT,
                        Id = Guid.NewGuid(),
                        IsCommitted = true
                    };
                    _repository.Add(ledgerBalance, false);
                }
            }

            var statusHistroy = new DirInvPostPayStatusHist();
            statusHistroy.DirectInvPostPaymentId = invoice.Id;
            statusHistroy.Status = invoice.Status;
            statusHistroy.Active = invoice.Active;
            statusHistroy.Comments = invoice.ApproverRemarks;
            var dirInvPostPayHistCommentsBC = new DirInvPostPayHistCommentsBC(_logger, _repository);
            dirInvPostPayHistCommentsBC.saveDirInvPostPayStatusHist(statusHistroy, false);
            if (HdrHistory != null)
            {
                HdrHistory.ForEach(Hdr =>
                {
                    dirInvPostPayHist = new DirInvPostPayHist()
                    {
                        DirectInvPostPaymentId = invoice.Id,
                        FieldName = Hdr.FieldName,
                        PrevValue = Hdr.OldValue,
                        CurrentValue = Hdr.NewValue,
                        Active = "Y"

                    };
                    dirInvPostPayHistCommentsBC.saveDirInvPostPayHistory(dirInvPostPayHist, false);
                });

            }
            _repository.SaveChanges();

            return invoice.Id;
        }
    }
}