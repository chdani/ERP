using ERPService.Common;
using ERPService.Common.Shared;
using ERPService.DataModel.CTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPService.DataModel.DTO
{
    public class GoodsRecNote : BaseEntity
    {
        [PDFExport(headerText = "TRANSNO", order = 1, width = 50, cellAlign = 1)]
        [ExcelExport(headerText = "TRANSNO", order = 1)]
        public string TransNo { get; set; }
        public long SeqNo { get; set; }
        [PDFExport(headerText = "TRANSDATE", order = 2, width = 50, dateFormat = "dd-MM-yyyy", cellAlign = 1)]
        [ExcelExport(headerText = "TRANSDATE", order = 2, dateFormat = "dd-MM-yyyy")]
        public DateTime TransDate { get; set; }

        [AuditableAttribute("GRN_INVOICE_INVNO")]
        [PDFExport(headerText = "INVOICENO", order = 6, width = 50, cellAlign = 1)]
        [ExcelExport(headerText = "INVOICENO", order = 6)]
        public string InvoiceNo { get; set; }

        [AuditableAttribute("GRN_INVOICE_INVDATE", ERPConstants.DATE_FORMAT)]
        [PDFExport(headerText = "INVOICEDATE", order = 7, width = 50, dateFormat = "dd-MM-yyyy", cellAlign = 1)]
        [ExcelExport(headerText = "INVOICEDATE", dateFormat = "dd-MM-yyyy", order = 7)]
        public DateTime InvoiceDate { get; set; }

        public Guid PurchaseOrderId { get; set; }
        [AuditableAttribute("GRN_WH_LOCATION")]
        public Guid WareHouseLocationId { get; set; }

        public Guid VendorMasterId { get; set; }

        public string Status { get; set; }

        [AuditableAttribute("APP_REMARKS")]
        [PDFExport(headerText = "REMARKS", order = 8, width = 50, cellAlign = 1)]
        [ExcelExport(headerText = "REMARKS", order = 8)]
        public string Remarks { get; set; }

        [NotMapped]
        [PDFExport(headerText = "PONO", order = 3, width = 50, cellAlign = 1)]
        [ExcelExport(headerText = "PONO", order = 3)]
        public string PurchaseOrderNo { get; set; }

        [NotMapped]
        public string ApproverRemarks { get; set; }
        [NotMapped]
        [PDFExport(headerText = "VENDORNAME", order = 4, width = 50, cellAlign = 1)]
        [ExcelExport(headerText = "VENDORNAME", order = 4)]
        public string VendorName { get; set; }
        [NotMapped]
        public List<GoodsRecNoteDet> GoodsReceiptNoteDet { get; set; }

        [NotMapped]
        [PDFExport(headerText = "WAREHOUSE", order = 5, width = 50, cellAlign = 1)]
        [ExcelExport(headerText = "WAREHOUSE", order = 5)]
        public string WareHouseLocation { get; set; }

        [NotMapped]
        [PDFExport(subHeaderText = "PRODUCTMASTER", order = 1, width = 50, cellAlign = 1)]
        [ExcelExport(subHeaderText = "PRODUCTMASTER", order = 1)]
        public string ProductName { get; set; }
        [NotMapped]
        [PDFExport(subHeaderText = "UNITMASTER", order = 2, width = 50, cellAlign = 1)]
        [ExcelExport(subHeaderText = "UNITMASTER", order = 2)]
        public string UnitName { get; set; }

        [NotMapped]
        [PDFExport(subHeaderText = "QUANTITY", order = 3, currencyFormat = ERPConstants.NUMBER_WITH_DECIMAL_FORMAT, width = 50, cellAlign = 1)]
        [ExcelExport(subHeaderText = "QUANTITY", order = 3, currencyFormat = ERPConstants.NUMBER_WITH_DECIMAL_FORMAT)]

        public decimal Quantity { get; set; }
        [NotMapped]
        [PDFExport(subHeaderText = "SHELVENO", order = 4, width = 50, cellAlign = 1)]
        [ExcelExport(subHeaderText = "SHELVENO", order = 4)]
        public string ShelveNo { get; set; }
        [NotMapped]
        [PDFExport(subHeaderText = "BARCODE", order = 5, width = 50, cellAlign = 1)]
        [ExcelExport(subHeaderText = "BARCODE", order = 5)]
        public string Barcode { get; set; }
        [NotMapped]
        [PDFExport(subHeaderText = "PRICE", order = 6, currencyFormat = ERPConstants.NUMBER_WITH_DECIMAL_FORMAT, width = 50, cellAlign = 1)]
        [ExcelExport(subHeaderText = "PRICE", order = 6, currencyFormat = ERPConstants.NUMBER_WITH_DECIMAL_FORMAT)]
        public decimal Price { get; set; }

        [NotMapped]
        [PDFExport(subHeaderText = "AMOUNT", order = 7, currencyFormat = ERPConstants.NUMBER_WITH_DECIMAL_FORMAT, width = 50, cellAlign = 1)]
        [ExcelExport(subHeaderText = "AMOUNT", order = 7, currencyFormat = ERPConstants.NUMBER_WITH_DECIMAL_FORMAT)]
        public decimal Amount { get; set; }
        [NotMapped]
        [PDFExport(subHeaderText = "REMARKS", order = 8, width = 50, cellAlign = 1)]
        [ExcelExport(subHeaderText = "REMARKS", order = 8)]
        public string Remark { get; set; }




    }
}
