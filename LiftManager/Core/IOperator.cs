namespace LiftManager.Core;

public interface IOperator : IDisposable
{
  Task<bool> MoveToFloor(int position);
  Task<bool> Stop();
}