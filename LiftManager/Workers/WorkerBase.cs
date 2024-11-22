namespace LiftManager.Workers;

public abstract class WorkerBase(ILogger<WorkerBase> logger, IConfiguration configuration) : BackgroundService
{
  protected ILogger<WorkerBase> Logger { get; } = logger;

  protected IConfiguration Configuration { get; } = configuration;
}