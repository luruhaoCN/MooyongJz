using DapperExtensions;
using DapperExtensions.Mapper;
using DapperExtensions.Sql;
using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;

namespace MooyongCommon.MyDB
{
    /// <summary>
    /// 数据库连接辅助类
    /// </summary>
    public class ConnectionFactory
    {
        /// <summary>
        /// 转换数据库类型
        /// </summary>
        /// <param name="databaseType">数据库类型</param>
        /// <returns></returns>
        public static DatabaseType GetDataBaseType(string databaseType)
        {
            DatabaseType returnValue = DatabaseType.SqlServer;
            foreach (DatabaseType dbType in Enum.GetValues(typeof(DatabaseType)))
            {
                if (dbType.ToString().Equals(databaseType, StringComparison.OrdinalIgnoreCase))
                {
                    returnValue = dbType;
                    break;
                }
            }
            return returnValue;
        }

        /// <summary>
        /// 获取数据库连接
        /// </summary>
        /// <returns></returns>
        public static Database CreateConnection(string strConn, DatabaseType databaseType = DatabaseType.Oracle)
        {
            Database connection = null;
            //获取配置进行转换
            switch (databaseType)
            {
                case DatabaseType.SqlServer:
                    var sqlConn = new SqlConnection(strConn);
                    var sqlconfig = new DapperExtensionsConfiguration(typeof(AutoClassMapper<>), new List<Assembly>(), new SqlServerDialect());
                    var sqlGenerator = new SqlGeneratorImpl(sqlconfig);
                    connection = new Database(sqlConn, sqlGenerator);
                    break;
                case DatabaseType.MySql:
                    var mysqlConn = new MySqlConnection(strConn);
                    var mysqlconfig = new DapperExtensionsConfiguration(typeof(AutoClassMapper<>), new List<Assembly>(), new MySqlDialect());
                    var mysqlGenerator = new SqlGeneratorImpl(mysqlconfig);
                    connection = new Database(mysqlConn, mysqlGenerator);
                    break;
                case DatabaseType.Sqlite:
                    var sqlliteConn = new SqliteConnection(strConn);
                    var sqlliteconfig = new DapperExtensionsConfiguration(typeof(AutoClassMapper<>), new List<Assembly>(), new SqliteDialect());
                    var sqlliteGenerator = new SqlGeneratorImpl(sqlliteconfig);
                    connection = new Database(sqlliteConn, sqlliteGenerator);
                    break;
                case DatabaseType.Oracle:
                    var orcalConn = new OracleConnection(strConn);
                    var orcaleconfig = new DapperExtensionsConfiguration(typeof(AutoClassMapper<>), new List<Assembly>(), new OracleDialect());
                    var orcaleGenerator = new SqlGeneratorImpl(orcaleconfig);
                    connection = new Database(orcalConn, orcaleGenerator);
                    break;
            }
            return connection;
        }
    }
}
