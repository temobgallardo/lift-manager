using LiftManager.Domain.Enums;

namespace LiftManager.Workers;

public class LiftManagerWorker : WorkerBase
{
  public LiftManagerWorker(ILogger<WorkerBase> logger, IConfiguration configuration) : base(logger, configuration)
  {
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    Logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

    CheckIfOutsideLiftRequestAndOperate();
    CheckIfInsideLiftRequestAndOperate();

    Logger.LogInformation("Work completed at: {time}", DateTime.Now);
  }

  private void CheckIfInsideLiftRequestAndOperate()
  {
    string[]? args = GetArgs(LiftLocation.Inside.ToString());

    if (args == null)
    {
      return;
    }

    Logger.LogInformation($"Args readed: {args}");
  }

  private void CheckIfOutsideLiftRequestAndOperate()
  {
    string[]? args = GetArgs(LiftLocation.Outside.ToString());

    if (args == null)
    {
      return;
    }

    Logger.LogInformation($"Args readed: {args}");
  }

  private string[]? GetArgs(string typeOfArgs) => Configuration[typeOfArgs]?.Split(',');
}
