namespace LiftManager.Core;

public class Logger : ILogger
{
  private readonly Serilog.ILogger _log;

  public Logger(Serilog.ILogger log)
  {
    _log = log;
  }

  public void LogDebug(string message) => _log.Debug(message);

  public void LogError(Exception ex, string message) => _log.Error(ex, message);

  public void LogInformation(string message) => _log.Information(message);

  public void LogTrace(string message) => _log.Debug(message);
}
