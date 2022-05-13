using ERPService.Common.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPService.DataModel.DTO
{
    public class CodesMaster : BaseEntity
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string CodeType { get; set; }
        [NotMapped]
        public ICollection<CodesDetails> CodesDetail { get; set; }

    }
}
