using ERPService.Common;
using ERPService.Common.Interfaces;
using ERPService.Common.Shared;
using ERPService.DataModel.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace ERPService.BC.Utility
{
    public static class AppMessagesBC
    {
        public static void GetAppMessages(IRepository _repository, UserContext _userContext)
        {
            ERPExceptions.APP_MESSAGES = new Dictionary<string, string>();
            ObjectCache cache = MemoryCache.Default;
            var lang = "en";
            if(_userContext != null)
                lang = _userContext.Language;

            var cacheKey = ERPCacheKey.APPMESSAGE + lang;
            if (cache.Contains(cacheKey))
                ERPExceptions.APP_MESSAGES = (Dictionary<string, string>)cache.Get(cacheKey);
            else
            {
                var langMastr = _repository.GetQuery<LangMaster>()
                        .Where(a => a.Language == lang && a.CodeType == ERPCacheKey.APPMESSAGE && a.Active == "Y").ToList();

                var messages = _repository.GetQuery<AppMessage>().Where(a => a.Active == "Y");

                foreach (var msg in messages)
                {
                    if (!ERPExceptions.APP_MESSAGES.ContainsKey(msg.Code))
                    {
                        var langMessage = langMastr.FirstOrDefault(a => a.Code == msg.Code);
                        if (langMessage != null)
                            ERPExceptions.APP_MESSAGES.Add(msg.Code, langMessage.Description);
                        else
                            ERPExceptions.APP_MESSAGES.Add(msg.Code, msg.Description);
                    }
                }
                cache.Add(cacheKey, ERPExceptions.APP_MESSAGES, DateTime.Now.AddDays(2));
            }
        }
    }
}
