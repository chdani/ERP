using ERPService.BC.Utility;
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
    public class InventoryTransferBC
    {

        private ILogger _logger;
        private IRepository _repository;
        private UserContext _userContext;
        private ObjectCache cache = MemoryCache.Default;
        private object search;
        private byte[] buff;
        public InventoryTransferBC(ILogger logger, IRepository repository, UserContext userContext)
        {
            _logger = logger;
            _repository = repository;
            _userContext = userContext;
        }
        public InventoryTransfer GetInventoryTransferById(Guid requestId)
        {
            var inventoryTransfer = _repository.GetById<InventoryTransfer>(requestId);
            if (inventoryTransfer != null)
                inventoryTransfer.inventoryTransferDets = GetInventoryTransferDetByHdrId(requestId);
            return inventoryTransfer;
        }
        public List<InventoryTransferDet> GetInventoryTransferDetByHdrId(Guid hdrId)
        {
            return _repository.GetQuery<InventoryTransferDet>().Where(a => a.Active == "Y" && a.InventoryTransferId == hdrId).ToList(); ;
        }
        public List<InventoryTransferDet> GetInventoryTransferDetList(Guid transferId)
        {
            List<InventoryTransferDet> InventoryTransferDets = null;
            InventoryTransferDets = _repository.GetQuery<InventoryTransferDet>().Where(a => a.InventoryTransferId == transferId && a.Active == "Y").ToList();

            foreach (var InventoryTransferDet in InventoryTransferDets)
            {
                InventoryTransferDet.ProductDetail = _repository.GetQuery<ProductMaster>().Where(a => a.Id == InventoryTransferDet.ProductMasterId && a.Active == "Y").FirstOrDefault();
                InventoryTransferDet.ProductUnitDetail = _repository.GetQuery<ProdUnitMaster>().Where(a => a.Id == InventoryTransferDet.UnitMasterId && a.Active == "Y").FirstOrDefault();
            }
            return InventoryTransferDets;
        }
        public List<InventoryTransfer> GetinventoryTransferList(InventoryTransfSearchCTO search, bool isExport = false)
        {
            if (search.FromTransDate <= DateTime.MinValue && search.ToTransDate <= DateTime.MinValue && string.IsNullOrEmpty(search.TransNo) && string.IsNullOrEmpty(search.Status))
            {
                search.FromTransDate = DateTime.Now.AddMonths(-1);
                search.ToTransDate = DateTime.Now;
            }

            var InventoryTransferList = _repository.GetQuery<InventoryTransfer>().Where(a =>
               (string.IsNullOrEmpty(search.TransNo) || a.TransNo == search.TransNo)
             && (Guid.Empty == search.ToWareHouseLocationId || a.ToWareHouseLocationId == search.ToWareHouseLocationId)
             && (Guid.Empty == search.FromWareHouseLocationId || a.FromWareHouseLocationId == search.FromWareHouseLocationId)
             && (search.FromTransDate <= DateTime.MinValue || a.TransDate >= search.FromTransDate)
             && (search.ToTransDate <= DateTime.MinValue || a.TransDate <= search.ToTransDate)
             && (string.IsNullOrEmpty(search.Status) || a.Status == search.Status)
             && a.Active == "Y").OrderByDescending(a => a.TransNo).ToList();



            foreach (var InventoryTransfer in InventoryTransferList)
            {
                InventoryTransfer.inventoryTransferDets = _repository.GetQuery<InventoryTransferDet>().Where(a => a.InventoryTransferId == InventoryTransfer.Id && a.Active == "Y").ToList();
                InventoryTransfer.toWareHouseLocationDet = _repository.GetQuery<WareHouseLocation>().Where(a => a.Id == InventoryTransfer.ToWareHouseLocationId && a.Active == "Y").FirstOrDefault();
                InventoryTransfer.fromWareHouseLocationDet = _repository.GetQuery<WareHouseLocation>().Where(a => a.Id == InventoryTransfer.FromWareHouseLocationId && a.Active == "Y").FirstOrDefault();
                InventoryTransfer.toWareHouseDet = _repository.GetQuery<WareHouse>().Where(a => a.Id == InventoryTransfer.toWareHouseLocationDet.WarehouseId && a.Active == "Y").FirstOrDefault();
                InventoryTransfer.fromWareHouseDet = _repository.GetQuery<WareHouse>().Where(a => a.Id == InventoryTransfer.fromWareHouseLocationDet.WarehouseId && a.Active == "Y").FirstOrDefault();
            }



            if (isExport)
            {
                var product = _repository.List<ProductMaster>(a => a.Active == "Y");
                var unit = _repository.List<ProdUnitMaster>(a => a.Active == "Y");
                foreach (var InventoryTransfer in InventoryTransferList)
                {
                    InventoryTransfer.FromWareHouseLocation = $"{  InventoryTransfer.fromWareHouseLocationDet.Name}{""}{  InventoryTransfer.fromWareHouseLocationDet.Address}";
                    InventoryTransfer.FromWareHouse = $"{InventoryTransfer.fromWareHouseDet.Name}{""}{   InventoryTransfer.fromWareHouseDet.Address}";
                    InventoryTransfer.ToWareHouseLocation = $"{InventoryTransfer.toWareHouseLocationDet.Name}{""}{InventoryTransfer.toWareHouseLocationDet.Address}";
                    InventoryTransfer.ToWareHouse = $"{ InventoryTransfer.toWareHouseDet.Name}{""}{ InventoryTransfer.toWareHouseDet.Address}";
                    foreach (var InventoryTransferDet in InventoryTransfer.inventoryTransferDets)
                    {
                        InventoryTransferDet.ProductMaster = product.FirstOrDefault(a => a.Id == InventoryTransferDet.ProductMasterId ).ProdDescription;
                        InventoryTransferDet.UnitMaster = unit.FirstOrDefault(a => a.Id == InventoryTransferDet.UnitMasterId ).UnitName;
                        InventoryTransferDet.Remark = InventoryTransferDet.Remarks;
                    }
                }
            }             
               

            return InventoryTransferList;
        }
        public AppResponse SaveInventoryTransfer(InventoryTransfer inventoryTransfer)
        {
            List<String> validationMessages = new List<string>();
            AppResponse appResponse = new AppResponse();
            bool validation = true;

            if (inventoryTransfer.TransDate <= DateTime.MinValue)
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.MANDMISSING]);
                validation = false;
            }
            if (inventoryTransfer.Active == "Y")
            {
                if (inventoryTransfer.inventoryTransferDets == null || inventoryTransfer.inventoryTransferDets.Count == 0)
                {
                    validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.NOTDETROWEXIST]);
                    validation = false;
                }
            }

            if (inventoryTransfer.Id == Guid.Empty)
            {
                var inventoryTransferTransNoAndSeqNo = AppGeneralMethods.TranstypeSeqNumber("InventoryTransferTransType", _repository);
                inventoryTransfer.TransNo = inventoryTransferTransNoAndSeqNo.Item1;
                inventoryTransfer.SeqNo = inventoryTransferTransNoAndSeqNo.Item2;
            }
            if (inventoryTransfer != null && validation)
            {

                inventoryTransfer.TransDate = inventoryTransfer.TransDate.ToLocalTime().Date;
                inventoryTransfer.Status = "PURTRNSTSSUBMITTED";

                InsertUpdateInvTransfer(inventoryTransfer);
                appResponse.Status = APPMessageKey.DATASAVESUCSS;
            }
            else
            {
                appResponse.Status = APPMessageKey.ONEORMOREERR;
                appResponse.Messages = validationMessages;
            }
            return appResponse;
        }
        public AppResponse SaveInventoryTransferComment(InvTransferComment invTransferComment)
        {
            InsertInvTransferComments(invTransferComment, true);
            return new AppResponse() { Status = APPMessageKey.DATASAVESUCSS };
        }
        public AppResponse SaveInventoryTransferDetComment(InvTransferDetComment invTransferDetComment)
        {
            InsertInventoryTransferDetsComments(invTransferDetComment, true);
            return new AppResponse() { Status = APPMessageKey.DATASAVESUCSS };
        }
        public List<InvTransferHist> GetInvTransferHistory(Guid invTransferHistId)
        {
            var usemaster = _repository.GetQuery<UserMaster>().ToList();
            var invTransferHist = _repository.GetQuery<InvTransferHist>().Where(a => a.InventoryTransferId == invTransferHistId).OrderByDescending(a => a.CreatedDate).ToList();
            invTransferHist.ForEach(Hsty =>
            {
                var userName = usemaster.Where(a => a.Id == Hsty.CreatedBy).FirstOrDefault();
                if (userName != null)
                    Hsty.UserName = userName.FirstName + " " + userName.LastName;
            });
            return invTransferHist.OrderByDescending(a => a.CreatedDate).ToList();
        }
        public List<InvTransferDetHist> GetInvTransferDetHistory(Guid inventoryTransferDetId)
        {
            var usemaster = _repository.GetQuery<UserMaster>().ToList();
            var invTransferDetHist = _repository.GetQuery<InvTransferDetHist>().Where(a => a.InventoryTransferDetId == inventoryTransferDetId).OrderByDescending(a => a.CreatedDate).ToList();
            invTransferDetHist.ForEach(Hsty =>
            {
                var userName = usemaster.Where(a => a.Id == Hsty.CreatedBy).FirstOrDefault();
                if (userName != null)
                    Hsty.UserName = userName.FirstName + " " + userName.LastName;
            });
            return invTransferDetHist.OrderByDescending(a => a.CreatedDate).ToList();
        }
        public List<InvTransferComment> GetInvTransferComment(Guid inventoryTransferId)
        {
            var comments = _repository.GetQuery<InvTransferComment>().Where(a => a.InventoryTransferId == inventoryTransferId).ToList();
            var commentIds = comments.Select(a => a.Id).Distinct();

            var User = _repository.GetQuery<UserMaster>().ToList();

            var appDoucuments = _repository.GetQuery<AppDocument>().Where(a => a.Active == "Y"
                    && a.TransactionType == "INVTRANCOMM" && commentIds.Contains(a.TransactionId)).ToList();

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
        public List<InvTransferDetComment> GetInvTransferDetComment(Guid inventoryTransferDetsId)
        {
            var comments = _repository.GetQuery<InvTransferDetComment>().Where(a => a.InventoryTransferDetId == inventoryTransferDetsId).ToList();
            var commentIds = comments.Select(a => a.Id).Distinct();

            var User = _repository.GetQuery<UserMaster>().ToList();

            var appDoucuments = _repository.GetQuery<AppDocument>().Where(a => a.Active == "Y"
                    && a.TransactionType == "INVTRANDETCOMM" && commentIds.Contains(a.TransactionId)).ToList();

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
        public List<AppDocument> GeInvTransferAttachments(Guid inventoryTransferId)
        {
            var usemaster = _repository.GetQuery<UserMaster>().ToList();

            var comments = _repository.GetQuery<InvTransferComment>().Where(a => a.InventoryTransferId == inventoryTransferId && a.Active == "Y").ToList();

            var fileAttachmentIds = new List<Guid>();
            fileAttachmentIds.AddRange(comments.Select(a => a.Id).Distinct().ToList());
            fileAttachmentIds.Add(inventoryTransferId);

            var appDoucuments = _repository.GetQuery<AppDocument>().Where(a => a.Active == "Y"
                    && (a.TransactionType == "INVTRANCOMM" || a.TransactionType == "INVTRAN") && fileAttachmentIds.Contains(a.TransactionId))
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
        public List<InvTransferStatusHist> GetInvTransferStatusHistory(Guid inventoryTransferId)
        {
            var usemaster = _repository.GetQuery<UserMaster>().Where(a => a.Active == "Y").ToList();
            var approvals = _repository.GetQuery<InvTransferStatusHist>().Where(a => a.InventoryTransferId == inventoryTransferId).OrderByDescending(a => a.CreatedDate).ToList();
            approvals.ForEach(status =>
            {
                var userName = usemaster.Where(a => a.Id == status.CreatedBy).FirstOrDefault();
                if (userName != null)
                    status.UserName = userName.FirstName + " " + userName.LastName;
            });

            return approvals.OrderByDescending(a => a.CreatedDate).ToList();
        }
        private List<string> ValidateChildRecords(List<InventoryTransferDet> inventoryTransferDet, out bool validation)
        {
            var validationMessages = new List<string>();
            validation = true;
            foreach (var det in inventoryTransferDet)
            {
                if (det.ProductMasterId == Guid.Empty
                    || det.Quantity <= 0
                    || det.UnitMasterId == Guid.Empty)
                {
                    validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.MANDMISSING]);
                    validation = false;
                }
            }

            return validationMessages;
        }
        public AppResponse SaveInvTransfer(InventoryTransfer invTransfer)
        {
            List<String> validationMessages = new List<string>();
            AppResponse appResponse = new AppResponse();
            bool validation = true;

            if (invTransfer.TransDate <= DateTime.MinValue)
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.MANDMISSING]);
                validation = false;
            }
            if (invTransfer.Active == "Y")
            {
                if (invTransfer.inventoryTransferDets == null || invTransfer.inventoryTransferDets.Count == 0)
                {
                    validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.NOTDETROWEXIST]);
                    validation = false;
                }
            }

            if (invTransfer != null && validation)
            {

                invTransfer.TransDate = invTransfer.TransDate.ToLocalTime().Date;
                invTransfer.Status = "PURTRNSTSSUBMITTED";

                InsertUpdateInvTransfer(invTransfer);
                appResponse.Status = APPMessageKey.DATASAVESUCSS;
            }
            else
            {
                appResponse.Status = APPMessageKey.ONEORMOREERR;
                appResponse.Messages = validationMessages;
            }
            return appResponse;
        }
        private void InsertUpdateInvTransfer(InventoryTransfer invTransfer)
        {
            var histories = new List<AppAudit>();

            if (invTransfer.Id == Guid.Empty)
            {
                histories.Add(new AppAudit() { FieldName = "APP_RECORD_CREATED", NewValue = "", OldValue = "" });
                invTransfer.Id = Guid.NewGuid();
                _repository.Add(invTransfer, false);
            }
            else
            {
                var OldValue = _repository.GetById<InventoryTransfer>(invTransfer.Id);
                histories = AuditUtility.GetAuditableObject(OldValue, invTransfer);
                _repository.Update(invTransfer, false);
            }


            if (histories != null)
            {
                histories.ForEach(Hdr =>
                {
                    var invTransferHist = new InvTransferHist()
                    {
                        InventoryTransferId = invTransfer.Id,
                        FieldName = Hdr.FieldName,
                        PrevValue = Hdr.OldValue,
                        CurrentValue = Hdr.NewValue,
                        Active = "Y",
                        Id = Guid.NewGuid()

                    };
                    _repository.Add(invTransferHist);
                });

            }

            if (invTransfer.inventoryTransferDets != null)
            {
                foreach (var det in invTransfer.inventoryTransferDets)
                {
                    det.InventoryTransferId = invTransfer.Id;
                    InsertUpdateinventoryTransferDets(det, false);
                }
            }

            var statusHistory = new InvTransferStatusHist()
            {
                Active = "Y",
                Comments = invTransfer.Remarks,
                Status = "PURTRNSTSSUBMITTED",
                InventoryTransferId = invTransfer.Id,
            };

            InsertUpdateinvTransferApproval(statusHistory, false);

            _repository.SaveChanges();
        }
        private void InsertUpdateinventoryTransferDets(InventoryTransferDet det, bool saveChanges)
        {

            var detailHistory = new List<AppAudit>();
            if (det.Id != Guid.Empty)
            {
                var OldDet = _repository.GetById<InventoryTransferDet>(det.Id);
                detailHistory = AuditUtility.GetAuditableObject<InventoryTransferDet>(OldDet, det);
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
                    var inventoryTransferDetsHist = new InvTransferDetHist()
                    {
                        InventoryTransferDetId = det.Id,
                        FieldName = Hdr.FieldName,
                        PrevValue = Hdr.OldValue,
                        CurrentValue = Hdr.NewValue,
                        Active = "Y",
                        Id = Guid.NewGuid()
                    };
                    _repository.Add(inventoryTransferDetsHist);
                });

            }

            if (saveChanges)
                _repository.SaveChanges();
        }
        private void InsertInvTransferComments(InvTransferComment comment, bool saveChanges)
        {
            comment.Id = Guid.NewGuid();
            _repository.Add(comment);

            if (comment.AppDocuments != null && comment.AppDocuments.Count > 0)
            {
                foreach (var document in comment.AppDocuments)
                {
                    document.TransactionId = comment.Id;
                    document.TransactionType = "INVTRANCOMM";
                    document.Active = "Y";
                }

                var appDocumentBC = new AppDocumentBC(_logger, _repository);
                appDocumentBC.saveAppDocument(comment.AppDocuments, false);
            }
            if (saveChanges)
                _repository.SaveChanges();

        }
        private void InsertInventoryTransferDetsComments(InvTransferDetComment comment, bool saveChanges)
        {
            comment.Id = Guid.NewGuid();
            _repository.Add(comment);

            if (comment.AppDocuments != null && comment.AppDocuments.Count > 0)
            {
                foreach (var document in comment.AppDocuments)
                {
                    document.TransactionId = comment.Id;
                    document.TransactionType = "PRODINVISSDETCOMM";
                    document.Active = "Y";
                }
                var appDocumentBC = new AppDocumentBC(_logger, _repository);
                appDocumentBC.saveAppDocument(comment.AppDocuments, false);
            }
            if (saveChanges)
                _repository.SaveChanges();

        }
        private void InsertUpdateinvTransferApproval(InvTransferStatusHist statusHis, bool saveChanges)
        {
            if (statusHis.Id == Guid.Empty)
            {
                statusHis.Id = Guid.NewGuid();
                _repository.Add(statusHis, false);
            }
            else
                _repository.Update(statusHis, false);

            var comment = new InvTransferComment()
            {
                Active = "Y",
                Comments = statusHis.Comments,
                Id = Guid.NewGuid(),
                InventoryTransferId = statusHis.InventoryTransferId,
            };
            InsertInvTransferComments(comment, false);

            if (saveChanges)
                _repository.SaveChanges();
        }
        public AppResponse ApproveInventoryTransfer(InventoryTransfer inventoryTransfer)
        {
            List<String> validationMessages = new List<string>();
            AppResponse appResponse = new AppResponse();
            bool validation = true;

            if (inventoryTransfer.Id == Guid.Empty || string.IsNullOrEmpty(inventoryTransfer.ApproverRemarks))
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.MANDMISSING]);
                validation = false;
            }

            var inventoryTransferDets = GetInventoryTransferDetByHdrId(inventoryTransfer.Id);
            if (inventoryTransferDets == null || inventoryTransferDets.Count <= 0)
            {
                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.MANDMISSING]);
                validation = false;
            }

            if (validation)
            {
                var statusHistory = new InvTransferStatusHist()
                {
                    Active = "Y",
                    Comments = inventoryTransfer.ApproverRemarks,
                    Status = inventoryTransfer.Status,
                    InventoryTransferId = inventoryTransfer.Id,
                };


                inventoryTransfer.inventoryTransferDets = null;
                var inventories = new List<ProductInventory>();
                var inventories1 = new List<ProductInventory>();
                if (inventoryTransfer.Status == "PURTRNSTSAPPROVED")
                {

                    var balanceStockBC = new ProductInventoryBC(_logger, _repository);
                    var input = new ProdInventoryBalance() { WareHouseLocationId = inventoryTransfer.FromWareHouseLocationId };
                    var stock = balanceStockBC.GetProdInventoryBalance(input);
                    if (stock != null && stock.Count > 0)
                    {
                        var units = _repository.GetQuery<ProdUnitMaster>();

                        foreach (var det in inventoryTransferDets)
                        {
                            var unit = units.FirstOrDefault(a => a.Id == det.UnitMasterId);
                            decimal convertQty = 1;
                            if (unit != null)
                                convertQty = unit.ConversionUnit;

                            var prodStock = stock.Where(a => a.ProductMasterId == det.ProductMasterId).ToList();
                            if (prodStock.Sum(a => a.AvlQuantity) < (det.Quantity * convertQty))
                            {
                                validation = false;
                                validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.NOSTOCK]);
                                break;
                            }
                            else
                            {
                                var balanceQty = det.Quantity * convertQty;
                                var inventoryTransferTransno = long.Parse(inventoryTransfer.TransNo);
                                foreach (var stk in prodStock)
                                {
                                    var reduceQty = stk.AvlQuantity > balanceQty ? balanceQty : stk.AvlQuantity;

                                    balanceQty -= reduceQty;
                                    inventories.Add(new ProductInventory
                                    {
                                        Active = "Y",
                                        TransDate = inventoryTransfer.TransDate,
                                        ProductMasterId = det.ProductMasterId,
                                        StockOut = (reduceQty == null ? 0 : decimal.Parse(reduceQty.ToString())),
                                        TransId = det.Id,
                                        ShelveNo = det.ShelveNo,
                                        Barcode = det.Barcode,
                                        TransNo = inventoryTransferTransno,
                                        ActorType = "WAREHOUSE",
                                        ActorId = inventoryTransfer.ToWareHouseLocationId,
                                        TransType = "TRANSFERDET",
                                        ExpiryDate = stk.ExpiryDate,
                                        WareHouseLocationId = inventoryTransfer.FromWareHouseLocationId,
                                        Id = Guid.NewGuid()
                                    });

                                    inventories1.Add(new ProductInventory
                                    {
                                        Active = "Y",
                                        TransDate = inventoryTransfer.TransDate,
                                        ProductMasterId = det.ProductMasterId,
                                        StockIn = (reduceQty == null ? 0 : decimal.Parse(reduceQty.ToString())),
                                        TransId = det.Id,
                                        ShelveNo = det.ShelveNo,
                                        Barcode = det.Barcode,
                                        TransNo = inventoryTransferTransno,
                                        ActorType = "WAREHOUSE",
                                        ActorId = inventoryTransfer.FromWareHouseLocationId,
                                        TransType = "TRANSFERDET",
                                        ExpiryDate = stk.ExpiryDate,
                                        WareHouseLocationId = inventoryTransfer.ToWareHouseLocationId,
                                        Id = Guid.NewGuid()
                                    });
                                    if (balanceQty <= 0)
                                        break;
                                }
                            }
                        }
                    }
                    else
                    {
                        validation = false;
                        validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.NOSTOCK]);
                    }
                }
                if (validation)
                {


                    if (validation)
                    {
                        if (inventories.Count > 0)
                        {
                            foreach (var inv in inventories)
                                _repository.Add(inv, false);
                        }
                        if (inventories1.Count > 0)
                        {
                            foreach (var inv in inventories1)
                                _repository.Add(inv, false);
                        }
                        _repository.Update(inventoryTransfer, false);

                        InsertUpdateinvTransferApproval(statusHistory, false);
                        _repository.SaveChanges();
                        appResponse.Status = APPMessageKey.DATASAVESUCSS;
                    }
                    else
                    {
                        appResponse.Status = APPMessageKey.ONEORMOREERR;
                        appResponse.Messages = validationMessages;
                    }

                }
            }
            else
            {
                appResponse.Status = APPMessageKey.ONEORMOREERR;
                appResponse.Messages = validationMessages;
            }
            return appResponse;
        }


        public List<ShelfsData> getShelfs(ShelfCodeReq Req)
        {
            List<ShelfsData> shelfs = new List<ShelfsData>();

            var data = _repository.GetQuery<ProductInventory>().Where(y => y.WareHouseLocationId == Req.fromWarehouseLocationId && y.ProductMasterId == Req.productMasterId && y.Active == "Y").Select(x => new { x.ShelveNo, x.Barcode }).ToList();
            foreach (var item in data)
            {
                ShelfsData shelf = new ShelfsData
                {
                    ShelveNo = item.ShelveNo,
                    Barcode = item.Barcode


                };
                shelfs.Add(shelf);
            }

            return shelfs;



        }

    }
}
