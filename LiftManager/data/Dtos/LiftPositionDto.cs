namespace LiftManager.Data.Dto;

/// <summary>
/// 
/// </summary>
/// <param name="Id"></param>
/// <param name="SourceFloor"></param>
/// <param name="DestinationFloor">Null means no destination at the moment</param>
public record class LiftPositionDto : IComparable
{
  public int Id { get; set; }
  public DateTime RequestedDate { get; set; }
  public int SourceFloor { get; set; }
  public int? DestinationFloor { get; set; }
  public LiftPositionDto(DateTime requestedDate, int sourceFloor, int? destinationFloor)
  {
    RequestedDate = requestedDate;
    SourceFloor = sourceFloor;
    DestinationFloor = destinationFloor;
  }
  public LiftPositionDto(int sourceFloor, int? destinationFloor)
  {
    RequestedDate = DateTime.Now;
    SourceFloor = sourceFloor;
    DestinationFloor = destinationFloor;
  }

  public int CompareTo(object? obj)
  {
    if (obj == null) return 1;

    var other = obj as LiftPositionDto ?? throw new ArgumentException("Object is not a Temperature");
    return this.Id.CompareTo(other.Id);
  }
}