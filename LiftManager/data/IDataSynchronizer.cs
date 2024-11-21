namespace LiftManager.data;

public interface IDataSynchronizer
{
  Task<LiftPositionDto> GetPosition();
  Task<LiftPositionDto> SavePosition();
}