using ERPService.Common.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPService.DataModel.DTO
{
    public class EmbassyMaster : BaseEntity
    {
        public string CountryCode { get; set; }
        public string NameEng { get; set; }
        public string NameArabic { get; set; }
        public long Number { get; set; }
        public string DefaultCurrency { get; set; }
        public string Address { get; set; }
    }
}
