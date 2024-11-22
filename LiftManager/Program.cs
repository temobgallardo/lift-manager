// var builder = ConsoleApplication.CreateBuilder(args);

// var builder = new ConfigurationBuilder()
//     .SetBasePath(Directory.GetCurrentDirectory())
//     .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
//     .AddEnvironmentVariables()
//     .AddCommandLine(args);

// IConfiguration configuration = builder.Build();


using LiftManager;
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
    .Build();

var appSettings = config.GetSection(nameof(AppSettings)).Get<AppSettings>();

var builder = Host.CreateDefaultBuilder(args)
    .UseSerilog()
    .ConfigureServices(services =>
    {
        //   services.AddSingleton<IRequestMatrixService, RequestMatrixService>();
        //   services.AddSingleton<IRequestWordsToSearchService, RequestWordsToSearchService>();
        //   services.AddSingleton<ITrie<string>, Trie>();
        //   services.AddScoped<IWordFinder, WordFinder.WordFinder>();
        //   services.AddHostedService<WordFinderWorker>();
        services.AddScoped<IAppSettings>((sp) => appSettings);
    })
    .ConfigureAppConfiguration(c =>
    {
        c.AddConfiguration(config);
    });


using IHost host = builder.Build();
await host.RunAsync();
