namespace LiftManager.data;

/// <summary>
/// 
/// </summary>
/// <param name="Id"></param>
/// <param name="CurrentPosition"></param>
/// <param name="SourceFloor"></param>
/// <param name="DestinationFloor">Null means no destination at the moment</param>
public record class LiftPosition(int Id, int CurrentPosition, int SourceFloor, int? DestinationFloor);