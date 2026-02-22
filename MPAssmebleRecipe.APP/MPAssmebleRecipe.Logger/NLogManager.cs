using MPAssmebleRecipe.Logger.Interfaces;
using NLog;

namespace MPAssmebleRecipe.Logger
{
    /// <summary>
    /// NLog管理器
    /// </summary>
    public static class NLogManager
    {
        private static ILoggerHelper _instance;
        private static bool _isConfigured;

        /// <summary>
        /// 获取日志实例
        /// </summary>
        public static ILoggerHelper GetLogger()
        {
            if (_instance == null)
            {
                _instance = new NLogHelper();
            }
            return _instance;
        }

        /// <summary>
        /// 获取指定名称的日志实例
        /// </summary>
        public static ILoggerHelper GetLogger(string loggerName)
        {
            return new NLogHelper(loggerName);
        }

        /// <summary>
        /// 配置NLog
        /// </summary>
        /// <param name="configPath">配置文件路径，默认为nlog.config</param>
        public static void Configure(string configPath = "nlog.config")
        {
            if (!_isConfigured)
            {
                LogManager.LoadConfiguration(configPath);
                _isConfigured = true;
            }
        }

        /// <summary>
        /// 关闭NLog
        /// </summary>
        public static void Shutdown()
        {
            LogManager.Shutdown();
        }
    }
} 