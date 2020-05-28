using log4net;
using log4net.Config;
using log4net.Repository;
using System.IO;

namespace MooyongCommon
{
    public static class ApiLogger
    {
        private const string repositoryName = "NETCoreRepository";
        private const string configFile = "log4net.config";
        private static ILoggerRepository repository { get; set; }

        private static readonly ILog _loginfo = LogManager.GetLogger(repositoryName, "loginfo");
        private static readonly ILog _logerror = LogManager.GetLogger(repositoryName, "logerror");
        private static readonly ILog _logwarn = LogManager.GetLogger(repositoryName, "logwarn");
        private static readonly ILog _logfatal = LogManager.GetLogger(repositoryName, "logfatal");

        public static void Configure()
        {
            repository = LogManager.CreateRepository(repositoryName);
            XmlConfigurator.Configure(repository, new FileInfo(configFile));
        }

        public static void Info(string msg)
        {
            _loginfo.Info(msg);
        }

        public static void Warn(string msg)
        {
            _logwarn.Warn(msg);
        }

        public static void Error(string msg)
        {
            _logerror.Error(msg);
        }

        public static void Fatal(string msg)
        {
            _logfatal.Fatal(msg);
        }
    }
}
