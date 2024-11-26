namespace LiftManager.Core;

public interface IOperator : IDisposable
{
  Task<bool> LiftToFloor(int position);
  Task<bool> Stop();
}