using LiftManager.Data;
using LiftManager.Domain.Data;

public static class LiftPositionMapper
{
  /// <summary>
  /// Transform the dto to a domain model
  /// </summary>
  /// <param name="s">Source class</param>
  /// <returns></returns>
  public static LiftPosition ToDomain(this LiftPositionDto s) => new(s.Id, s.SourceFloor, s.DestinationFloor);
  /// <summary>
  /// Transform the domain to a DTO model
  /// </summary>
  /// <param name="s">source</param>
  /// <returns></returns>
  public static LiftPositionDto ToDto(this LiftPosition s) => new(s.Id, s.SourceFloor, s.DestinationFloor);
}