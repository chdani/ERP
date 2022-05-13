using ERPService.Common;
using ERPService.Common.Shared;
using ERPService.DataModel.CTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPService.DataModel.DTO
{
    public class ServiceRequest : BaseEntity
    {
        [PDFExport(headerText = "TRANSNO", order = 1, width = 50, cellAlign = 1)]
        [ExcelExport(headerText = "TRANSNO", order = 1)]
        public string TransNo { get; set; }
        public long SeqNo { get; set; }
        [PDFExport(headerText = "TRANSDATE", order = 2, width = 50, dateFormat = "dd-MM-yyyy", cellAlign = 1)]
        [ExcelExport(headerText = "TRANSDATE", order = 2)]
        public DateTime TransDate { get; set; }

        public Guid EmployeeId { get; set; }

        [AuditableAttribute("SR_CATEGORY_NAME")]
        public Guid ProdCategoryId { get; set; }
        [AuditableAttribute("SR_PROD_NAME")]
        public Guid ProductMasterId { get; set; }
        public Guid ProdSubCategoryId { get; set; }
        public decimal RequiredQty { get; set; }

        [AuditableAttribute("SR_UNIT_NAME")]
        public Guid UnitMasterId { get; set; }
        public string ProdConfiguration { get; set; }

        [AuditableAttribute("SR_QUANTITY", ERPConstants.NUMBER_FORMAT)]

        public decimal Quantity { get; set; }

        [AuditableAttribute("APP_REMARKS")]
        [PDFExport(headerText = "REMARKS", order = 5, width = 50, cellAlign = 1)]
        [ExcelExport(headerText = "REMARKS", order = 5)]
        public string Remarks { get; set; }
        public int CurApprovalLevel { get; set; }
        public int NextApprovalLevel { get; set; }
        [NotMapped]
        [PDFExport(headerText = "STATUS", order = 4, width = 50, cellAlign = 1)]
        [ExcelExport(headerText = "STATUS", order = 4)]
        public string StatusText { get; set; }
        [NotMapped]
        [PDFExport(headerText = "EMPLOYEE", order = 3, width = 50, cellAlign = 1)]
        [ExcelExport(headerText = "EMPLOYEE", order = 3)]
        public string EmployeeName { get; set; }
        [NotMapped]
        public string ApproverRemarks { get; set; }

        [NotMapped]
        public string StatusCode { get; set; }
        [NotMapped]
        public bool CanApproveReject { get; set; }
        [NotMapped]
        public List<ServiceReqApproval> ServReqApproval { get; set; }


        [NotMapped]
        [PDFExport(subHeaderText = "PRODUCT", order = 1, width = 50, cellAlign = 1)]
        [ExcelExport(subHeaderText = "PRODUCT", order = 1)]
        public string ProdCategoryName { get; set; }

        [NotMapped]
        [PDFExport(subHeaderText = "PRODUCTMASTER", order = 3, width = 50, cellAlign = 1)]
        [ExcelExport(subHeaderText = "PRODUCTMASTER", order = 3)]
        public String ProductName { get; set; }
        [NotMapped]
        [PDFExport(subHeaderText = "UNITMASTER", order = 4, width = 50, cellAlign = 1)]
        [ExcelExport(subHeaderText = "UNITMASTER", order = 4)]
        public String UnitName { get; set; }

        [NotMapped]
        [PDFExport(subHeaderText = "QUANTITY", order = 5, currencyFormat = ERPConstants.NUMBER_WITH_DECIMAL_FORMAT, width = 50, cellAlign = 1)]
        [ExcelExport(subHeaderText = "QUANTITY", order = 5, currencyFormat = ERPConstants.NUMBER_WITH_DECIMAL_FORMAT)]
        public decimal Quantitys { get; set; }
        [NotMapped]
        [PDFExport(subHeaderText = "SUBCATMASTERNAME", order = 2, width = 50, cellAlign = 1)]
        [ExcelExport(subHeaderText = "SUBCATMASTERNAME", order = 2)]
        public string ProdSubCategoryName { get; set; }

    }
}
