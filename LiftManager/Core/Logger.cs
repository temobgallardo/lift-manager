namespace LiftManager.Core;

public class Logger : ILogger
{
  private readonly Serilog.ILogger _log;

  public Logger(Serilog.ILogger log)
  {
    _log = log;
  }

  public void Debug(string message) => _log.Debug(message);

  public void Error(Exception ex, string message) => _log.Error(ex, message);

  public void Information(string message) => _log.Information(message);
  public void Information(string message, object o) => _log.Information(message, o);
}
