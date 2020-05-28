using Dapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MooyongCommon.MyDB
{
    public interface IDapperFactory
    {
        DapperClient CreateClient(string name);
    }
}
