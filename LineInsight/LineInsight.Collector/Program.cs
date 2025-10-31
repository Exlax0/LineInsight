using LineInsight.Collector.Services;
using LineInsight.Domain.Interfaces;
using LineInsight.Infrastructure;
using LineInsight.Infrastructure.Factories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

await Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(c =>
        c.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true))
    .ConfigureServices((ctx, services) =>
    {
        var cs = ctx.Configuration.GetSection("Database")["ConnectionString"]!;
        services.AddDbContext<LineDbContext>(o => o.UseSqlite(cs));

        services.AddSingleton<IPlcConnectorFactory, PlcConnectorFactory>();
        services.AddHostedService<ConfigSyncService>();
        services.AddHostedService<PollingService>();
    })
    .RunConsoleAsync();
