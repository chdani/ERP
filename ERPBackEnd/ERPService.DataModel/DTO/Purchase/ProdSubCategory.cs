
using ERPService.Common.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPService.DataModel.DTO
{
    public class ProdSubCategory : BaseEntity
    {
        public Guid ProdCategoryId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
       
    }
}

