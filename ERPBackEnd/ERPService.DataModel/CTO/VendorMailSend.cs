using ERPService.Common.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPService.DataModel.CTO
{
    public class VendorMailSend : BaseEntity
    {
        public string QuotionReqNo { get; set; }
        public string purchaseTransNo { get; set; }
        public DateTime QuotationReqDate { get; set; }
        public string Remarks { get; set; }
        public List<VendorMailDet> VendorMailDets { get; set; } = new List<VendorMailDet>();
        public string ExportHeaderText { get; set; }
        public decimal Amount { get; set; }
        public decimal Quantity { get; set; }

    }
    public class VendorMailDet
    {
        public string Name { get; set; }
        public string ToMail { get; set; }
        public List<string> CCMail { get; set; } = new List<string>();
        public string VendorName { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

    }

}
