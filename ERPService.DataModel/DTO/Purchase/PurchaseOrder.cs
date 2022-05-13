using ERPService.Common;
using ERPService.Common.Shared;
using ERPService.DataModel.CTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPService.DataModel.DTO
{
    public class PurchaseOrder : BaseEntity
    {
        [AuditableAttribute("PURCHASE_NO")]
        [PDFExport(headerText = "TRANSNO", order = 1, width = 50, cellAlign = 1)]
        [ExcelExport(headerText = "TRANSNO", order = 1)]
        public string TransNo { get; set; }
        public long SeqNo { get; set; }
        [AuditableAttribute("PURCHASE_TRANSDATE", ERPConstants.DATE_FORMAT)]
        [PDFExport(headerText = "TRANSDATE", order = 2, width = 50, dateFormat = "dd-MM-yyyy", cellAlign = 1)]
        [ExcelExport(headerText = "TRANSDATE", order = 2, dateFormat = "dd-MM-yyyy")]
        public DateTime TransDate { get; set; }

        [PDFExport(headerText = "LEDGERCODE", order = 5, width = 50, cellAlign = 1)]
        [ExcelExport(headerText = "LEDGERCODE", order = 5)]
        public int LedgerCode { get; set; }
        [PDFExport(headerText = "COSTCENTERCODE", order = 6, width = 50, cellAlign = 1)]
        [ExcelExport(headerText = "COSTCENTERCODE", order = 6)]
        public string CostCenterCode { get; set; }
        public Guid OrgId { get; set; }
        public string FinYear { get; set; }
        public decimal TotalAmount { get; set; }
        public Guid PurchaseRequestId { get; set; }
        public Guid VendorMasterId { get; set; }
        [NotMapped]
        public string ledger { get; set; }
        [NotMapped]
        public string CostCenter { get; set; }
        [NotMapped]
        [PDFExport(headerText = "PURCHASEREQNO", order = 3, width = 50, cellAlign = 1)]
        [ExcelExport(headerText = "PURCHASEREQNO", order = 3)]
        public string PurchaseRequestNo { get; set; }
        public string Status { get; set; }
        [AuditableAttribute("APP_REMARKS")]
        [PDFExport(headerText = "REMARKS", order = 7, width = 50, cellAlign = 1)]
        [ExcelExport(headerText = "REMARKS", order = 7)]
        public string Remarks { get; set; }

        [NotMapped]
        public string ApproverRemarks { get; set; }
        [NotMapped]
        [PDFExport(headerText = "VENDORNAME", order = 4, width = 50, cellAlign = 1)]
        [ExcelExport(headerText = "VENDORNAME", order = 4)]
        public string Vendor { get; set; }
        [NotMapped]
        public List<PurchaseOrderDet> PurchaseOrderDet { get; set; }

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
        [PDFExport(subHeaderText = "PRICE", order = 4, currencyFormat = ERPConstants.NUMBER_WITH_DECIMAL_FORMAT, width = 50, cellAlign = 1)]
        [ExcelExport(subHeaderText = "PRICE", order = 4, currencyFormat = ERPConstants.NUMBER_WITH_DECIMAL_FORMAT)]
        public decimal Price { get; set; }

        [NotMapped]
        [PDFExport(subHeaderText = "AMOUNT", order = 5, currencyFormat = ERPConstants.NUMBER_WITH_DECIMAL_FORMAT, width = 50, cellAlign = 1)]
        [ExcelExport(subHeaderText = "AMOUNT", order = 5, currencyFormat = ERPConstants.NUMBER_WITH_DECIMAL_FORMAT)]
        public decimal Amount { get; set; }
        [NotMapped]
        [PDFExport(subHeaderText = "REMARKS", order = 6, width = 50, cellAlign = 1)]
        [ExcelExport(subHeaderText = "REMARKS", order = 6)]
        public string Remark { get; set; }


    }
}
