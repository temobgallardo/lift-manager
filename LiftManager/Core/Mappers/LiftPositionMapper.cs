using LiftManager.Data.Dto;
using LiftManager.Domain.Data;

public static class LiftPositionMapper
{
  /// <summary>
  /// Transform the dto to a domain model
  /// </summary>
  /// <param name="dto">Source class</param>
  /// <returns></returns>
  public static LiftPosition ToDomain(this LiftPositionDto dto) => new(dto.RequestedDate, dto.SourceFloor, dto.DestinationFloor, dto.OperationType);
  /// <summary>
  /// Transform the domain to a DTO model
  /// </summary>
  /// <param name="domain">source</param>
  /// <returns></returns>
  public static LiftPositionDto ToDto(this LiftPosition domain) => new(domain.RequestedDate, domain.SourceFloor, domain.DestinationFloor, domain.OperationType);
}