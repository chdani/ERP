
using ERPService.Common.Shared;
using System;

namespace ERPService.DataModel.DTO
{
    public class WareHouseLocation : BaseEntity
    {

        public Guid WarehouseId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string ContactNo { get; set; }
    }
}

