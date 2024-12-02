namespace LiftManager.Domain;

public interface IRepository : IDisposable
{
  Task<bool> SaveLiftPosition(Data.LiftPosition liftPosition);
  Task<Data.LiftPosition> GetCurrentLiftPosition();
}