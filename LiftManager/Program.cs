using LiftManager;
using LiftManager.Core;
using LiftManager.Data;
using LiftManager.Domain;
using LiftManager.Workers;
using Serilog;

Log.Logger = new LoggerConfiguration()
#if DEBUG
      .WriteTo.Console()
#else
     .WriteTo.File(
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, nameof(WordFinderWorker) + "-service.log")
    )
#endif
    .CreateLogger();

IConfigurationRoot config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .Build();

var appSettings = config.GetSection(nameof(AppSettings)).Get<AppSettings>();

var builder = Host.CreateDefaultBuilder(args)
    // .UseEnvironment("Production")
    .UseSerilog()
    .ConfigureServices(services =>
    {
        services.AddSingleton<IAppSettings>((sp) => appSettings);
        services.AddSingleton(Log.Logger);
        services.AddSingleton<LiftManager.Core.ILogger, Logger>();
        services.AddSingleton<IDataStore, DataStore>();
        services.AddSingleton<IRepository, Repository>();
        services.AddSingleton<IOperator, Operator>();
        services.AddHostedService<LiftManagerWorker>();
    })
    .ConfigureAppConfiguration(c =>
    {
        c.AddConfiguration(config);
    });

using IHost host = builder.Build();
host.Run();
