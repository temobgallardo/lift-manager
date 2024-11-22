using LiftManager;
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
    .UseSerilog()
    .ConfigureServices(services =>
    {
        services.AddScoped<IAppSettings>((sp) => appSettings);
        services.AddHostedService<LiftManagerWorker>();
    })
    .ConfigureAppConfiguration(c =>
    {
        c.AddConfiguration(config);
    });

using IHost host = builder.Build();
await host.RunAsync();
