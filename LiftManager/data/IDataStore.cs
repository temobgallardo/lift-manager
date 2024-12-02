using LiftManager.Data.Dto;

namespace LiftManager.Data;

public interface IDataStore : IDisposable
{
  Task<bool> Save(LiftPositionDto toSave);
  Task<LiftPositionDto> GetLatest();
  Task<bool> DeleteAll();
  /// <summary>
  /// Delete all the collectioins and internal files from the db
  /// </summary>
  /// <returns></returns>
  Task Clear();
}