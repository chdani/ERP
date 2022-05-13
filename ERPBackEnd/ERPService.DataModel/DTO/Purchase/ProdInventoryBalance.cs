using ERPService.Common;
using ERPService.Common.Shared;
using ERPService.DataModel.CTO;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPService.DataModel.DTO
{
    public class ProdInventoryBalance : BaseEntity
    {
        public Guid ProductMasterId { get; set; }
        public Guid WareHouseLocationId { get; set; }
        [PDFExport(headerText = "EXPIRYDATE", order = 3, width = 50, dateFormat = "dd-MM-yyyy", cellAlign = 1)]
        [ExcelExport(headerText = "EXPIRYDATE", dateFormat = "dd-MM-yyyy", order = 3)]
        public DateTime? ExpiryDate { get; set; }
        [PDFExport(headerText = "QUANTITY", order = 4, currencyFormat = ERPConstants.NUMBER_WITH_DECIMAL_FORMAT, width = 50, cellAlign = 1)]
        [ExcelExport(headerText = "QUANTITY", currencyFormat = ERPConstants.NUMBER_WITH_DECIMAL_FORMAT, order = 4)]
        public decimal AvlQuantity { get; set; }
        public string ShelveNo { get; set; }
        [NotMapped]
        public Guid ProdCategoryId { get; set; }
        [NotMapped]
        public string ExportType { get; set; }
        [NotMapped]
        public string ExportHeaderText { get; set; }

        [NotMapped]
        [PDFExport(headerText = "WAREHOUSE", order = 1, width = 50, cellAlign = 1)]
        [ExcelExport(headerText = "WAREHOUSE", order = 1)]
        public string WareHouse { get; set; }
        [NotMapped]
        [PDFExport(headerText = "PRODUCTMASTER", order = 2, width = 50, cellAlign = 1)]
        [ExcelExport(headerText = "PRODUCTMASTER", order = 2)]
        public string ProductMaster { get; set; }

    }
}
