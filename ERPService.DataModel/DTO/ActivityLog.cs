using ERPService.Common.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPService.DataModel.DTO
{
    public class ActivityLog : BaseEntity
    {
        public Guid UserId { get; set; }
        public string Host { get; set; }
        public string Headers { get; set; }
        public string RequestBody { get; set; }
        public string RequestMethod { get; set; }
        public string UserHostAddress { get; set; }
        public string UserAgent { get; set; }
        public string AbsoluteURI { get; set; }
        public DateTime RequestedOn { get; set; }

        [NotMapped]
        public string MethodName { get; set; }

        [NotMapped]
        public string FirstName { get; set; }

        [NotMapped]
        public string LastName { get; set; }

        //public Guid Id { get; set; }
        //public Guid ActivityType { get; set; }
        //public string RequestURL { get; set; }
        //public string ControllerName { get; set; }
        //public string ActionName { get; set; }
        //public string Parameters { get; set; }
    }
}
