using ERPService.Common.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPService.DataModel.DTO
{
    public class ExceptionLog
    {
        public Guid Id { get; set; }
        public string ExceptionMessage { get; set; }
        public string InnerException { get; set; }
        public string StackTrace { get; set; }
        public DateTime ExceptionOccurredAt { get; set; }
    }
}
