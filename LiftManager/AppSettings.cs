namespace LiftManager;

public class AppSettings : IAppSettings
{
  public required int InitialFloor { get; set; }
  public required int NumberOfFloors { get; set; }
}

public interface IAppSettings
{
  int InitialFloor { get; set; }
  int NumberOfFloors { get; set; }
}