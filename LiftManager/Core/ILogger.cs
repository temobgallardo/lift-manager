namespace LiftManager.Core;

public interface ILogger
{
  void LogInformation(string message);
  void LogTrace(string message);
  void LogError(Exception ex, string message);
  void LogDebug(string message);
}