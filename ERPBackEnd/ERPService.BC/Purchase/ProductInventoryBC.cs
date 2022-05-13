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
    public class ProductInventoryBC
    {
        private ILogger _logger;
        private IRepository _repository;
        public ProductInventoryBC(ILogger logger, IRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }


        public AppResponse SaveProductInventoryList(ProductInventoryHdr inventoryHdr, bool saveChanges)
        {
            List<String> validationMessages = new List<string>();
            AppResponse appResponse = new AppResponse();
            bool validation = true;
            foreach (var inv in inventoryHdr.Inventories)
            {
                if (inv.ProductMasterId == Guid.Empty || (inv.StockIn <= 0 && inv.StockOut <= 0)
                    || inv.TransId == Guid.Empty || string.IsNullOrEmpty(inv.TransType))
                {
                    validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.MANDMISSING]);
                    validation = false;
                }
            }


            if (inventoryHdr.Inventories != null && validation)
            {
                var balanceList = GetProdInventoryBalance(new ProdInventoryBalance());

                var noStock = false;
                if (inventoryHdr.StockType == "I")
                {
                    foreach (var invDet in inventoryHdr.Inventories)
                    {
                        var balance = balanceList.FirstOrDefault(a => a.ProductMasterId == invDet.ProductMasterId && a.WareHouseLocationId == invDet.WareHouseLocationId
                               && a.ExpiryDate == invDet.ExpiryDate);
                        if (balance == null || balance.AvlQuantity <= 0)
                        {
                            noStock = true;
                            break;
                        }
                    }
                    if (noStock)
                    {
                        appResponse.Status = APPMessageKey.ONEORMOREERR;
                        validationMessages.Add(ERPExceptions.APP_MESSAGES[APPMessageKey.NOSTOCK]);
                    }
                }

                if (!noStock)
                {
                    InsertUpdateProdInventories(inventoryHdr.Inventories, saveChanges);
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

        private void InsertUpdateProdInventories(List<ProductInventory> inventories, bool saveChanges)
        {
            foreach (var inv in inventories)
            {
                if (inv.Id == Guid.Empty)
                {
                    inv.Id = Guid.NewGuid();
                    _repository.Add(inv, false);

                }
                else
                {
                    _repository.Update(inv, false);
                }
            }
            if (saveChanges)
                _repository.SaveChanges();
        }

        public List<ProdInventoryBalance> GetProdInventoryBalance(ProdInventoryBalance search, bool isExport = false)
        {
            var proInvQry = _repository.GetQuery<ProdInventoryBalance>();
            var prodCatQry = _repository.GetQuery<ProductCategory>();
            var prodQry = _repository.GetQuery<ProductMaster>();
            var balanceList = (from inv in proInvQry
                               join prod in prodQry on inv.ProductMasterId equals prod.Id
                               join pcat in prodCatQry on prod.ProdCategoryId equals pcat.Id
                               where inv.Active == "Y"
                                    && inv.AvlQuantity > 0
                                    && (search.WareHouseLocationId == Guid.Empty || search.WareHouseLocationId == inv.WareHouseLocationId)
                                    && (search.ProductMasterId == Guid.Empty || search.ProductMasterId == inv.ProductMasterId)
                                    && (search.ProdCategoryId == Guid.Empty || search.ProdCategoryId == pcat.Id)
                                    && (!search.ExpiryDate.HasValue || search.ExpiryDate <= DateTime.MinValue || search.ExpiryDate.Value == inv.ExpiryDate)
                               select inv
                                 ).OrderBy(a => a.ExpiryDate).ToList();

            if (isExport)
            {
                var location = _repository.GetQuery<WareHouseLocation>().Where(a => a.Active == "Y").ToList();
                foreach (var list in balanceList)
                {
                    var wareloc = location.FirstOrDefault(a => a.Id == list.WareHouseLocationId);
                    list.WareHouse = wareloc?.Name;
                    var product = prodQry.FirstOrDefault(a => a.Id == list.ProductMasterId);
                    list.ProductMaster = product?.ProdDescription;

                }
            }
            return balanceList;

        }

        public List<ProductInventory> GetProductInvTransactions(ProdInventorySearch search, bool isExport)
        {
            var proInvQry = _repository.GetQuery<ProductInventory>();
            var prodCatQry = _repository.GetQuery<ProductCategory>();
            var prodQry = _repository.GetQuery<ProductMaster>();
            var balanceList = (from inv in proInvQry
                               join prod in prodQry on inv.ProductMasterId equals prod.Id
                               join pcat in prodCatQry on prod.ProdCategoryId equals pcat.Id
                               where inv.Active == "Y"
                                    && (search.WareHouseLocationId == Guid.Empty || search.WareHouseLocationId == inv.WareHouseLocationId)
                                    && (search.ProductMasterId == Guid.Empty || search.ProductMasterId == inv.ProductMasterId)
                                    && (search.ProdCategoryId == Guid.Empty || search.ProdCategoryId == pcat.Id)
                                    && (string.IsNullOrEmpty(search.TransactionType) || search.TransactionType == inv.TransType)
                                    && (search.FromTransDate <= DateTime.MinValue || inv.TransDate >= search.FromTransDate)
                                    && (search.ToTransDate <= DateTime.MinValue || inv.TransDate <= search.ToTransDate)
                               select inv
                                 ).OrderBy(a => a.TransDate).ToList();

            var openingBalanceList = (from inv in proInvQry
                                      join prod in prodQry on inv.ProductMasterId equals prod.Id
                                      join pcat in prodCatQry on prod.ProdCategoryId equals pcat.Id
                                      where inv.Active == "Y"
                                           && (search.WareHouseLocationId == Guid.Empty || search.WareHouseLocationId == inv.WareHouseLocationId)
                                           && (search.ProductMasterId == Guid.Empty || search.ProductMasterId == inv.ProductMasterId)
                                           && (search.ProdCategoryId == Guid.Empty || search.ProdCategoryId == pcat.Id)
                                           && (string.IsNullOrEmpty(search.TransactionType) || search.TransactionType == inv.TransType)
                                           && inv.TransDate < search.FromTransDate
                                      select inv
                     ).ToList();

            var inventoryBalance = new List<ProductInventory>();
            var openingBal = openingBalanceList.Sum(a => (a.StockIn - a.StockOut));
            inventoryBalance.Add(new ProductInventory()
            {
                StockIn = openingBal,
                TransType = "TRNOPENINGBALANCE"

            });
            var vendor = _repository.List<VendorMaster>(a => a.Active == "Y");
            var employee = _repository.List<Employee>(a => a.Active == "Y");
            var wareHouseLocationList = _repository.List<WareHouseLocation>(a => a.Active == "Y");
            var wareHouseList = _repository.List<WareHouse>(a => a.Active == "Y");
            decimal closingBal = openingBal;
            foreach (var item in balanceList)
            {
                if (item.ActorType == "EMPLOYEE")
                {
                    item.Actor = employee.FirstOrDefault(a => a.Id == item?.ActorId).FullNameEng;
                }
                else if (item.ActorType == "WAREHOUSE")
                {
                    var loc = wareHouseLocationList.FirstOrDefault(a => a.Id == item.ActorId);
                    var warehouse = wareHouseList.FirstOrDefault(a => a.Id == loc.WarehouseId);
                    item.Actor = $"{warehouse?.Name} {" "} {loc?.Name}";
                }
                else if (item.ActorType == "VENDOR")
                {
                    item.Actor = vendor.FirstOrDefault(a => a.Id == item.ActorId).Name;
                }

                inventoryBalance.Add(item);
                closingBal += item.StockIn;
                closingBal -= item.StockOut;
            }
            inventoryBalance.Add(new ProductInventory()
            {
                StockOut = closingBal,
                TransType = "TRNCLOSINGBALANCE"

            });
            if (isExport)
            {
                var codeMasterDetailsList = _repository.GetQuery<CodesDetails>();

                wareHouseLocationList.ForEach(wareHouseLoc =>
                {
                    var wareHouseName = wareHouseList.FirstOrDefault(a => a.Id == wareHouseLoc.WarehouseId);
                    wareHouseLoc.Name = (wareHouseName == null ? "" : wareHouseName.Name) + "-" + wareHouseLoc.Name;
                });

                inventoryBalance.ForEach(invBal =>
                {
                    var transTypeDec = codeMasterDetailsList.FirstOrDefault(a => a.Code == invBal.TransType);
                    var wareHouseName = wareHouseLocationList.FirstOrDefault(a => a.Id == invBal?.WareHouseLocationId);
                    var loc = wareHouseLocationList.FirstOrDefault(a => a.Id == wareHouseName?.WarehouseId);
                    var productName = prodQry.FirstOrDefault(a => a.Id == invBal.ProductMasterId);
                    invBal.TransactionType = transTypeDec?.Description;
                    invBal.Warehouse = $"{wareHouseName?.Name} {" "} {loc?.Name}";
                    invBal.Product = productName?.ProdDescription;
                    invBal.ExpiryDate = invBal?.ExpiryDate;
                    invBal.StockIn = invBal == null ? 0 : invBal.StockIn;
                    invBal.StockOut = invBal == null ? 0 : invBal.StockOut;

                });
            }

            return inventoryBalance;

        }
    }
}