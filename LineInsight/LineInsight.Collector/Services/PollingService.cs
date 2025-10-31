using LineInsight.Domain.Entities;
using LineInsight.Domain.Enums;
using LineInsight.Domain.Interfaces;
using LineInsight.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LineInsight.Collector.Services;

public sealed class PollingService : BackgroundService
{
    private readonly IServiceProvider _sp;
    private readonly IPlcConnectorFactory _factory;
    private readonly ILogger<PollingService> _log;

    public PollingService(IServiceProvider sp, IPlcConnectorFactory factory, ILogger<PollingService> log)
    {
        _sp = sp;
        _factory = factory;
        _log = log;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<LineDbContext>();

            var machines = await db.Machines
                .Include(m => m.Plc)
                .Include(m => m.Tags)
                .Where(m => m.Plc.Enabled)
                .OrderBy(m => m.PlcEndpointId)
                .ToListAsync(stoppingToken);

            foreach (var group in machines.GroupBy(m => m.PlcEndpointId))
                _ = Task.Run(() => PollGroupAsync(group.ToList(), stoppingToken));

            await Task.Delay(500, stoppingToken);
        }
    }

    private async Task PollGroupAsync(List<Machine> list, CancellationToken ct)
    {
        var plcCfg = list.First().Plc;

        try
        {
            await using var conn = _factory.Create(plcCfg);
            await conn.ConnectAsync(ct);

            var now = DateTime.UtcNow;
            using var scope = _sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<LineDbContext>();

            foreach (var m in list)
            {
                foreach (var t in m.Tags)
                {
                    try
                    {
                        var val = await conn.ReadAsync(t, ct);
                        db.Readings.Add(new TagReading
                        {
                            TagDefId = t.Id,
                            UtcTs = now,
                            Value = val?.ToString() ?? "",
                            Quality = ReadQuality.Good
                        });
                    }
                    catch (Exception ex)
                    {
                        _log.LogWarning(ex, "Read failed {Plc}/{Machine}/{Tag}", plcCfg.Name, m.Name, t.Name);
                        db.Readings.Add(new TagReading
                        {
                            TagDefId = t.Id,
                            UtcTs = DateTime.UtcNow,
                            Value = "",
                            Quality = ReadQuality.Bad
                        });
                    }
                }
            }

            await db.SaveChangesAsync(ct);
            await Task.Delay(plcCfg.PollMs, ct);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "PLC group error {Plc}", plcCfg.Name);
            await Task.Delay(2000, ct);
        }
    }
}
