using LiftManager.Domain;

namespace LiftManager.Core;

public class Operator(IRepository repository, ILogger logger, IAppSettings appSettings) : IOperator
{
  private ILogger? _logger = logger;
  private IAppSettings _appSettings = appSettings;
  private IRepository? _repository = repository;

  public async Task<bool> MoveToFloor(int destinationFloor)
  {
    if (destinationFloor < 0 && destinationFloor >= _appSettings.NumberOfFloors)
    {
      _logger?.LogInformation($"Invalid Destinatio Floor={destinationFloor}. Cancelling operation");
    }

    LiftPosition liftPosition = await GetLiftPosition();

    if (liftPosition is null)
    {
      _logger?.LogInformation($"No Lift Positioin Available in the DB");
    }

    if (liftPosition!.Source == destinationFloor)
    {
      _logger?.LogTrace($"Moving to floor: Source Position = {liftPosition.Source} | Destination Position = {liftPosition.Destination}");
      return await Task.FromResult(true);
    }

    return await _repository!.SaveLiftPosition(new LiftPosition(liftPosition.Id + 1, destinationFloor, destinationFloor));
  }

  // TODO: Add functionality to make the model natural
  public async Task<bool> Stop()
  {
    _logger?.LogTrace("Stopping lift");
    await Task.Delay(500);
    return await Task.FromResult(true);
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