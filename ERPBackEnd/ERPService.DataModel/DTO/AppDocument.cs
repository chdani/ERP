using ERPService.Common.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPService.DataModel.DTO
{
    public class AppDocument : BaseEntity
    {
      
        public Guid TransactionId { get; set; }
        public string TransactionType { get; set; }
        public string FileName { get; set; }
        public string DocumentType { get; set; }
        public string UniqueNumber { get; set; }
       
        public DateTime? ExpiryDate { get; set; }
        public byte[] FileContent { get; set; }
        [NotMapped]
        public string UserName { get; set; }

    }
}




