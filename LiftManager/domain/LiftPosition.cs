namespace LiftManager.Domain;

/// <summary>
/// 
/// </summary>
/// <param name="Id"></param>
/// <param name="Source"></param>
/// <param name="Destination">Null means no destination at the moment</param>
public record class LiftPosition(int Id, int Source, int? Destination);