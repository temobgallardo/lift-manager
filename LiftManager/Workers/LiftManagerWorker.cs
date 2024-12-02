using LiftManager.Core;

namespace LiftManager.Workers;

public class LiftManagerWorker : WorkerBase
{
  private readonly IOperator _liftOperator;
  private const string ARG_INDEX_KEY = "liftRequest";

  public LiftManagerWorker(ILogger<WorkerBase> logger, IConfiguration configuration, IOperator liftOperator) : base(logger, configuration)
  {
    _liftOperator = liftOperator;
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    Logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

    // TODO: a request manager on top of the Operator so it can better pass request to it in a more efficient manner (not so linear).

#if DEBUG
    // string[] experiment = new[] { "outside=1", "inside=2", "outside=3", "inside=1", "outside=2", "outside=10", "outside=1", "inside=10", "inside=10", "outside=4", "inside=6" };

    string[] experiment = Configuration[ARG_INDEX_KEY].Split(',');
    foreach (var e in experiment)
    {
      var arg = e.Split('=');
      if (arg[0].Contains("outside"))
      {
        await CheckIfOutsideLiftRequestAndOperate(arg[1]);
      }
      else
      {
        await CheckIfInsideLiftRequestAndOperate(arg[1]);
      }
    }
#else
     string[] experiment = Configuration[ARG_INDEX_KEY].Split(',');
        foreach (var e in experiment)
        {
          var arg = e.Split('=');
          if (arg[0].Contains("outside"))
          {
            await CheckIfOutsideLiftRequestAndOperate(arg[1]);
          }
          else
          {
            await CheckIfInsideLiftRequestAndOperate(arg[1]);
          }
        }
        await CheckIfOutsideLiftRequestAndOperate(Configuration["outside"]);
        await CheckIfInsideLiftRequestAndOperate(Configuration["inside"]);
#endif

    Logger.LogInformation("Work completed at: {time}", DateTime.Now);
  }

  private async Task CheckIfInsideLiftRequestAndOperate(string floor)
  {
    if (floor == null)
    {
      Logger.LogInformation($"No operation requested");
      return;
    }

    Logger.LogInformation($"Lift operation requested | Operation: Inside Lift | Move to floor: {floor}");
    await _liftOperator.LiftToFloor(Convert.ToInt16(floor));
  }

  private async Task CheckIfOutsideLiftRequestAndOperate(string floor)
  {
    if (floor == null)
    {
      Logger.LogInformation($"No operation requested");
      return;
    }

    Logger.LogInformation($"Lift operation requested | Operation: Outside Lift | Move to floor: {floor}");

    await _liftOperator.RequestLiftToFloor(Convert.ToInt16(floor));
  }
}
