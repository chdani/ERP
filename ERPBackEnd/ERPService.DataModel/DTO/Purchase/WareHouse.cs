
using ERPService.Common.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPService.DataModel.DTO
{
    public class WareHouse : BaseEntity
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string ContactNo { get; set; }

        [NotMapped]
        public List<WareHouseLocation> WareHouseLocation { get; set; }
    }
}

