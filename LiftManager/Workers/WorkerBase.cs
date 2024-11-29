namespace LiftManager.Workers;

public abstract class WorkerBase : BackgroundService
{
  protected ILogger<WorkerBase> Logger { get; }

  protected IConfiguration Configuration { get; }

  protected WorkerBase(ILogger<WorkerBase> logger, IConfiguration configuration)
  {
    Logger = logger;
    Configuration = configuration;
  }
}