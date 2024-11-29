using LiftManager.Data.Dto;

namespace LiftManager.Data;

public interface IDataStore : IDisposable
{
  Task<bool> Save(LiftPositionDto toSave);
  Task<LiftPositionDto> GetLatest();
  Task<bool> Drop();
}