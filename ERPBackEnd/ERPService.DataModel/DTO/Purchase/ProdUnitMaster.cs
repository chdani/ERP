using ERPService.Common.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPService.DataModel.DTO
{
    public class ProdUnitMaster : BaseEntity
    { 
     
        public string UnitCode { get; set; }
        public string UnitName { get; set; }
        public decimal ConversionUnit { get; set; }


    }
}
