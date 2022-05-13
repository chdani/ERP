using ERPService.Common.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPService.DataModel.DTO
{
    public class EmpDependent : BaseEntity
    {
        public Guid EmployeeId { get; set; }
        public string FullName { get; set; }
        public string QatariID { get; set; }
        public string Passport { get; set; }
        public DateTime? DOB { get; set; }
        public string PlaceOfBirth { get; set; }
        public string RelationCode { get; set; }
    }
}