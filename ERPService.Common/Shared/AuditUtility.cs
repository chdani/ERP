using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ERPService.Common.Shared
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class AuditableAttribute : Attribute
    {
        private string _dispname;

        public virtual string DispName
        {
            get { return _dispname; }
        }

        private string _format;

        public virtual string Format
        {
            get { return _format; }
        }

        public AuditableAttribute(string dispName, string format = "")
        {
            _dispname = dispName;
            _format = format;
        }

    }
    public static class AuditUtility
    {
        public static List<AppAudit> GetAuditableObject<T>(T oldEnt, T newEnt)
        {

            AuditableAttribute att;
            if (oldEnt == null || newEnt == null)
                return null;
            List<AppAudit> retObj = new List<AppAudit>();

            foreach (PropertyInfo propObj1 in oldEnt.GetType().GetProperties())
            {
                att = (AuditableAttribute)Attribute.GetCustomAttribute(propObj1, typeof(AuditableAttribute));

                if (att == null)
                    continue;
                var propObj2 = newEnt.GetType().GetProperty(propObj1.Name);
                string oldvalue = propObj1.GetValue(oldEnt, null) == null ? "" : propObj1.GetValue(oldEnt, null).ToString();
                string newvalue = propObj2.GetValue(newEnt, null) == null ? "" : propObj2.GetValue(newEnt, null).ToString();

                if (!string.IsNullOrEmpty(att.Format))
                {
                    try
                    {
                        if (propObj1.PropertyType == typeof(DateTime))
                        {
                            oldvalue = propObj1 == null ?  ""  : ((DateTime)propObj1?.GetValue(oldEnt, null)).ToString(att.Format);
                            newvalue = propObj2 == null ? "" : ((DateTime)propObj2.GetValue(newEnt, null)).ToString(att.Format);
                        }
                        else if (propObj1.PropertyType == typeof(decimal))
                        {
                            oldvalue = propObj1 == null ? "" : ((decimal)propObj1?.GetValue(oldEnt, null)).ToString(att.Format);
                            newvalue = propObj2 == null ? "" : ((decimal)propObj2.GetValue(newEnt, null)).ToString(att.Format);
                        }
                        else if (propObj1.PropertyType == typeof(float))
                        {
                            oldvalue = propObj1 == null ? "" : ((float)propObj1?.GetValue(oldEnt, null)).ToString(att.Format);
                            newvalue = propObj2 == null ? "" : ((float)propObj2.GetValue(newEnt, null)).ToString(att.Format);
                        }
                        else if (propObj1.PropertyType == typeof(long) || propObj1.PropertyType == typeof(int))
                        {
                            oldvalue = propObj1 == null ? "" : ((long)propObj1?.GetValue(oldEnt, null)).ToString(att.Format);
                            newvalue = propObj2 == null ? "" : ((long)propObj2.GetValue(newEnt, null)).ToString(att.Format);
                        }
                    }
                    catch
                    {
                        oldvalue = propObj1.GetValue(oldEnt, null) == null ? "" : propObj1.GetValue(oldEnt, null).ToString();
                        newvalue = propObj2.GetValue(newEnt, null) == null ? "" : propObj2.GetValue(newEnt, null).ToString();
                    }
                }
                    
                if(oldvalue != newvalue)
                    retObj.Add(new AppAudit() { FieldName = att.DispName, OldValue = oldvalue  == null ? "" : oldvalue.ToString() , NewValue = newvalue == null?"" : newvalue.ToString() });
            }
            return retObj;
        }

    }
    public class AppAudit
    {
        public string FieldName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
    }
}