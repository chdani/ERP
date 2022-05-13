using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPService.DataModel 
{
    public class ApprovalAccessDetails
    {

        public Guid EmployeeId { get; set; }
        public string  UserName              {get;set;}
       public string  AllowAdd              {get;set;}
       public string  AllowApprove          {get;set;}
       public string  AllowEdit             {get;set;}
       public string  AllowDelete           {get;set;}
       public string  Email                 {get;set;}
       public string PhoneNumber { get; set; }
    }
}
