using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoundModel.DAL
{
    public class EFHelper : DbContext
    {
        public EFHelper(string connectionString)
                      : base(connectionString)
        {
        }
    }
}
