using LiftManager.Domain.Data;
using LiftManager.Domain;

namespace LiftManager.Core;

public class Operator : IOperator
{
  private ILogger? _logger;
  private IAppSettings _appSettings;
  private IRepository? _repository;

  public Operator(IRepository repository, ILogger logger, IAppSettings appSettings)
  {
    _logger = logger;
    _appSettings = appSettings;
    _repository = repository;
  }

  public async Task<bool> LiftToFloor(int destinationFloor)
  {
    if (destinationFloor < _appSettings.InitialFloor || destinationFloor >= _appSettings.NumberOfFloors)
    {
      _logger?.LogError(new Exception(), $"Invalid Destinatio Floor | Destinatio Floor={destinationFloor} is out of boundaries [{_appSettings.InitialFloor}, {_appSettings.NumberOfFloors}]. Cancelling operation...");
      return false;
    }

    LiftPosition liftPosition = await GetLiftPosition();

    if (liftPosition is null)
    {
      _logger?.LogInformation($"No Lift Positioin Available in the DB");
      return false;
    }

    _logger?.LogTrace($"Moving to floor: Source Position = {liftPosition.SourceFloor} | Destination Position = {liftPosition.DestinationFloor}");

    if (liftPosition!.SourceFloor == destinationFloor)
    {
      return true;
    }

    return await _repository!.SaveLiftPosition(new LiftPosition(DateTime.Now, destinationFloor, destinationFloor));
  }

  // TODO: Add functionality to make the model natural
  public async Task<bool> Stop()
  {
    _logger?.LogTrace("Stopping lift");
    await Task.Delay(500);
    return true;
  }

  public void Dispose()
  {
    _repository?.Dispose();
    _repository = null;
    _logger = null;
    GC.SuppressFinalize(this);
  }

  private async Task<LiftPosition?> GetLiftPosition()
  {
    try
    {
      return await _repository?.GetLiftPosition();
    }
    catch (Exception e)
    {
      _logger?.LogError(e, $"Issue while retrieving the saved lift position | Message: {e.Message}");
    }

    return null;
  }

}