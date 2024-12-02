using LiftManager.Domain.Data;
using LiftManager.Domain;
using LiftManager.Domain.Enums;

namespace LiftManager.Core;

public class Operator : IOperator
{
  private bool _disposed = false;
  private ILogger? _logger;
  private IAppSettings _appSettings;
  private IRepository? _repository;

  public Operator(IRepository repository, ILogger logger, IAppSettings appSettings)
  {
    _logger = logger;
    _appSettings = appSettings;
    _repository = repository;
    _disposed = true;
  }

  /// <summary>
  /// This operations happens inside the lift when the user requests it to move to a floor
  /// </summary>
  /// <param name="floor">Floor where the lift will move</param>
  /// <returns>True if operation was successfull, false otherwise</returns>
  public async Task<bool> LiftToFloor(int floor)
  {
    if (floor < _appSettings.InitialFloor || floor >= _appSettings.NumberOfFloors)
    {
      _logger?.Error(new Exception(), $"Invalid Destinatio Floor | Destinatio Floor={floor} is out of boundaries [{_appSettings.InitialFloor}, {_appSettings.NumberOfFloors}]. Cancelling operation...");
      return false;
    }

    LiftPosition currentPosition = await GetLiftPosition();

    if (currentPosition is null)
    {
      _logger?.Debug($"No lift losition available in the DB. Moving to floor={floor}");
      // Initial position of lift is at the bottom (floor 0)
      return await SimulateAndSaveLift(_appSettings.InitialFloor, floor, OperationType.Inside);
    }

    var placeHolder = "{@LiftPosition}";
    _logger?.Information($"Moving from floor: {placeHolder} to {floor}", currentPosition);

    if (currentPosition!.SourceFloor == floor)
    {
      _logger?.Debug($"Lift stays in same floor as destination is current floor");
      return false;
    }


    return await SimulateAndSaveLift(_appSettings.InitialFloor, floor, OperationType.Inside);
  }

  public async Task<bool> Stop()
  {
    _logger?.Debug("Stopping lift");
    await Task.Delay(500);
    return true;
  }

  /// <summary>
  /// This operation happens when user is outside the lift and request it
  /// </summary>
  /// <param name="floor">Floor where the lift will move</param>
  /// <returns>True if operation was successfull, false otherwise</returns>
  public async Task<bool> RequestLiftToFloor(int floor)
  {
    if (floor < _appSettings.InitialFloor || floor > _appSettings.NumberOfFloors)
    {
      _logger?.Information($"Floor requested '{floor}' is out of range [{_appSettings.InitialFloor}, {_appSettings.NumberOfFloors}]");
      return false;
    }

    LiftPosition actualFloor = await GetLiftPosition();
    if (actualFloor is null)
    {
      return await SimulateAndSaveLift(_appSettings.InitialFloor, floor, OperationType.Outside);
    }

    return await SimulateAndSaveLift(actualFloor.DestinationFloor!.Value, floor, OperationType.Outside);
  }

  private async Task<bool> SimulateAndSaveLift(int sourceFloor, int destinationFloor, OperationType operationType)
  {
    _logger?.Debug($"Requesting lift to floor={destinationFloor} | current floor={sourceFloor}");
    LiftPosition newFloorRequest = new(DateTime.Now, sourceFloor, destinationFloor, operationType);
    await SimulateLiftMovement(newFloorRequest);
    return await _repository.SaveLiftPosition(newFloorRequest);
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
      _repository?.Dispose();
      _logger = null;
      _disposed = false;
    }
  }

  private async Task<LiftPosition?> GetLiftPosition()
  {
    try
    {
      return await _repository?.GetCurrentLiftPosition();
    }
    catch (Exception e)
    {
      _logger?.Error(e, $"Issue while retrieving the saved lift position | Message: {e.Message}");
    }

    return null;
  }

  private async Task SimulateLiftMovement(LiftPosition floorRequested)
  {
    for (int f = floorRequested.SourceFloor; f != floorRequested.DestinationFloor;)
    {
      await Task.Delay(_appSettings.TravelSimulationTime);
      _logger?.Information($"Lift on floor number={f}");
      f = floorRequested.SourceFloor > floorRequested.DestinationFloor ? f - 1 : f + 1;
    }

    _logger.Information($"Opening the lift door in floor number = {floorRequested.DestinationFloor}");
  }
}