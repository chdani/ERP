using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPService.Common.Shared
{
    public class UserContext  
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Roles { get; set; }
        public string Token { get; set; }
        public string EmailId { get; set; }
        public Guid EmployeeId { get; set; }
        public string UserType { get; set; }
        public string Language { get; set; }
        public List<UserAccount> LedgerAccounts { get; set; }
        public List<UserOrganization> Organizations { get; set; }       
        public void CopyLocal(UserContext fromcontext)
        {
            if (fromcontext == null)
                return;
            Id = fromcontext.Id;
            UserName = fromcontext.UserName;
            FirstName = fromcontext.FirstName;
            EmailId = fromcontext.EmailId;
            LastName = fromcontext.LastName;
            UserType = fromcontext.UserType;
            Roles = fromcontext.Roles;
            Token = fromcontext.Token;
            Language = fromcontext.Language;
            LedgerAccounts = fromcontext.LedgerAccounts;
            Organizations = fromcontext.Organizations;
        }
    }
    public class UserAccount
    {
        public Guid Id { get; set; }
        public int LedgerCode { get; set; }
        public string LedgerDesc { get; set; }
        public string UsedFor { get; set; }
        public string Remarks { get; set; }


    }
    public class UserOrganization
    {
        public Guid Id { get; set; }
        public string OrgCode { get; set; }
        public string OrgName { get; set; }
        public string Location { get; set; }
    }
}
