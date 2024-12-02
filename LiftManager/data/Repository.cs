using LiftManager.Domain;
using LiftManager.Domain.Data;

namespace LiftManager.Data;

public class Repository : IRepository
{
  private IDataStore _dataStore;
  private Core.ILogger _logger;
  private bool _disposed = false;

  public Repository(IDataStore dataStore, Core.ILogger logger)
  {
    _dataStore = dataStore;
    _logger = logger;
    _disposed = true;
  }

  public async Task<LiftPosition> GetCurrentLiftPosition()
  {
    var current = await _dataStore.GetLatest();
    _logger.Information("Current Lift Position={@Current}", current);
    return current.ToDomain();
  }

  public Task<bool> SaveLiftPosition(LiftPosition toSave)
  {
    _logger.Information("Saving={@ToSave}", toSave);
    return _dataStore.Save(toSave.ToDto());
  }

  public void Dispose()
  {
    Dispose(_disposed);
    GC.SuppressFinalize(this);
  }
  protected virtual void Dispose(bool disposing)
  {
    if (disposing)
    {
      _dataStore?.Dispose();
      _logger = null;
      _disposed = false;
    }
  }
}