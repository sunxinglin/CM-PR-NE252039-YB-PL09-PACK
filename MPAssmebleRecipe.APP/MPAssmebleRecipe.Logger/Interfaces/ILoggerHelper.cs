using System;

namespace MPAssmebleRecipe.Logger.Interfaces
{
    /// <summary>
    /// 日志帮助接口
    /// </summary>
    public interface ILoggerHelper
    {
        void Debug(string message);
        void Info(string message);
        void Warn(string message);
        void Error(string message);
        void Error(Exception ex, string message);
        void Fatal(Exception ex, string message);
    }
} 