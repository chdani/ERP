using ERPService.Common;
using ERPService.Common.Shared;
using ERPService.DataModel.CTO;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPService.DataModel.DTO
{
    public class ProductInventory : BaseEntity
    {
        public Guid TransId { get; set; }
        [PDFExport(headerText = "TRANSNO", order = 1, width = 50, cellAlign = 1)]
        [ExcelExport(headerText = "TRANSNO", order = 1)]
        public long TransNo { get; set; }
        public string ActorType { get; set; }
        public Guid ActorId { get; set; }
        public string TransType { get; set; }
        [PDFExport(headerText = "TRANSDATE", order = 2, width = 50, dateFormat = "dd-MM-yyyy", cellAlign = 1)]
        [ExcelExport(headerText = "TRANSDATE", order = 2, dateFormat = "dd-MM-yyyy")]
        public DateTime TransDate { get; set; }
        public Guid ProductMasterId { get; set; }
        public Guid WareHouseLocationId { get; set; }

        public string ShelveNo { get; set; }
        public string Barcode { get; set; }
        [NotMapped]
        [PDFExport(headerText = "ACTOR", order = 4, width = 50, cellAlign = 1)]
        [ExcelExport(headerText = "ACTOR", order = 4)]
        public string Actor { get; set; }

        [PDFExport(headerText = "STOCK_IN", order = 8, width = 50, currencyFormat = "#,###", cellAlign = 1)]
        [ExcelExport(headerText = "STOCK_IN", currencyFormat = "#,###", order = 8)]
        public decimal StockIn { get; set; }
        [PDFExport(headerText = "STOCK_OUT", order = 9, width = 50, currencyFormat = "#,###", cellAlign = 1)]
        [ExcelExport(headerText = "STOCK_OUT", currencyFormat = "#,###", order = 9)]
        public decimal StockOut { get; set; }
        [PDFExport(headerText = "EXPIRYDATE", order = 7, width = 50, dateFormat = "dd-MM-yyyy", cellAlign = 1)]
        [ExcelExport(headerText = "EXPIRYDATE", dateFormat = "dd-MM-yyyy", order = 7)]
        public DateTime? ExpiryDate { get; set; }
        [NotMapped]
        [PDFExport(headerText = "TRANSACTION_TYPE", order = 3, width = 50, cellAlign = 1)]
        [ExcelExport(headerText = "TRANSACTION_TYPE", order = 3)]
        public string TransactionType { get; set; }
        [NotMapped]
        [PDFExport(headerText = "WAREHOUSE", order = 5, width = 50, cellAlign = 1)]
        [ExcelExport(headerText = "WAREHOUSE", order = 5)]
        public string Warehouse { get; set; }
        [NotMapped]
        [PDFExport(headerText = "PRODUCTMASTER", order = 6, width = 50, cellAlign = 1)]
        [ExcelExport(headerText = "PRODUCTMASTER", order = 6)]
        public string Product { get; set; }
    }
}
