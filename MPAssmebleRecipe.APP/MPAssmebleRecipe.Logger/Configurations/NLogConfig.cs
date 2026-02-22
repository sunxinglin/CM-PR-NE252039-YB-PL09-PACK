using NLog;

namespace MPAssmebleRecipe.Logger.Configurations
{
    public static class NLogConfig
    {
        public static void Configure(string configPath = "nlog.config")
        {
            LogManager.LoadConfiguration(configPath);
        }
    }
} 