using LiftManager.Data.Dto;

namespace LiftManager.Data;

public interface ILiftRequestQueue : IDisposable
{
  /// <summary>
  /// Push
  /// </summary>
  /// <param name="liftPosition"></param>
  /// <returns></returns>
  bool QueueLiftRequest(LiftPositionDto liftPosition);
  /// <summary>
  /// Pop
  /// </summary>
  /// <returns></returns>
  bool GetLiftRequest();
}