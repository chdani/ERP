using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ERPService.Common
{
    public static class WindowsAuth
    {
        public static bool ValidateCredentials(string username, string password, string domain)
        {
            NetworkCredential credentials
              = new NetworkCredential(username, password, domain);

            LdapDirectoryIdentifier id = new LdapDirectoryIdentifier(domain);

            using (LdapConnection connection = new LdapConnection(id, credentials, AuthType.Kerberos))
            {
                connection.SessionOptions.Sealing = true;
                connection.SessionOptions.Signing = true;

                try
                {
                    //If bind is successfull user is validated
                    connection.Bind();
                }
                catch (LdapException ex)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool CheckIfUserActive(string username, string domain)
        {
            try
            {
                using (PrincipalContext ctx = new PrincipalContext(ContextType.Domain))
                {
                    // find a user
                    UserPrincipal user = UserPrincipal.FindByIdentity(ctx, username);
                    bool? isActive = false;
                    if (user != null)
                        isActive = user.Enabled;

                    if (isActive != null && isActive == true)
                        return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
