using WorkerService1;


IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        IConfiguration configuration = hostContext.Configuration;

        Configuration options = configuration.GetSection("Configuration").Get<Configuration>();

        services.AddSingleton(options);
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
