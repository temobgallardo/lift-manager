namespace LiftManager.Data;

public interface IDataSynchronizer
{
  Task<LiftPositionDto> GetPosition();
  Task<LiftPositionDto> SavePosition();
}