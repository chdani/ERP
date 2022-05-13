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
    public class AppTranslation
    {
        public string CodeType { get; set; }
        public List<AppTranslationMaster> TranslationData { get; set; }
    }
    public class AppTranslationMaster : BaseEntity
    {

        public Guid  Id { get; set; }
        public string Code { get; set; }
        public string EnglishDescription { get; set; }
        public int EnglishDisplayOrder { get; set; }
        public string ArabicDescription { get; set; }
        public int ArabicDisplayOrder { get; set; }

    }
}
