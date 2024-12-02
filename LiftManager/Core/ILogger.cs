namespace LiftManager.Core;

public interface ILogger
{
  void Debug(string message);
  void Error(Exception ex, string message);
  void Information(string message);
  void Information(string message, object o);
}