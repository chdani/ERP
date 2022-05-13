using ERPService.Common;
using ERPService.Common.Interfaces;
using ERPService.Common.Shared;
using ERPService.DataModel.DTO;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;


namespace ERPService.BC
{
    public class BudgetAllocationBC
    {
        private ILogger _logger;
        private IRepository _repository;
        public BudgetAllocationBC(ILogger logger, IRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }
        public List<BudgAllocHdr> getBudgetAllocationSearch(BudgAllocHdr search, bool isExport = false)
        {


            search.FromDate = search.FromDate.ToLocalTime().Date;
            search.ToDate = search.ToDate.ToLocalTime().Date;

            var budgetAllocation = _repository.GetQuery<BudgAllocHdr>().Where(a =>
                  a.FinYear == search.FinYear
                  && (search.OrgId == Guid.Empty || search.OrgId == a.OrgId)
                  && ((search.BudgetAmount == 0) || (search.BudgetAmount == a.BudgetAmount))
                  && ((search.FromDate <= DateTime.MinValue || search.ToDate <= DateTime.MinValue)
                       || (a.BudgetDate >= search.FromDate && a.BudgetDate <= search.ToDate)) &&

                   a.Active == "Y").OrderByDescending(a => a.TransNo).ToList();

            var buddet = _repository.GetQuery<BudgAllocDet>();
            budgetAllocation.ForEach(Hd =>
            {
                var BudgAllocDet = new BudgAllocDet();
                var det = buddet.Where(a => a.BudgAllocHdrId == Hd.Id).ToList();
                if (det != null)
                    Hd.BudgAllocDet = det;
            });
            if (search.SelectBudgetType.Count > 0 && search.SelectBudgetType != null)
            {
                var filterType = budgetAllocation.Where(a => search.SelectBudgetType.Contains(a.BudgetType)).ToList();
                budgetAllocation = filterType;
            }
            if (search.SelectedStatus.Count > 0 && search.SelectedStatus != null)
            {
                var filterStatus = budgetAllocation.Where(a => search.SelectedStatus.Contains(a.Status)).ToList();
                budgetAllocation = filterStatus;
            }
            if (isExport)
            {
                foreach (var bud in budgetAllocation)
                {
                    var budgettype = _repository.GetQuery<CodesDetails>().FirstOrDefault(a => a.Code == bud.BudgetType);
                    bud.BudgetType = budgettype.Description;
                    foreach (var dd in bud.BudgAllocDet)
                    {
                        var LedgerCode = _repository.GetQuery<LedgerAccount>().FirstOrDefault(a => a.LedgerCode == dd.LedgerCode);
                        var ToLedgerCode = _repository.GetQuery<LedgerAccount>().FirstOrDefault(a => a.LedgerCode == dd.ToLedgerCode);
                        dd.LedgerDesc = dd.LedgerCode.ToString() + " - " + LedgerCode.LedgerDesc;
                        if (ToLedgerCode != null)
                            dd.ToLedgerDesc = dd.ToLedgerCode.ToString() + " - " + ToLedgerCode.LedgerDesc;
                        else
                            dd.ToLedgerDesc = "";

                        if (dd.Remarks != null)
                            dd.Remarks = dd.Remarks;
                        else
                            dd.Remarks = "";
                    }
                }
            }
            return budgetAllocation;
        }
        public AppResponse SaveBudgetAllocation(BudgAllocHdr budegetAllochdr)
        {
            AppResponse appResponse = new AppResponse();
            List<AppAudit> HdrHistory = new List<AppAudit>();
            BudgAllocHdrHist budgAllocHdrHist = new BudgAllocHdrHist();
            List<String> validationMessages = new List<string>();

            bool validation = true;
            if (budegetAllochdr.BudgetType != "BUDG_SUPPLMNT")
            {
                if (budegetAllochdr.BudgetAmount <= 0)
                {
                    validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.MANDMISSING]);
                    validation = false;
                }
            }
            if (string.IsNullOrEmpty(budegetAllochdr.BudgetType) || budegetAllochdr.BudgetDate <= DateTime.MinValue ||
            string.IsNullOrEmpty(budegetAllochdr.FinYear))
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.MANDMISSING]);
                validation = false;
            }

            budegetAllochdr.BudgetDate = budegetAllochdr.BudgetDate.ToLocalTime();

            if (budegetAllochdr.Id != Guid.Empty)
            {
                var header = _repository.GetById<BudgAllocHdr>(budegetAllochdr.Id);
                if (header == null)
                {
                    validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.RECNOTFOUND]);
                    validation = false;
                }
            }
            if (budegetAllochdr.BudgAllocDet != null && budegetAllochdr.BudgAllocDet.Count > 0)
            {
                var childMessages = new List<string>();
                var result = validateBudgetAllocDet(budegetAllochdr.BudgAllocDet, budegetAllochdr.BudgetType, out childMessages);
                if (!result)
                {
                    validation = result;
                    validationMessages.AddRange(childMessages);
                }
            }

            if (validation)
            {
                InsertUpdateBudgetAllocation(budegetAllochdr);
                appResponse.Status = APPMessageKey.DATASAVESUCSS;

            }
            else
            {
                appResponse.Status = APPMessageKey.ONEORMOREERR;
                appResponse.Messages = validationMessages;
            }
            return appResponse;
        }

        private bool validateBudgetAllocDet(ICollection<BudgAllocDet> budgAllocDets, string budgetType, out List<string> validationMessages)
        {
            var validation = true;
            validationMessages = new List<string>();
            foreach (var det in budgAllocDets)
            {
                if (budgetType != "BUDG_SUPPLMNT")
                {
                    if ( det.BudgetAmount <= 0)
                    {
                        validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.MANDMISSING]);
                        validation = false;
                    }
                }
                if (det.LedgerCode == 0 )
                {
                    validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.MANDMISSING]);
                    validation = false;
                }
                var ledgerAcc = _repository.GetQuery<LedgerAccount>().Where(a => a.LedgerCode == det.LedgerCode && a.Active == "Y").ToList();
                if (ledgerAcc == null || ledgerAcc.Count() == 0)
                {
                    validationMessages.Add(string.Format(ERPExceptions.APP_MESSAGES[APPMessageKey.LEDCODENOTEXIT], det.LedgerCode));
                    validation = false;
                }

                if (budgetType == "BUDG_TRANS" && det.Active == "Y")
                {
                    var toLedger = _repository.GetQuery<LedgerAccount>().Where(a => a.LedgerCode == det.ToLedgerCode && a.Active == "Y").ToList();
                    if (toLedger == null || toLedger.Count() == 0)
                    {
                        validationMessages.Add(string.Format(ERPExceptions.APP_MESSAGES[APPMessageKey.LEDCODENOTEXIT], det.ToLedgerCode));
                        validation = false;
                    }
                }

                var budgetDet = _repository.GetQuery<BudgAllocDet>().Where(a => a.Id != det.Id && det.Active == "Y" &&
                        a.LedgerCode == det.LedgerCode && a.BudgAllocHdrId == det.BudgAllocHdrId).FirstOrDefault();
                if (budgetDet != null)
                {
                    validationMessages.Add(string.Format(ERPExceptions.APP_MESSAGES[APPMessageKey.DUPLIBUDLEDCODE], det.LedgerCode));
                    validation = false;
                }

                if (det.Action != 'N' || det.Id != Guid.Empty)
                {
                    budgetDet = _repository.GetById<BudgAllocDet>(det.Id);
                    if (budgetDet == null)
                    {
                        validationMessages.Add(string.Format(ERPExceptions.APP_MESSAGES[APPMessageKey.RECEXITBUDLEDCODE], det.LedgerCode));
                        validation = false;
                    }
                }
            }
            return validation;
        }

        private void InsertUpdateBudgetAllocation(BudgAllocHdr budgAllocHdr)
        {
            List<AppAudit> HdrDetailsHistory = new List<AppAudit>();
            BudgAllocDetHist budgAllocHdrHist = new BudgAllocDetHist();

            var changes = new List<AppAudit>();

            if (budgAllocHdr.Id == Guid.Empty)
            {
                changes.Add(new AppAudit() { FieldName = "APP_RECORD_CREATED", NewValue = "", OldValue = "" });

                budgAllocHdr.Id = Guid.NewGuid();
                if (budgAllocHdr.AppDocuments != null && budgAllocHdr.AppDocuments.Count > 0)
                {
                    foreach (var appDocument in budgAllocHdr.AppDocuments)
                    {
                        appDocument.TransactionId = budgAllocHdr.Id;
                    }
                    var appDocumentBC = new AppDocumentBC(_logger, _repository);
                    var AppDocument = appDocumentBC.saveAppDocument(budgAllocHdr.AppDocuments, false);
                }
                _repository.Add(budgAllocHdr, false);
            }
            else
            {

                var Oldhdr = _repository.GetById<BudgAllocHdr>(budgAllocHdr.Id);
                changes = AuditUtility.GetAuditableObject<BudgAllocHdr>(Oldhdr, budgAllocHdr);

                if (budgAllocHdr.AppDocuments != null && budgAllocHdr.AppDocuments.Count > 0)
                {
                    foreach (var appDocument in budgAllocHdr.AppDocuments)
                    {
                        appDocument.TransactionId = budgAllocHdr.Id;
                    }
                    var appDocumentBC = new AppDocumentBC(_logger, _repository);
                    var AppDocument = appDocumentBC.saveAppDocument(budgAllocHdr.AppDocuments, false);
                }
                _repository.Update(budgAllocHdr, false);
            }

            var budgetHistroyCommentsBC = new BudgetHistroyCommentsBC(_logger, _repository);
            var allocDet = budgAllocHdr.BudgAllocDet;
            if (allocDet != null)
            {
                foreach (var det in allocDet)
                {
                    if (det.Id != Guid.Empty)
                    {

                        var OldhdrDetails = _repository.GetById<BudgAllocDet>(det.Id);
                        HdrDetailsHistory = AuditUtility.GetAuditableObject<BudgAllocDet>(OldhdrDetails, det);

                    }
                    else
                        HdrDetailsHistory.Add(new AppAudit() { FieldName = "APP_RECORD_CREATED", NewValue = "", OldValue = "" });
                    det.BudgAllocHdrId = budgAllocHdr.Id;
                    if (HdrDetailsHistory != null)
                    {
                        HdrDetailsHistory.ForEach(Hdr =>
                        {
                            budgAllocHdrHist = new BudgAllocDetHist()
                            {
                                BudgAllocDetId = det.Id,
                                FieldName = Hdr.FieldName,
                                PrevValue = Hdr.OldValue,
                                CurrentValue = Hdr.NewValue,
                                Active = "Y"

                            };
                            var saveHdrDetailsHistory = budgetHistroyCommentsBC.SaveBudgAllocDetHist(budgAllocHdrHist);
                        });

                    }
                    if (det.Id == Guid.Empty)
                    {
                        det.Id = Guid.NewGuid();
                        if (det.AppDocuments != null && det.AppDocuments.Count > 0)
                        {
                            foreach (var appDocument in det.AppDocuments)
                            {
                                appDocument.TransactionId = det.Id;
                            }
                            var appDocumentBC = new AppDocumentBC(_logger, _repository);
                            var AppDocument = appDocumentBC.saveAppDocument(det.AppDocuments, false);
                        }

                        _repository.Add(det, false);
                    }
                    else
                    {
                        if (det.AppDocuments != null && det.AppDocuments.Count > 0)
                        {
                            foreach (var appDocument in det.AppDocuments)
                            {
                                appDocument.TransactionId = det.Id;
                            }
                            var appDocumentBC = new AppDocumentBC(_logger, _repository);
                            var AppDocument = appDocumentBC.saveAppDocument(det.AppDocuments, false);
                        }
                        _repository.Update(det, false);

                        if (budgAllocHdr.Status != "APPROVED")
                        {
                            var ledgerBal = _repository.GetQuery<LedgerBalance>().Where(a => a.TransactionId == det.Id
                                        && (a.TransactionType == ERPTransaction.BUDGET_TRANSFER || a.TransactionType == ERPTransaction.BUDGET_ALLOCATION)
                                        && a.Active == "Y");
                            foreach (var ledger in ledgerBal)
                            {
                                ledger.Active = "N";
                                _repository.Update(ledger, false);
                            }
                        }
                    }

                    if (det.Active == "Y" && budgAllocHdr.Status == "APPROVED")
                    {
                        if (budgAllocHdr.BudgetType == "BUDG_TRANS")
                        {
                            var ledgerBalance = new LedgerBalance()
                            {
                                FinYear = budgAllocHdr.FinYear,
                                Debit = det.BudgetAmount,
                                Credit = 0,
                                LedgerCode = det.LedgerCode,
                                LedgerDate = budgAllocHdr.BudgetDate,
                                Active = "Y",
                                TransactionId = det.Id,
                                OrgId = det.OrgId,
                                TransactionType = ERPTransaction.BUDGET_TRANSFER,
                                Remarks = "Transfer To " + det.ToLedgerCode,
                                Id = Guid.NewGuid(),
                                IsCommitted = true
                            };
                            _repository.Add(ledgerBalance, false);

                            var ledgerBalanceTo = new LedgerBalance()
                            {
                                FinYear = budgAllocHdr.FinYear,
                                Debit = 0,
                                Credit = det.BudgetAmount,
                                LedgerCode = det.ToLedgerCode.Value,
                                LedgerDate = budgAllocHdr.BudgetDate,
                                Active = "Y",
                                TransactionId = det.Id,
                                OrgId = det.OrgId,
                                TransactionType = ERPTransaction.BUDGET_TRANSFER,
                                Remarks = "Transfer from " + det.LedgerCode,
                                Id = Guid.NewGuid(),
                                IsCommitted = true
                            };
                            _repository.Add(ledgerBalanceTo, false);
                        }
                        else if (budgAllocHdr.BudgetType == "BUDG_RETURN")
                        {
                            var ledgerBalance = new LedgerBalance()
                            {
                                FinYear = budgAllocHdr.FinYear,
                                Debit = det.BudgetAmount,
                                Credit = 0,
                                LedgerCode = det.LedgerCode,
                                LedgerDate = budgAllocHdr.BudgetDate,
                                Active = "Y",
                                TransactionId = det.Id,
                                OrgId = det.OrgId,
                                TransactionType = ERPTransaction.BUDGET_ALLOCATION,
                                Id = Guid.NewGuid(),
                                IsCommitted = true
                            };
                            _repository.Add(ledgerBalance, false);
                        }
                        else if (budgAllocHdr.BudgetType == "BUDG_SUPPLMNT")
                        {
                            var ledgerBalance = new LedgerBalance();
                            ledgerBalance.FinYear = budgAllocHdr.FinYear;
                            ledgerBalance.LedgerCode = det.LedgerCode;
                            ledgerBalance.LedgerDate = budgAllocHdr.BudgetDate;
                            ledgerBalance.Active = "Y";
                            ledgerBalance.TransactionId = det.Id;
                            ledgerBalance.OrgId = det.OrgId;
                            ledgerBalance.TransactionType = ERPTransaction.BUDGET_ALLOCATION;
                            ledgerBalance.Id = Guid.NewGuid();
                            ledgerBalance.IsCommitted = true;
                            //Negative amount in adjustment to be Debited 
                            if (det.BudgetAmount < 0)
                            {
                                ledgerBalance.Debit = Math.Abs(det.BudgetAmount);//Sending positive amount to debit
                                ledgerBalance.Credit = 0;
                            }
                            else //Positive amount in adjustment to be Credited 
                            {
                                ledgerBalance.Debit = 0;
                                ledgerBalance.Credit = det.BudgetAmount;

                            }
                            _repository.Add(ledgerBalance, false);
                        }
                        else
                        {
                            var ledgerBalance = new LedgerBalance()
                            {
                                FinYear = budgAllocHdr.FinYear,
                                Debit = 0,
                                Credit = det.BudgetAmount,
                                LedgerCode = det.LedgerCode,
                                LedgerDate = budgAllocHdr.BudgetDate,
                                Active = "Y",
                                TransactionId = det.Id,
                                OrgId = det.OrgId,
                                TransactionType = ERPTransaction.BUDGET_ALLOCATION,
                                Id = Guid.NewGuid(),
                                IsCommitted = true
                            };
                            _repository.Add(ledgerBalance, false);
                        }
                    }
                }
            }

            var statushist = new BudgAllocHdrStatusHist();
            statushist.BudgAllocHdrId = budgAllocHdr.Id;
            statushist.Status = budgAllocHdr.Status;
            statushist.Comments = budgAllocHdr.ApproverRemarks;
            statushist.Active = budgAllocHdr.Status;
            budgetHistroyCommentsBC.SaveStatusHistory(statushist, false);

            if (changes != null)
            {
                var budgAllocHdrHistories = new List<BudgAllocHdrHist>();
                changes.ForEach(Hdr =>
                {
                    budgAllocHdrHistories.Add(new BudgAllocHdrHist()
                    {
                        BudgAllocHdrId = budgAllocHdr.Id,
                        FieldName = Hdr.FieldName,
                        PrevValue = Hdr.OldValue,
                        CurrentValue = Hdr.NewValue,
                        Active = "Y"

                    });
                });
                budgetHistroyCommentsBC.SaveBudgAllocHdrHist(budgAllocHdrHistories, false);
            }
            _repository.SaveChanges();
        }

        public BudgAllocHdr GetBudgetAlloctionById(Guid allocationHdrId)
        {
            List<BudgAllocDet> budgAllocDet = new List<BudgAllocDet>();
            var allocheader = _repository.Get<BudgAllocHdr>(a => a.Id == allocationHdrId);
            if (allocheader != null)
            {
                budgAllocDet = _repository.GetQuery<BudgAllocDet>().Where(a => a.BudgAllocHdrId == allocationHdrId && a.Active == "Y").ToList();
            }
            allocheader.BudgAllocDet = budgAllocDet;
            return allocheader;
        }
        public List<BudgAllocHdr> GetBudgetAlloctions(BudgAllocHdr search)
        {
            return _repository.GetQuery<BudgAllocHdr>().
                 Where(a => (string.IsNullOrEmpty(search.FinYear) || a.FinYear.Contains(search.FinYear))
                 && (search.OrgId == Guid.Empty || search.OrgId == a.OrgId)
                 && a.Active == "Y").ToList();
        }

        public List<BudgAllocDet> GetBudgetAllocDetByHdrId(Guid budgetHdrId)
        {
            var budgAllocDet = _repository.GetQuery<BudgAllocDet>().Where(a => a.BudgAllocHdrId == budgetHdrId && a.Active == "Y").Distinct().ToList();
            if (budgAllocDet != null && budgAllocDet.Count > 0)
            {
                var accounts = _repository.GetQuery<LedgerAccount>().ToList();
                foreach (var det in budgAllocDet)
                {
                    var ledgr = accounts.FirstOrDefault(a => a.LedgerCode == det.LedgerCode);
                    if (ledgr != null)
                        det.LedgerDesc = ledgr.LedgerCode + "- " + ledgr.LedgerDesc;
                    ledgr = accounts.FirstOrDefault(a => a.LedgerCode == det.ToLedgerCode);
                    if (ledgr != null)
                        det.ToLedgerDesc = ledgr.LedgerCode + "- " + ledgr.LedgerDesc;

                }
            }
            return budgAllocDet;
        }
    }

}