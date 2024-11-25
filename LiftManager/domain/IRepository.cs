namespace LiftManager.Domain;

public interface IRepository : IDisposable
{
  // TODO: Make a method to store new LiftPosition record (history)
  // TODO: Make or improve GetLiftPosition to Get latest added record
  Task<bool> SaveLiftPosition(LiftPosition liftPosition);
  Task<LiftPosition> GetLiftPosition();
}