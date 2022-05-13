using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERPService.DataModel.DTO;

namespace ERPService.DataModel.CTO
{
    public class ActivityLogVM
    {
        public string LogType { get; set; }
        //public Guid ActivityType { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int PageFrom { get; set; }
        public int PageTo { get; set; }
    }

    public class ActivityLogRes
    {
        public ActivityLogRes()
        {
            Activity = new List<ActivityLog>();
        }

        public List<ActivityLog> Activity { get; set; }
        public int RowCount { get; set; }
    }

    public class ExceptionLogRes
    {
        public ExceptionLogRes()
        {
            Exception = new List<ExceptionLog>();
        }

        public List<ExceptionLog> Exception { get; set; }
        public int RowCount { get; set; }
    }
}
