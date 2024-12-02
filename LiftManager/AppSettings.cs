namespace LiftManager;

public class AppSettings : IAppSettings
{
  public required int InitialFloor { get; set; }
  public required int FilesForBulk { get; set; }
  public required int NumberOfFloors { get; set; }
  public required int TravelSimulationTime { get; set; }
  public required string DatabaseDirectory { get; set; }
}

public interface IAppSettings
{
  int InitialFloor { get; set; }
  int FilesForBulk { get; set; }
  int NumberOfFloors { get; set; }
  /// <summary>
  /// The time the lift travel from floor to floor in milliseconds
  /// </summary>
  int TravelSimulationTime { get; set; }

  string DatabaseDirectory { get; set; }

}