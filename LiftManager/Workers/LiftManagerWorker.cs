using LiftManager.Core;
using LiftManager.Domain.Enums;

namespace LiftManager.Workers;

public class LiftManagerWorker : WorkerBase
{
  private readonly IOperator _liftOperator;

  public LiftManagerWorker(ILogger<WorkerBase> logger, IConfiguration configuration, IOperator liftOperator) : base(logger, configuration)
  {
    _liftOperator = liftOperator;
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    Logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

#if DEBUG
    string[] experment = new[] { "outside", "inside", "outside", "inside", "outside" };
#else
    CheckIfOutsideLiftRequestAndOperate();
    CheckIfInsideLiftRequestAndOperate();
#endif
    Logger.LogInformation("Work completed at: {time}", DateTime.Now);
  }

  private void CheckIfInsideLiftRequestAndOperate()
  {
    string[]? args = GetArgs(OperationType.Inside.ToString());

    if (args == null)
    {
      return;
    }

    Logger.LogInformation($"Lift operation requested | Operation: Inside Lift | Move to floor: {args[0]}");
    _liftOperator.LiftToFloor(Convert.ToInt16(args[0]));
  }

  private void CheckIfOutsideLiftRequestAndOperate()
  {
    string[]? args = GetArgs(OperationType.Outside.ToString());

    if (args == null)
    {
      return;
    }

    Logger.LogInformation($"Lift operation requested | Operation: Outside Lift | Move to floor: {args[0]}");

    _liftOperator.RequestLiftToFloor(Convert.ToInt16(args[0]));
  }

  private string[]? GetArgs(string typeOfArgs) => Configuration[typeOfArgs]?.Split(',');
}
