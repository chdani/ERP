using ERPService.Common.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPService.DataModel.DTO
{
    public class EmpEducation : BaseEntity
    {
        public Guid EmployeeId { get; set; }
        public string EduLevelCode { get; set;}
        public string EstablishmentCode { get; set;}
        public string Specialization { get; set; }
        public string CompletedYear { get; set; }
        public string GradePercentage { get; set; }
        public string Remarks { get; set; }


    }
}
