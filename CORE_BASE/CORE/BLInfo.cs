using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using OSM_Model;
using System.Data;
using System.Data.Linq;
using System.Linq.Expressions;
using System.Base.General;
using System.Reflection;

namespace System.Base.Base
{
    public class BLInfo<T> where T : class
    {
        DataContextDataContext dc = new DataContextDataContext(OSMConnection.ConnectionString);

        public BLInfo()
        {
            this.SecurityAccess(Assembly.GetCallingAssembly());
        }

        public IQueryable<T> GetQuerable<T>() where T : class,new()
        {            
            return dc.GetTable<T>().AsQueryable();
        }

    }
}
