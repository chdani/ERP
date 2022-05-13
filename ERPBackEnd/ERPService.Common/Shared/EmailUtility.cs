using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ERPService.Common.Shared
{
    public static class EmailUtility
    {

        public static AppResponse SendEmail(MailMessage message)
        {
            try
            {
                var femail = ERPSettings.APPSYSTEMSETTINGS[APPSystemsettingsKey.SENDEREMAILID];
                var password = ERPSettings.APPSYSTEMSETTINGS[APPSystemsettingsKey.SENDEREMAILPWD];

                SmtpClient smtpClient = new SmtpClient(ERPSettings.APPSYSTEMSETTINGS[APPSystemsettingsKey.EMAILSERVERURL], int.Parse(ERPSettings.APPSYSTEMSETTINGS[APPSystemsettingsKey.EMAILSERVERPORT]));
                smtpClient.Credentials = new NetworkCredential(femail, password);
                if (ERPSettings.APPSYSTEMSETTINGS[APPSystemsettingsKey.ENABLESSLFOREMAIL] == "1")
                    smtpClient.EnableSsl = true;

                smtpClient.Send(message);
                return new AppResponse() { Status = APPMessageKey.DATASAVESUCSS }; ;
            }
            catch (Exception ex)
            {
                var repsonse = new AppResponse() { Status = APPMessageKey.ONEORMOREERR, Messages = new List<string>() };
                repsonse.Messages.Add(ex.Message);
                return repsonse;
            }
        }
    }
}
