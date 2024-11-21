namespace LiftManager.data;

public interface IDataStore
{
  Task<LiftPositionDto> Read();
  Task Write(LiftPositionDto liftPosition);
}