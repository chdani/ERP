using ERPService.Common.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPService.DataModel.DTO
{
    public class CurrencyMaster : BaseEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
    }
}
