namespace LiftManager.Core;

public interface IOperator : IDisposable
{
  Task<bool> LiftToFloor(int floor);
  Task<bool> RequestLiftToFloor(int floor);
  Task<bool> Stop();
}