using LiftManager.domain.enums;

namespace LiftManager.Workers;

public class LiftManagerWorker(ILogger<LiftManagerWorker> logger, IConfiguration configuration) : WorkerBase(logger, configuration)
{
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


  }

  private void CheckIfOutsideLiftRequestAndOperate()
  {
    string[]? args = GetArgs(LiftLocation.Outside.ToString());

    if (args == null)
    {
      return;
    }
  }

  private string[]? GetArgs(string typeOfArgs) => Configuration[typeOfArgs]?.Split(',');
}
