using ERPService.Common.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPService.DataModel.DTO
{
    public class Department : BaseEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public Guid ParentId { get; set; }
        public string Type { get; set; }

        [NotMapped]
        public List<Department> departments { get; set; }
        
       
    }
}