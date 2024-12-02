using LiftManager.Data.Dto;
using LiteDB;

namespace LiftManager.Data;

public class DataStore : IDataStore
{
  private LiftPositionDto _last;
  private IAppSettings _appSettings;
  /// <summary>
  /// Serves as a temporal store until X elements reached then do a bulk insert.
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

  public async Task<LiftPositionDto> GetLatest()
  {
    if (_queue.Count > 0)
    {
      return _last;
    }

    _logger.Debug("Inserting in cache");
    var collection = _liteDatabase.Value.GetCollection<LiftPositionDto>();
    var last = collection.FindAll().OrderByDescending(x => x).FirstOrDefault();
    return last;
  }

  public async Task<bool> Save(LiftPositionDto toSave)
  {
    _logger.Information("Caching file: {@ToSave}", toSave);
    _last = toSave;
    _queue.Enqueue(toSave);

    if (_queue.Count == NUMBER_FILES_FOR_BULK_INSERT)
      return SaveInBulk();

    return true;
  }

  public async Task<bool> DeleteAll()
  {
    var coll = _liteDatabase.Value.GetCollection<LiftPositionDto>();
    var removed = coll.DeleteAll();
    return removed > 0;
  }

  public void Dispose()
  {
    GC.SuppressFinalize(this);
    _liteDatabase = null;
    _appSettings = null;
    _queue = null;
    _logger = null;
    _last = null;
  }

  public async Task Clear()
  {
    _liteDatabase.Value.DropCollection(nameof(LiftPositionDto));
    _liteDatabase.Value.DropCollection("_files");
    _liteDatabase.Value.DropCollection("_chunks");
  }

  private bool SaveInBulk()
  {
    _logger.Information("Inserting bulk in local data base {@Queue}", _queue);
    var coll = _liteDatabase.Value.GetCollection<LiftPositionDto>();
    var insertedCount = coll.InsertBulk(_queue);
    var allInserted = insertedCount == _queue.Count;
    _queue.Clear();
    return allInserted;
  }

  private LiteDatabase CreateLiteDatabase()
  {
    var database = new LiteDatabase(_appSettings.DatabaseDirectory);

    var collection = database.GetCollection<LiftPositionDto>();
    collection.EnsureIndex(x => x.Id);
    collection.EnsureIndex(x => x.RequestedDate);

    return database;
  }

}