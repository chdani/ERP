
using ERPService.Common.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPService.DataModel.DTO
{
    public class ProductMaster : BaseEntity
    {
        public Guid ProdCategoryId { get; set; }
        public string ProdCode { get; set; }
        public string ProdDescription { get; set; }
        public string Barcode { get; set; }
        public decimal ReOrderLevel { get; set; }
        public Guid DefaultUnitId { get; set; }
        public bool IsExpirable { get; set; }
        public bool IsStockable { get; set; }
        public Guid ProdSubCategoryId { get; set; }


    }
}

