using ERPService.Common.Shared;
using ERPService.DataModel.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ERPService.DataModel.CTO
{
    public class EmployeeDetailes : BaseEntity
    {
        public List<Employee> employees { get; set; }
        public List<EmpEducation> Educations { get; set; }
        public List<EmpDependent> dependents { get; set; }
    }

   

   
}
