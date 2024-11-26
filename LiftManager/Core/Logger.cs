namespace LiftManager.Core;

public class Logger(Microsoft.Extensions.Logging.ILogger logger) : ILogger
{
  public void LogDebug(string message) => logger.LogDebug(message);

  public void LogError(Exception ex, string message) => logger.LogError(ex, message);

  public void LogInformation(string message) => logger.LogInformation(message);

  public void LogTrace(string message) => logger.LogTrace(message);
}
