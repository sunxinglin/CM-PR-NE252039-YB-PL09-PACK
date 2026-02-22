using System;
using MPAssmebleRecipe.Logger.Interfaces;
using NLog;

public class NLogHelper : ILoggerHelper
{
    private readonly ILogger _logger;

    public NLogHelper(string loggerName = null)
    {
        _logger = loggerName == null ? LogManager.GetCurrentClassLogger() 
                                   : LogManager.GetLogger(loggerName);
    }

    public void Debug(string message)
    {
        _logger.Debug(message);
    }

    public void Info(string message)
    {
        _logger.Info(message);
    }

    public void Warn(string message)
    {
        _logger.Warn(message);
    }

    public void Error(string message)
    {
        _logger.Error(message);
    }

    public void Error(Exception ex, string message)
    {
        _logger.Error(ex, message);
    }

    public void Fatal(Exception ex, string message)
    {
        _logger.Fatal(ex, message);
    }
} 