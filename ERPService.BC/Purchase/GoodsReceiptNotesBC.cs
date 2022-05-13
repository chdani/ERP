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
    public class GoodsReceiptNotesBC
    {
        private ILogger _logger;
        private IRepository _repository;
        private UserContext _userContext;
        private byte[] buff;

        public GoodsReceiptNotesBC(ILogger logger, IRepository repository, UserContext userContext)
        {
            _logger = logger;
            _repository = repository;
            _userContext = userContext;
        }

        public GoodsRecNote GetGoodsRecNoteById(Guid requestId)
        {
            var goodsRecNote = _repository.GetById<GoodsRecNote>(requestId);
            if (goodsRecNote != null)
                goodsRecNote.GoodsReceiptNoteDet = GetGoodsRecNoteDetByHdrId(requestId);
            return goodsRecNote;
        }

        public List<GoodsRecNoteDet> GetGoodsRecNoteDetByHdrId(Guid hdrId)
        {
            return _repository.GetQuery<GoodsRecNoteDet>().Where(a => a.Active == "Y" && a.GoodsReceiptNoteId == hdrId).ToList(); ;
        }
        public AppResponse SaveGoodsRecNote(GoodsRecNote goodsRecNote)
        {
            List<String> validationMessages = new List<string>();
            AppResponse appResponse = new AppResponse();
            bool validation = true;

            if (goodsRecNote.PurchaseOrderId == Guid.Empty || goodsRecNote.VendorMasterId == Guid.Empty
                || goodsRecNote.TransDate <= DateTime.MinValue
                || goodsRecNote.InvoiceDate <= DateTime.MinValue || string.IsNullOrEmpty(goodsRecNote.InvoiceNo))
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.MANDMISSING]);
                validation = false;
            }
            if (goodsRecNote.Active == "Y")
            {
                if (goodsRecNote.GoodsReceiptNoteDet == null || goodsRecNote.GoodsReceiptNoteDet.Count == 0)
                {
                    validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.NOTDETROWEXIST]);
                    validation = false;
                }
                else
                {

                    bool childValid = true;
                    var childMessages = ValidateChildRecords(goodsRecNote.GoodsReceiptNoteDet, out childValid);
                    if (!childValid)
                    {
                        validation = false;
                        validationMessages.AddRange(childMessages);
                    }
                }
            }
            if (goodsRecNote.Id == Guid.Empty)
            {
                var goodsReceiptnoteTransNoAndSeqNo = AppGeneralMethods.TranstypeSeqNumber("GoodsReceiptnoteTransType", _repository);
                goodsRecNote.TransNo = goodsReceiptnoteTransNoAndSeqNo.Item1;
                goodsRecNote.SeqNo = goodsReceiptnoteTransNoAndSeqNo.Item2;
            }

            if (goodsRecNote != null && validation)
            {

                goodsRecNote.TransDate = goodsRecNote.TransDate.ToLocalTime().Date;
                goodsRecNote.InvoiceDate = goodsRecNote.InvoiceDate.ToLocalTime().Date;
                goodsRecNote.Status = "PURTRNSTSSUBMITTED";
                InsertUpdateGoodsRecNote(goodsRecNote);
                appResponse.Status = APPMessageKey.DATASAVESUCSS;
            }
            else
            {
                appResponse.Status = APPMessageKey.ONEORMOREERR;
                appResponse.Messages = validationMessages;
            }
            return appResponse;
        }

        public AppResponse SaveGoodsRecNoteComment(GoodsRecNoteComment goodRecNotComment)
        {
            InsertGoodsRecNoteComments(goodRecNotComment, true);
            return new AppResponse() { Status = APPMessageKey.DATASAVESUCSS };
        }

        public AppResponse SaveGoodsRecNoteDetComment(GoodsRecNoteDetComment goodRecNotDetComment)
        {
            InsertGoodsRecNoteDetComments(goodRecNotDetComment, true);
            return new AppResponse() { Status = APPMessageKey.DATASAVESUCSS };
        }

        public AppResponse ApproveGoodsRecNote(GoodsRecNote goodsRecNote)
        {
            List<String> validationMessages = new List<string>();
            AppResponse appResponse = new AppResponse();
            bool validation = true;

            if (goodsRecNote.Id == Guid.Empty || string.IsNullOrEmpty(goodsRecNote.ApproverRemarks))
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.MANDMISSING]);
                validation = false;
            }
            var gooodsId = goodsRecNote.Id;
            var grnDetails = GetGoodsRecNoteDetByHdrId(goodsRecNote.Id);
            if (grnDetails == null || grnDetails.Count <= 0)
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.MANDMISSING]);
                validation = false;
            }

            if (validation)
            {
                var statusHistory = new GoodsRecNoteStatusHist()
                {
                    Active = "Y",
                    Comments = goodsRecNote.ApproverRemarks,
                    Status = goodsRecNote.Status,
                    GoodsReceiptNoteId = goodsRecNote.Id,
                };

                goodsRecNote.GoodsReceiptNoteDet = null;

                if (goodsRecNote.Status == "PURTRNSTSAPPROVED")
                {
                    var units = _repository.GetQuery<ProdUnitMaster>();
                    var prodInventories = new List<ProductInventory>();
                    var goodsRecNoteTransno = long.Parse(goodsRecNote.TransNo);
                    foreach (var det in grnDetails)
                    {
                        var unit = units.FirstOrDefault(a => a.Id == det.UnitMasterId);
                        decimal convertQty = 1;
                        if (unit != null)
                            convertQty = unit.ConversionUnit;

                        prodInventories.Add(new ProductInventory
                        {
                            Active = "Y",
                            TransDate = goodsRecNote.TransDate,
                            ProductMasterId = det.ProductMasterId,
                            StockIn = det.Quantity * convertQty,
                            TransId = det.Id,
                            ShelveNo = det.ShelveNo,
                            Barcode = det.Barcode,
                            ActorType = "VENDOR",
                            ActorId = goodsRecNote.VendorMasterId,
                            TransNo = goodsRecNoteTransno,
                            TransType = "GRNDET",
                            WareHouseLocationId = goodsRecNote.WareHouseLocationId
                        });
                    }
                    var invBC = new ProductInventoryBC(_logger, _repository);

                    var prodInv = new ProductInventoryHdr();
                    prodInv.StockType = "R";
                    prodInv.Inventories = prodInventories;

                    appResponse = invBC.SaveProductInventoryList(prodInv, false);
                    if (appResponse.Status == APPMessageKey.DATASAVESUCSS)
                    {
                        _repository.Update(goodsRecNote, false);
                        InsertUpdateGoodsRecNoteApproval(statusHistory, false);
                        _repository.SaveChanges();
                        appResponse.Status = APPMessageKey.DATASAVESUCSS;
                        appResponse.ReferenceId = gooodsId;
                    }
                }
                else
                {
                    _repository.Update(goodsRecNote, false);
                    InsertUpdateGoodsRecNoteApproval(statusHistory, false);
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

        public List<GoodsRecNoteHist> GetGoodsRecNoteHistory(Guid goodRecNoteId)
        {
            var usemaster = _repository.GetQuery<UserMaster>().ToList();
            var goodRecNoteHist = _repository.GetQuery<GoodsRecNoteHist>().Where(a => a.GoodsReceiptNoteId == goodRecNoteId).OrderByDescending(a => a.CreatedDate).ToList();
            goodRecNoteHist.ForEach(Hsty =>
            {
                var userName = usemaster.Where(a => a.Id == Hsty.CreatedBy).FirstOrDefault();
                if (userName != null)
                    Hsty.UserName = userName.FirstName + " " + userName.LastName;
            });
            return goodRecNoteHist.OrderByDescending(a => a.CreatedDate).ToList();
        }

        public List<GoodsRecNoteDetHist> GetGoodsRecNoteDetHistory(Guid goodRecNoteDetId)
        {
            var usemaster = _repository.GetQuery<UserMaster>().ToList();
            var goodRecNoteDetIdHist = _repository.GetQuery<GoodsRecNoteDetHist>().Where(a => a.GoodsReceiptNoteDetId == goodRecNoteDetId).OrderByDescending(a => a.CreatedDate).ToList();
            goodRecNoteDetIdHist.ForEach(Hsty =>
            {
                var userName = usemaster.Where(a => a.Id == Hsty.CreatedBy).FirstOrDefault();
                if (userName != null)
                    Hsty.UserName = userName.FirstName + " " + userName.LastName;
            });
            return goodRecNoteDetIdHist.OrderByDescending(a => a.CreatedDate).ToList();
        }

        public List<GoodsRecNoteComment> GetGoodsRecNoteComment(Guid goodRecNoteId)
        {
            var comments = _repository.GetQuery<GoodsRecNoteComment>().Where(a => a.GoodsReceiptNoteId == goodRecNoteId).ToList();
            var commentIds = comments.Select(a => a.Id).Distinct();

            var User = _repository.GetQuery<UserMaster>().ToList();

            var appDoucuments = _repository.GetQuery<AppDocument>().Where(a => a.Active == "Y"
                    && a.TransactionType == "GRNCOMM" && commentIds.Contains(a.TransactionId)).ToList();

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

        public List<GoodsRecNoteDetComment> GetGoodsRecNoteDetComment(Guid goodRecNoteId)
        {
            var comments = _repository.GetQuery<GoodsRecNoteDetComment>().Where(a => a.GoodsReceiptNoteDetId == goodRecNoteId).ToList();
            var commentIds = comments.Select(a => a.Id).Distinct();

            var User = _repository.GetQuery<UserMaster>().ToList();

            var appDoucuments = _repository.GetQuery<AppDocument>().Where(a => a.Active == "Y"
                    && a.TransactionType == "GRNDETCOMM" && commentIds.Contains(a.TransactionId)).ToList();

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

        public List<AppDocument> GetGoodsRecNotesAttachments(Guid goodRecNoteId)
        {
            var usemaster = _repository.GetQuery<UserMaster>().ToList();

            var comments = _repository.GetQuery<GoodsRecNoteComment>().Where(a => a.GoodsReceiptNoteId == goodRecNoteId && a.Active == "Y").ToList();

            var fileAttachmentIds = new List<Guid>();
            fileAttachmentIds.AddRange(comments.Select(a => a.Id).Distinct().ToList());
            fileAttachmentIds.Add(goodRecNoteId);

            var appDoucuments = _repository.GetQuery<AppDocument>().Where(a => a.Active == "Y"
                    && (a.TransactionType == "GRNCOMM" || a.TransactionType == "GRN") && fileAttachmentIds.Contains(a.TransactionId))
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

        public List<GoodsRecNoteStatusHist> GetGoodsRecNoteStatusHistory(Guid goodRecNoteId)
        {
            var usemaster = _repository.GetQuery<UserMaster>().Where(a => a.Active == "Y").ToList();
            var approvals = _repository.GetQuery<GoodsRecNoteStatusHist>().Where(a => a.GoodsReceiptNoteId == goodRecNoteId).OrderByDescending(a => a.CreatedDate).ToList();
            approvals.ForEach(status =>
            {
                var userName = usemaster.Where(a => a.Id == status.CreatedBy).FirstOrDefault();
                if (userName != null)
                    status.UserName = userName.FirstName + " " + userName.LastName;
            });

            return approvals.OrderByDescending(a => a.CreatedDate).ToList();
        }

        private List<string> ValidateChildRecords(List<GoodsRecNoteDet> goodRecNotetDet, out bool validation)
        {
            var validationMessages = new List<string>();
            validation = true;
            foreach (var det in goodRecNotetDet)
            {
                if (det.ProductMasterId == Guid.Empty
                    || det.Quantity <= 0
                    || det.UnitMasterId == Guid.Empty
                    || det.Price <= 0)
                {
                    validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.MANDMISSING]);
                    validation = false;
                }
                if (det.ExpiryDate < DateTime.Now)
                {
                    validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.PROVIDEFUTUREDATEGRN]);
                    validation = false;
                }

            }

            return validationMessages;
        }

        private void InsertUpdateGoodsRecNote(GoodsRecNote goodsRecNote)
        {
            var histories = new List<AppAudit>();

            if (goodsRecNote.Id == Guid.Empty)
            {
                histories.Add(new AppAudit() { FieldName = "APP_RECORD_CREATED", NewValue = "", OldValue = "" });
                goodsRecNote.Id = Guid.NewGuid();
                _repository.Add(goodsRecNote, false);
            }
            else
            {
                var OldValue = _repository.GetById<GoodsRecNote>(goodsRecNote.Id);
                histories = AuditUtility.GetAuditableObject(OldValue, goodsRecNote);
                _repository.Update(goodsRecNote, false);
            }


            if (histories != null)
            {
                histories.ForEach(Hdr =>
                {
                    var goodRecNoteHist = new GoodsRecNoteHist()
                    {
                        GoodsReceiptNoteId = goodsRecNote.Id,
                        FieldName = Hdr.FieldName,
                        PrevValue = Hdr.OldValue,
                        CurrentValue = Hdr.NewValue,
                        Active = "Y",
                        Id = Guid.NewGuid()

                    };
                    _repository.Add(goodRecNoteHist);
                });

            }

            if (goodsRecNote.GoodsReceiptNoteDet != null)
            {
                foreach (var det in goodsRecNote.GoodsReceiptNoteDet)
                {
                    det.GoodsReceiptNoteId = goodsRecNote.Id;
                    InsertUpdateGoodsRecNoteDet(det, false);
                }
            }

            var statusHistory = new GoodsRecNoteStatusHist()
            {
                Active = "Y",
                Comments = goodsRecNote.Remarks,
                Status = "PURTRNSTSSUBMITTED",
                GoodsReceiptNoteId = goodsRecNote.Id,
            };

            InsertUpdateGoodsRecNoteApproval(statusHistory, false);

            _repository.SaveChanges();
        }

        private void InsertUpdateGoodsRecNoteDet(GoodsRecNoteDet det, bool saveChanges)
        {

            var detailHistory = new List<AppAudit>();
            if (det.Id != Guid.Empty)
            {
                var OldDet = _repository.GetById<GoodsRecNoteDet>(det.Id);
                detailHistory = AuditUtility.GetAuditableObject<GoodsRecNoteDet>(OldDet, det);
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
                    var goodRecNoteDetHist = new GoodsRecNoteDetHist()
                    {
                        GoodsReceiptNoteDetId = det.Id,
                        FieldName = Hdr.FieldName,
                        PrevValue = Hdr.OldValue,
                        CurrentValue = Hdr.NewValue,
                        Active = "Y",
                        Id = Guid.NewGuid()
                    };
                    _repository.Add(goodRecNoteDetHist);
                });

            }

            if (saveChanges)
                _repository.SaveChanges();
        }

        private void InsertUpdateGoodsRecNoteApproval(GoodsRecNoteStatusHist statusHis, bool saveChanges)
        {
            if (statusHis.Id == Guid.Empty)
            {
                statusHis.Id = Guid.NewGuid();
                _repository.Add(statusHis, false);
            }
            else
                _repository.Update(statusHis, false);

            var comment = new GoodsRecNoteComment()
            {
                Active = "Y",
                Comments = statusHis.Comments,
                Id = Guid.NewGuid(),
                GoodsReceiptNoteId = statusHis.GoodsReceiptNoteId,
            };
            InsertGoodsRecNoteComments(comment, false);

            if (saveChanges)
                _repository.SaveChanges();
        }

        private void InsertGoodsRecNoteComments(GoodsRecNoteComment comment, bool saveChanges)
        {
            comment.Id = Guid.NewGuid();
            _repository.Add(comment);

            if (comment.AppDocuments != null && comment.AppDocuments.Count > 0)
            {
                foreach (var document in comment.AppDocuments)
                {
                    document.TransactionId = comment.Id;
                    document.TransactionType = "GRNCOMM";
                    document.Active = "Y";
                }

                var appDocumentBC = new AppDocumentBC(_logger, _repository);
                appDocumentBC.saveAppDocument(comment.AppDocuments, false);
            }
            if (saveChanges)
                _repository.SaveChanges();

        }

        private void InsertGoodsRecNoteDetComments(GoodsRecNoteDetComment comment, bool saveChanges)
        {
            comment.Id = Guid.NewGuid();
            _repository.Add(comment);

            if (comment.AppDocuments != null && comment.AppDocuments.Count > 0)
            {
                foreach (var document in comment.AppDocuments)
                {
                    document.TransactionId = comment.Id;
                    document.TransactionType = "GRNDETCOMM";
                    document.Active = "Y";
                }
                var appDocumentBC = new AppDocumentBC(_logger, _repository);
                appDocumentBC.saveAppDocument(comment.AppDocuments, false);
            }
            if (saveChanges)
                _repository.SaveChanges();

        }

        public List<GoodsRecNote> GetGoodsRecNotes(GoodsRecNoteSearch search, bool isExport = false)
        {

            if (search.FromTransDate <= DateTime.MinValue && search.ToTransDate <= DateTime.MinValue && string.IsNullOrEmpty(search.TransNo))
            {
                search.FromTransDate = DateTime.Now.AddMonths(-1);
                search.ToTransDate = DateTime.Now;
            }

            var grnQuery = _repository.GetQuery<GoodsRecNote>();
            var poQry = _repository.GetQuery<PurchaseOrder>();

            var grnReqQuery = (from a in grnQuery
                               join po in poQry on a.PurchaseOrderId equals po.Id into poreslut
                               from por in poreslut.DefaultIfEmpty()
                               where a.Active == "Y"
                               && (string.IsNullOrEmpty(search.TransNo) || a.TransNo == search.TransNo)
                               && (search.VendorMasterId == Guid.Empty || a.VendorMasterId == search.VendorMasterId)
                               && (string.IsNullOrEmpty(search.InvNo) || a.InvoiceNo == search.InvNo)
                               && (string.IsNullOrEmpty(search.Status) || a.Status == search.Status)
                               && (search.FromTransDate <= DateTime.MinValue || a.TransDate >= search.FromTransDate)
                               && (search.ToTransDate <= DateTime.MinValue || a.TransDate <= search.ToTransDate)
                               && (search.FromInvDate <= DateTime.MinValue || a.InvoiceDate >= search.FromInvDate)
                               && (search.ToInvDate <= DateTime.MinValue || a.InvoiceDate <= search.ToInvDate)
                               select new
                               {
                                   Grn = a,
                                   PO = por
                               }).OrderByDescending(a => a.Grn.TransNo).ToList();

            var grnList = new List<GoodsRecNote>();
            foreach (var grn in grnReqQuery)
            {
                grnList.Add(new GoodsRecNote()
                {
                    Active = grn.Grn.Active,
                    CreatedBy = grn.Grn.CreatedBy,
                    CreatedDate = grn.Grn.CreatedDate,
                    Id = grn.Grn.Id,
                    InvoiceDate = grn.Grn.InvoiceDate,
                    InvoiceNo = grn.Grn.InvoiceNo,
                    PurchaseOrderId = grn.Grn.PurchaseOrderId,
                    ModifiedBy = grn.Grn.ModifiedBy,
                    ModifiedDate = grn.Grn.ModifiedDate,
                    Remarks = grn.Grn.Remarks,
                    Status = grn.Grn.Status,
                    WareHouseLocationId = grn.Grn.WareHouseLocationId,
                    TransDate = grn.Grn.TransDate,
                    TransNo = grn.Grn.TransNo,
                    SeqNo = grn.Grn.SeqNo,
                    VendorMasterId = grn.Grn.VendorMasterId,
                    PurchaseOrderNo = grn.PO == null ? "" : grn.PO.TransNo.ToString(),
                });
            }
            if (isExport)
            {
                var grnDet = _repository.GetQuery<GoodsRecNoteDet>().Where(a => a.Active == "Y").ToList();
                var vendor = _repository.GetQuery<VendorMaster>().Where(a => a.Active == "Y").ToList();
                var product = _repository.GetQuery<ProductMaster>().Where(a => a.Active == "Y").ToList();
                var unit = _repository.GetQuery<ProdUnitMaster>().Where(a => a.Active == "Y").ToList();
                var wareHouseLocationList = _repository.List<WareHouseLocation>(a => a.Active == "Y");
                var wareHouseList = _repository.List<WareHouse>(a => a.Active == "Y");
                foreach (var grn in grnList)
                {
                    var vendorName = vendor.FirstOrDefault(a => a.Id == grn.VendorMasterId);
                    grn.VendorName = $"{vendorName?.Title} {" "} {vendorName?.Name}";
                    grn.GoodsReceiptNoteDet = grnDet.Where(a => a.GoodsReceiptNoteId == grn.Id).ToList();
                    var loc = wareHouseLocationList.FirstOrDefault(a => a.Id == grn?.WareHouseLocationId);
                    var warehouse = wareHouseList.FirstOrDefault(a => a.Id == loc?.WarehouseId);
                    grn.WareHouseLocation = $"{warehouse?.Name} {" "} {loc?.Name}";
                    if (grn.GoodsReceiptNoteDet != null && grn.GoodsReceiptNoteDet.Count > 0)
                    {
                        foreach (var det in grn.GoodsReceiptNoteDet)
                        {
                            det.ProductName = product.FirstOrDefault(a => a.Id == det.ProductMasterId).ProdDescription;
                            det.UnitName = unit.FirstOrDefault(a => a.Id == det.UnitMasterId).UnitName;
                            det.Remark = det.Remarks;
                        }
                    }
                }
            }

            return grnList;
        }

        public AppResponse SendGrnSendMail(Guid purchaseId)
        {
            VendorMailSend MailSend = new VendorMailSend();
            List<GoodsRecNoteDet> grnDet = new List<GoodsRecNoteDet>();
            GoodsRecNoteDet noteDet = new GoodsRecNoteDet();
            var purord = _repository.GetQuery<PurchaseOrder>();
            var Grn = _repository.GetById<GoodsRecNote>(purchaseId);
            grnDet = _repository.GetQuery<GoodsRecNoteDet>().Where(a => a.Active == "Y" && a.GoodsReceiptNoteId == purchaseId).ToList();
            MailSend.QuotionReqNo = Grn.TransNo;
            MailSend.purchaseTransNo = purord.FirstOrDefault(a => a.Id == Grn.PurchaseOrderId).TransNo;
            MailSend.QuotationReqDate = Grn.TransDate;
            MailSend.Remarks = Grn.Remarks;
            MailSend.ExportHeaderText = "Goods Receipt Notes";

            var EmployeeList = _repository.List<Employee>(q => q.Active == "Y");
            var department = _repository.List<Department>(w => w.Active == "Y" && w.Type == "FINANCEDEP");

            if (EmployeeList != null)
            {

                EmployeeList.ForEach(t =>
                    {
                        var productMailDetData = new VendorMailDet();
                        var eployeeresult = department.FirstOrDefault(a => a.Id == t.CurrDepartmentId);
                        if (eployeeresult != null)
                        {
                            productMailDetData.ToMail = t.Email;
                            MailSend.VendorMailDets.Add(productMailDetData);
                        }

                    });


            }

            if (grnDet != null)
            {
                var location = _repository.List<WareHouseLocation>(a => a.Active == "Y");
                var units = _repository.List<ProdUnitMaster>(a => a.Active == "Y");
                var products = _repository.List<ProductMaster>(a => a.Active == "Y");

                foreach (var det in grnDet)
                {
                    var product = products.FirstOrDefault(a => a.Id == det.ProductMasterId);
                    var unit = units.FirstOrDefault(a => a.Id == det.UnitMasterId);
                    det.ProductName = product != null ? product.ProdDescription : "";
                    det.UnitName = unit != null ? unit.UnitName : "";
                }
                noteDet.Amount = grnDet.Sum(item => item.Amount);
                noteDet.Quantity = grnDet.Sum(item => item.Quantity);
                noteDet.WareHouseLocation = "Total";
                grnDet.Add(noteDet);
            }


            if (MailSend != null && MailSend.VendorMailDets != null)
            {
                MailSend.VendorMailDets.ForEach(mail =>
                {
                    int SNo = 1;

                    if (grnDet != null)
                    {
                        grnDet.ForEach(data =>
                            {
                                data.SNo = SNo;
                                SNo++;
                            });
                    }

                    var file = new QuotationDetSendMail<GoodsRecNoteDet>
                    {
                        gridData = grnDet,
                        _headerText = MailSend.ExportHeaderText,
                        _repository = _repository,
                        _userContext = _userContext,
                        _logger = _logger,
                        vendorMailSend = MailSend,

                    };
                    buff = file.getByte();

                    Guid guid = Guid.NewGuid();
                    string key = guid.ToString();

                    var stoemail = mail.ToMail;
                    var femail = ERPSettings.APPSYSTEMSETTINGS[APPSystemsettingsKey.SENDEREMAILID];
                    var EmailTemlate = ERPSettings.GoodsReceiptNotesMailTemplate;
                    string body = string.Empty;
                    var root = AppDomain.CurrentDomain.BaseDirectory; using (var reader = new StreamReader(root + EmailTemlate))
                    {
                        string readFile = reader.ReadToEnd();
                        string StrContent = string.Empty;
                        StrContent = readFile;
                        StrContent = StrContent.Replace("#PURCHASENO#", MailSend.purchaseTransNo.ToString());
                        body = StrContent.ToString();
                    }

                    MailMessage mailMessage = new MailMessage(femail, stoemail);
                    mailMessage.Subject = ERPSettings.GoodsReceiptNotesMailSubject;
                    mailMessage.Body = body;
                    mailMessage.IsBodyHtml = true;
                    mailMessage.Attachments.Add(new Attachment(new MemoryStream(buff), "Goods_Receipt_Notes.pdf"));
                    EmailUtility.SendEmail(mailMessage);

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