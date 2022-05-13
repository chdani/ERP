using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPService.Common.Shared
{
    public class AppResponse
    {
        public string Status { get; set; }

        public Guid ReferenceId { get; set; }
        public List<string> Messages { get; set; }
    }
}
