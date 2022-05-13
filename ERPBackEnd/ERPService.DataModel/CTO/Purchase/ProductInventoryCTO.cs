using ERPService.Common.Shared;
using ERPService.DataModel.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPService.DataModel.CTO
{

    public class ProductInventoryHdr
    {
        public string StockType { get; set; } //I -- Issue, R - Receipt
        public List<ProductInventory> Inventories { get; set; }
    }
}
