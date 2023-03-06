using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CORE_BASE.CORE
{
    public class dbEngine
    {
        public readonly CORE_MODELS.DbEngineDataContext _dbEngineContext;
        public readonly CORE_MODELS.CoreDataContext _dbContext;
        public readonly CORE_MODELS.WPSDataContext _WPSContext;
        public dbEngine()
        {
            _dbEngineContext = new CORE_MODELS.DbEngineDataContext(CoreEngineConnection.ConnectionString);
            _dbContext = new CORE_MODELS.CoreDataContext(CoreConnection.ConnectionString);
            _WPSContext = new CORE_MODELS.WPSDataContext(WPSConnection.ConnectionString);
        }
    }
}
