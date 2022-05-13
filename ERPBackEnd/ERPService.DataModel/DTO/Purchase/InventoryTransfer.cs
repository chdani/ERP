using ERPService.Common.Shared;
using ERPService.DataModel.CTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPService.DataModel.DTO
{
    public class InventoryTransfer : BaseEntity
    {


        public Guid FromWareHouseLocationId { get; set; }

        public Guid ToWareHouseLocationId { get; set; }
        [PDFExport(headerText = "REMARKS", order = 7, width = 50,  cellAlign = 1)]
        [ExcelExport(headerText = "REMARKS", order = 7)]
        public string Remarks { get; set; }

        public string Status { get; set; }

        public Guid? ApprovedBy { get; set; }

        public DateTime? ApprovedDate { get; set; }

        public string ApproverRemarks { get; set; }
        [PDFExport(headerText = "TRANSNO", order = 1, width = 50, cellAlign = 1)]
        [ExcelExport(headerText = "TRANSNO", order = 1)]
        public string TransNo { get; set; }
        public long SeqNo { get; set; }
        [PDFExport(headerText = "TRANSDATE", order = 2, width = 50, dateFormat = "dd-MM-yyyy", cellAlign = 1)]
        [ExcelExport(headerText = "TRANSDATE", order = 2, dateFormat = "dd-MM-yyyy")]
        public DateTime TransDate { get; set; }

        [NotMapped]
        [PDFExport(headerText = "TRANSDATE", order = 3, width = 50, cellAlign = 1)]
        [ExcelExport(headerText = "TRANSDATE", order = 3)]
        public string FromWareHouseLocation { get; set; }


        [NotMapped]
        [PDFExport(headerText = "TRANSDATE", order = 4, width = 50, cellAlign = 1)]
        [ExcelExport(headerText = "TRANSDATE", order = 4)]
        public string FromWareHouse { get; set; }

        [NotMapped]
        [PDFExport(headerText = "TRANSDATE", order = 5, width = 50,  cellAlign = 1)]
        [ExcelExport(headerText = "TRANSDATE", order = 5)]
        public string ToWareHouseLocation { get; set; }

        [NotMapped]
        [PDFExport(headerText = "TRANSDATE", order = 6, width = 50,  cellAlign = 1)]
        [ExcelExport(headerText = "TRANSDATE", order = 6)]
        public string ToWareHouse { get; set; }
        [NotMapped]
        [PDFExport(subHeaderText = "PRODUCTMASTER", order = 1, width = 50, cellAlign = 1)]
        [ExcelExport(subHeaderText = "PRODUCTMASTER", order = 1)]
        public string ProductMaster { get; set; }
        [NotMapped]
        [PDFExport(subHeaderText = "UNITMASTER", order = 2, width = 50, cellAlign = 1)]
        [ExcelExport(subHeaderText = "UNITMASTER", order = 2)]
        public string UnitMaster { get; set; }
        [NotMapped]
        [PDFExport(subHeaderText = "QUANTITY", order = 3, width = 50, cellAlign = 1)]
        [ExcelExport(subHeaderText = "QUANTITY", order = 3)]
        public decimal Quantity { get; set; }
      
        [NotMapped]
        [PDFExport(subHeaderText = "SHELVENO", order = 4, width = 50, cellAlign = 1)]
        [ExcelExport(subHeaderText = "SHELVENO", order = 4)]
        public string ShelveNo { get; set; }

        [NotMapped]
        [PDFExport(subHeaderText = "REMARKS", order = 5, width = 50, cellAlign = 1)]
        [ExcelExport(subHeaderText = "REMARKS", order = 5)]
        public string Remark { get; set; }

        [NotMapped]
        public WareHouseLocation fromWareHouseLocationDet { get; set; }

        [NotMapped]
        public WareHouseLocation toWareHouseLocationDet { get; set; }


        [NotMapped]
        public WareHouse fromWareHouseDet { get; set; }

        [NotMapped]
        public WareHouse toWareHouseDet { get; set; }


        [NotMapped]
        public List<InventoryTransferDet> inventoryTransferDets { get; set; }

    }
}
