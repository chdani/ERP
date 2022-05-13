using ERPService.Common.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPService.DataModel.DTO
{
    public class UserLedgerAccnt : BaseEntity
    {
        public Guid UserID { get; set; }
        public int AccountCode { get; set; }       
        
    }
}

