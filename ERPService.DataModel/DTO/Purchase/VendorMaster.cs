using ERPService.Common.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPService.DataModel.DTO
{
    public class VendorMaster : BaseEntity
    {
        public string Title { get; set; }
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string CountryName { get; set; }
        public string POBox { get; set; }
        public string Telephone { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string BankCountryCode { get; set; }
        public string BankCode { get; set; }
        public string IbanSwifT { get; set; }
        public string BankAccName { get; set; }
        public string BankAccNo { get; set; }
        public int LedgerCode { get; set; }
        [NotMapped]
        public string Others { get; set; }
        [NotMapped]
        public List<VendorProduct> VendorProducts { get; set; }
        [NotMapped]
        public List<VendorContact> VendorContacts { get; set; }
        [NotMapped]
        public List<VendorContract> VendorContracts { get; set; }
    }
}
