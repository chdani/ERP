using ERPService.Common.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPService.DataModel.DTO
{
    public class InventoryTransferDet : BaseEntity
    {


        public Guid InventoryTransferId { get; set; }

        public Guid ProductMasterId { get; set; }

        public Guid UnitMasterId { get; set; }

        public decimal? Quantity { get; set; }


        public string Barcode { get; set; }
        public string ShelveNo { get; set; }


        public string Remarks { get; set; }

        [NotMapped]
        public string ProductMaster { get; set; }
        [NotMapped]
        public string UnitMaster { get; set; }
        [NotMapped]
        public string Remark { get; set; }



        [NotMapped]
        public ProductMaster ProductDetail { get; set; }

        [NotMapped]
        public List<ShelfsData> ProductShelveNo { get; set; }



        [NotMapped]
        public ProdUnitMaster ProductUnitDetail { get; set; }

    }
}
