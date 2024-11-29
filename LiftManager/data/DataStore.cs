
using LiftManager.Data.Dto;
using LiteDB;

namespace LiftManager.Data;

public class DataStore : IDataStore
{
  private IAppSettings _appSettings;
  /// <summary>
  /// This will serve as a temporal store for LisftPosition until 10 elements to do a bulk insert.
  /// </summary>
  private Queue<LiftPositionDto> _queue;
  private Lazy<LiteDatabase> _liteDatabase;
  private Core.ILogger _logger;
  private readonly int NUMBER_FILES_FOR_BULK_INSERT;

  public DataStore(IAppSettings appSettings, Core.ILogger logger)
  {
    _appSettings = appSettings;
    _liteDatabase = new Lazy<LiteDatabase>(CreateLiteDatabase);
    _queue = new(_appSettings.FilesForBulk);
    _logger = logger;

    NUMBER_FILES_FOR_BULK_INSERT = _appSettings.FilesForBulk;
  }

  private LiteDatabase CreateLiteDatabase()
  {
    var database = new LiteDatabase(_appSettings.DatabaseDirectory);

    var collection = database.GetCollection<LiftPositionDto>();
    collection.EnsureIndex(x => x.Id);
    // collection.EnsureIndex(x => x.RequestedDate);
    return database;
  }

  public async Task<LiftPositionDto> GetLatest()
  {
    _logger.LogTrace("Inserting in cache");
    var collection = _liteDatabase.Value.GetCollection<LiftPositionDto>();
    var first = collection.FindAll().OrderByDescending(x => x).FirstOrDefault();
    var last = collection.FindAll().OrderByDescending(x => x).LastOrDefault();
    return first?.Id < last?.Id ? last : first;
  }

  public async Task<bool> Save(LiftPositionDto toSave)
  {
    _logger.LogTrace($"Caching file: Id=${toSave.Id}, Starting floor=${toSave.SourceFloor}, Destination Floor=${toSave.DestinationFloor}");
    _queue.Enqueue(toSave);

    if (_queue.Count == NUMBER_FILES_FOR_BULK_INSERT)
      return SaveInBulk();

    return true;
  }

  public async Task<bool> Drop()
  {
    var coll = _liteDatabase.Value.GetCollection<LiftPositionDto>();
    var removed = coll.DeleteAll();
    return removed > 0;
  }

  private bool SaveInBulk()
  {
    _logger.LogTrace("Inserting bulk in local data base");
    var coll = _liteDatabase.Value.GetCollection<LiftPositionDto>();
    var insertedCount = coll.InsertBulk(_queue);
    var allInserted = insertedCount == _queue.Count;
    _queue.Clear();
    return allInserted;
  }

  public void Dispose()
  {
    GC.SuppressFinalize(this);
    _liteDatabase = null;
    _appSettings = null;
    _appSettings = null;
    _queue = null;
  }
}