namespace LiftManager.Domain;

public interface IRepository : IDisposable
{
  // TODO: Make a method to store new LiftPosition record (history)
  // TODO: Make or improve GetLiftPosition to Get latest added record
  Task<bool> SaveLiftPosition(Data.LiftPosition liftPosition);
  Task<Data.LiftPosition> GetLiftPosition();
}