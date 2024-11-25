namespace LiftManager.Data;

/// <summary>
/// 
/// </summary>
/// <param name="Id"></param>
/// <param name="SourceFloor"></param>
/// <param name="DestinationFloor">Null means no destination at the moment</param>
public record class LiftPositionDto(int Id, int SourceFloor, int? DestinationFloor);