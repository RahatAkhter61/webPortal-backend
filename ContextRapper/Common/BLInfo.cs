using ContextMapper.Common;
using CORE_BASE.CORE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextMapper
{
   
        public class BLInfo<T> where T : class
        {
            Common.MapperDataContext dc = new MapperDataContext(CoreConnection.ConnectionString);

            public BLInfo()
            {
              
            }

            public IQueryable<T> GetQuerable<T>() where T : class,new()
            {
                return dc.GetTable<T>().AsQueryable();
            }

        }
    
}
