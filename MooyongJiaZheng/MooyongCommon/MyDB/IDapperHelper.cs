using DapperExtensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace MooyongCommon.MyDB
{
    public interface IDapperHelper
    {
        Database GetConnection();
        T Get<T>(string id, IDbTransaction tran = null, int? commandTimeout = null) where T : class;
        IEnumerable<T> GetAll<T>(object predicate = null, IList<ISort> sort = null, IDbTransaction tran = null, int? commandTimeout = null, bool buffered = true) where T : class;
        IEnumerable<T> GetPage<T>(object predicate, IList<ISort> sort, int page, int pagesize, IDbTransaction tran = null, int? commandTimeout = null, bool buffered = true) where T : class;
        dynamic Insert<T>(T obj, IDbTransaction tran = null, int? commandTimeout = null) where T : class;
        void Insert<T>(IEnumerable<T> list, IDbTransaction tran = null, int? commandTimeout = null) where T : class;
        bool Update<T>(T obj, IDbTransaction tran = null, int? commandTimeout = null, bool ignoreAllKeyProperties = false) where T : class;
        bool Update<T>(IEnumerable<T> list, IDbTransaction tran = null, int? commandTimeout = null, bool ignoreAllKeyProperties = false) where T : class;
        bool Delete<T>(T obj, IDbTransaction tran = null, int? commandTimeout = null) where T : class;
        bool Delete<T>(IEnumerable<T> list, IDbTransaction tran = null, int? commandTimeout = null) where T : class;
        IDbTransaction TranStart();
        void TranRollBack(IDbTransaction tran);
        void TranCommit(IDbTransaction tran);
        List<T> Query<T>(string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null);
        int Execute<T>(string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null);
    }
}