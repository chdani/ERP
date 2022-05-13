using ERPService.Common.Interfaces;
using ERPService.DataModel.CTO;
using ERPService.DataModel.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace ERPService.BC.Utility
{
    public static class AppGeneralMethods
    {

        public static void RemoveCache(string key, IRepository _repository)
        {
            //remove cache values from cache.
            ObjectCache cache = MemoryCache.Default;
            cache.Remove(key);
            var systemSettings = _repository.GetQuery<SystemSetting>().FirstOrDefault(a => a.ConfigKey == "USR_LANG");
            if (systemSettings != null)
            {
                var systemSettingConfigValue = systemSettings.ConfigValue.Split(',').ToList();
                foreach (var lang in systemSettingConfigValue)
                    cache.Remove(key + lang);
            }
        }
        public static (string, long) TranstypeSeqNumber(string transactionType, IRepository _repository)
        {
            var TransNo = "";
            long SeqNo = 0;
            var currentYear = DateTime.Now.Year.ToString();
            if (transactionType != null)
            {
                switch (transactionType)
                {
                    case "serviceRequestTransType":
                        {
                            var serviceRequestCount = _repository.GetQuery<ServiceRequest>().Count();
                            if (serviceRequestCount > 0)
                            {
                                var maxSeqNoForServiceReq = _repository.GetQuery<ServiceRequest>().Max(a => a.SeqNo);
                                SeqNo = maxSeqNoForServiceReq + 1;
                            }
                            else
                            {
                                SeqNo = 1;
                            }
                            TransNo = string.Format("{0}1{1}", currentYear, SeqNo.ToString("00000#"));
                        }

                        break;
                    case "quotationRequestTransType":
                        {
                            var quotationRequestCount = _repository.GetQuery<QuotationRequest>().Count();
                            if (quotationRequestCount > 0)
                            {
                                var maxSeqNoForQuotationReq = _repository.GetQuery<QuotationRequest>().Max(a => a.SeqNo);
                                SeqNo = maxSeqNoForQuotationReq + 1;
                            }
                            else
                            {
                                SeqNo = 1;
                            }
                            TransNo = string.Format("{0}2{1}", currentYear, SeqNo.ToString("00000#"));

                        }
                        break;

                    case "vendorQuotationTransType":
                        {
                            var vendorQuotationCount = _repository.GetQuery<VendorQuotation>().Count();
                            if (vendorQuotationCount > 0)
                            {
                                var maxSeqNoForVendorQuotation = _repository.GetQuery<VendorQuotation>().Max(a => a.SeqNo);
                                SeqNo = maxSeqNoForVendorQuotation + 1;
                            }
                            else
                            {
                                SeqNo = 1;
                            }
                            TransNo = string.Format("{0}3{1}", currentYear, SeqNo.ToString("00000#"));

                        }
                        break;
                    case "purchaseRequestTransType":
                        {
                            var purchaseRequestCount = _repository.GetQuery<PurchaseRequest>().Count();
                            if (purchaseRequestCount > 0)
                            {
                                var maxSeqNoForPurchaseRequest = _repository.GetQuery<PurchaseRequest>().Max(a => a.SeqNo);
                                SeqNo = maxSeqNoForPurchaseRequest + 1;
                            }
                            else
                            {
                                SeqNo = 1;
                            }
                            TransNo = string.Format("{0}4{1}", currentYear, SeqNo.ToString("00000#"));

                        }
                        break;
                    case "PurchaseOrderTransType":
                        {
                            var purchaseOrderCount = _repository.GetQuery<PurchaseOrder>().Count();
                            if (purchaseOrderCount > 0)
                            {
                                var maxSeqNoForPurchaseOrder = _repository.GetQuery<PurchaseOrder>().Max(a => a.SeqNo);
                                SeqNo = maxSeqNoForPurchaseOrder + 1;
                            }
                            else
                            {
                                SeqNo = 1;
                            }
                            TransNo = string.Format("{0}5{1}", currentYear, SeqNo.ToString("00000#"));

                        }
                        break;
                    case "GoodsReceiptnoteTransType":
                        {

                            var goodsRecNoteCount = _repository.GetQuery<GoodsRecNote>().Count();
                            if (goodsRecNoteCount > 0)
                            {
                                var maxSeqNoForGoodsReceiptnote = _repository.GetQuery<GoodsRecNote>().Max(a => a.SeqNo);
                                SeqNo = maxSeqNoForGoodsReceiptnote + 1;
                            }
                            else
                            {
                                SeqNo = 1;
                            }
                            TransNo = string.Format("{0}6{1}", currentYear, SeqNo.ToString("00000#"));
                        }
                        break;
                    case "IssueTransactionTransType":
                        {
                            var prodInvIssueCount = _repository.GetQuery<ProdInvIssue>().Count();
                            if (prodInvIssueCount > 0)
                            {
                                var maxSeqNoForIssueTransaction = _repository.GetQuery<ProdInvIssue>().Max(a => a.SeqNo);
                                SeqNo = maxSeqNoForIssueTransaction + 1;
                            }
                            else
                            {
                                SeqNo = 1;
                            }
                            TransNo = string.Format("{0}7{1}", currentYear, SeqNo.ToString("00000#"));
                        }
                        break;
                    case "InventoryTransferTransType":
                        {
                            var ProdInvIssueCount = _repository.GetQuery<InventoryTransfer>().Count();
                            if (ProdInvIssueCount > 0)
                            {
                                var maxSeqNoForInventoryTransfer = _repository.GetQuery<InventoryTransfer>().Max(a => a.SeqNo);
                                SeqNo = maxSeqNoForInventoryTransfer + 1;
                            }
                            else
                            {
                                SeqNo = 1;
                            }
                            TransNo = string.Format("{0}8{1}", currentYear, SeqNo.ToString("00000#"));
                        }
                        break;

                }
            }
            return (TransNo, SeqNo);
        }
    }
}
