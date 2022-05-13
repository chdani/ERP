using ERPService.Common.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPService.DataModel.DTO
{
    public class Employee : BaseEntity
    {
        public string EmpNumber { get; set; }
        public string FullNameEng { get; set; }
        public string FullNameArb { get; set; }
        public string QatariID { get; set; }
        public string Passport { get; set; }
        public DateTime? DOB { get; set; }
        public string PlaceOfBirth { get; set; }
        public string Nationality { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string MaritalStatusCode { get; set; }
        public Guid CurrDepartmentId { get; set; }
        public Guid CurrPositionId { get; set; }
        public Guid ManagerId { get; set; }
        public int CurrentGrade { get; set; }
        public bool IsDepratmentHead { get; set; }

        [NotMapped]
        public List<AppDocument> AppDocuments { get; set; }
        [NotMapped]
        public string Others { get; set; }
        [NotMapped]
        public Boolean CreateUser { get; set; }
        [NotMapped]
        public DateTime? FromDobDate { get; set; }
        [NotMapped]
        public DateTime? ToDobDate { get; set; }
        [NotMapped]
        public List<EmpEducation> EmpEducation { get; set; }

        [NotMapped]
        public List<EmpDependent> EmpDependent { get; set; }


    }
}
