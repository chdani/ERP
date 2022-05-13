using ERPService.Common;
using ERPService.Common.Shared;
using ERPService.DataModel.CTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPService.DataModel.DTO
{
    public class ProdInvIssue : BaseEntity
    {
        [PDFExport(headerText = "TRANSNO", order = 1, width = 50, cellAlign = 1)]
        [ExcelExport(headerText = "TRANSNO", order = 1)]
        public string TransNo { get; set; }
        public long SeqNo { get; set; }
        [PDFExport(headerText = "TRANSDATE", order = 2, width = 50, dateFormat = "dd-MM-yyyy", cellAlign = 1)]
        [ExcelExport(headerText = "TRANSDATE", order = 2, dateFormat = "dd-MM-yyyy")]
        public DateTime TransDate { get; set; }

        public Guid ServiceRequestId { get; set; }

        public Guid EmployeeId { get; set; }

        public string Type { get; set; }
        public string Status { get; set; }
        [AuditableAttribute("APP_REMARKS")]
        [PDFExport(headerText = "REMARKS", order = 5, width = 50, cellAlign = 1)]
        [ExcelExport(headerText = "REMARKS", order = 5)]
        public string Remarks { get; set; }
        [NotMapped]
        public string ApproverRemarks { get; set; }
        [NotMapped]
        [PDFExport(headerText = "EMPLOYEE", order = 3, width = 50, cellAlign = 1)]
        [ExcelExport(headerText = "EMPLOYEE", order = 3)]
        public string EmployeeName { get; set; }
        [NotMapped]
        [PDFExport(headerText = "SERVICEREQNO", order = 4, width = 50, cellAlign = 1)]
        [ExcelExport(headerText = "SERVICEREQNO", order = 4)]
        public string ServiceReqNo { get; set; }
        [NotMapped]
        public List<ProdInvIssueDet> ProdInvIssueDet { get; set; }

        [NotMapped]
        [PDFExport(subHeaderText = "WAREHOUSE", order = 1, width = 50, cellAlign = 1)]
        [ExcelExport(subHeaderText = "WAREHOUSE", order = 1)]
        public string WareHouse { get; set; }
        [NotMapped]
        [PDFExport(subHeaderText = "PRODUCTMASTER", order = 2, width = 50, cellAlign = 1)]
        [ExcelExport(subHeaderText = "PRODUCTMASTER", order = 2)]
        public string ProductName { get; set; }
        [NotMapped]
        [PDFExport(subHeaderText = "UNITMASTER", order = 3, width = 50, cellAlign = 1)]
        [ExcelExport(subHeaderText = "UNITMASTER", order = 3)]
        public string UnitName { get; set; }

        [NotMapped]
        [PDFExport(subHeaderText = "QUANTITY", order = 4, currencyFormat = ERPConstants.NUMBER_WITH_DECIMAL_FORMAT, width = 50, cellAlign = 1)]
        [ExcelExport(subHeaderText = "QUANTITY", order = 4, currencyFormat = ERPConstants.NUMBER_WITH_DECIMAL_FORMAT)]
        public decimal Quantity { get; set; }
        [NotMapped]
        [PDFExport(subHeaderText = "REMARKS", order = 5, width = 50, cellAlign = 1)]
        [ExcelExport(subHeaderText = "REMARKS", order = 5)]
        public string Remark { get; set; }

    }
}
