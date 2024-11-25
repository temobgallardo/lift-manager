namespace LiftManager.Data;

public interface IDataStore
{
  Task<LiftPositionDto> Read();
  Task Write(LiftPositionDto liftPosition);
}