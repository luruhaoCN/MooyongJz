using System;
using System.Collections.Generic;
using System.Text;

namespace MooyongCommon.MyDB
{
    public class DataBaseConfig
    {
        public string ConnectionString { get; set; }
        public DbStoreType DbType { get; set; }
    }
    public enum DbStoreType
    {
        MySql = 0,
        SqlServer = 1,
        Sqlite = 2,
        Oracle = 3
    }

    public class DapperFactoryOptions
    {
        public IList<Action<DataBaseConfig>> DapperActions { get; } = new List<Action<DataBaseConfig>>();
    }
}
