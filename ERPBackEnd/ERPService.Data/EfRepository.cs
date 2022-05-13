using ERPService.Common.Shared;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPService.Data
{
    public class EfRepository : Repository
    {
        public EfRepository(AppDbContext dbContext, IOwinContext context) : base(dbContext, context)
        {
        }
    }
}
