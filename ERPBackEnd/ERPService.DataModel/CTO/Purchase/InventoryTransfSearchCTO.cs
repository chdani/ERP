using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPService.DataModel.CTO 
{
    public class InventoryTransfSearchCTO
    {
        public string TransNo { get; set; }
        public DateTime FromTransDate { get; set; }
        public DateTime ToTransDate { get; set; }
        public Guid ToWareHouseLocationId { get; set; }
        public Guid FromWareHouseLocationId { get; set; }
        public string Status { get; set; }
        public string ExportType { get; set; }
        public string ExportHeaderText { get; set; }
    }
     
}
