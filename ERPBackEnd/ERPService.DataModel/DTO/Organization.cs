using ERPService.Common.Shared;

namespace ERPService.DataModel.DTO
{
    public class Organization : BaseEntity
    {
        public string OrgCode { get; set; }
        public string OrgName { get; set; }
        public string Location { get; set; }
        public bool DefaultOrg { get; set; }
    }
}
