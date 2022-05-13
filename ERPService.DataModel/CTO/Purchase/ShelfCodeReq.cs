using ERPService.Common.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPService.DataModel.CTO
{
    public class ShelfCodeReq
    {
    public Guid fromWarehouseLocationId { get; set; }
    public Guid productMasterId { get; set; }



    }
}
