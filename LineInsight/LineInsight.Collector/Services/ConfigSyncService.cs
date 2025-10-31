using LineInsight.Collector.Config;
using LineInsight.Domain.Entities;
using LineInsight.Domain.Enums;
using LineInsight.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LineInsight.Collector.Services;

public sealed class ConfigSyncService : BackgroundService
{
    private readonly IServiceProvider _sp; private readonly IConfiguration _cfg;
    public ConfigSyncService(IServiceProvider sp, IConfiguration cfg) { _sp = sp; _cfg = cfg; }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await SyncAsync(stoppingToken);
        // (Optional) subscribe to reload token for hot-reload later
    }

    private async Task SyncAsync(CancellationToken ct)
    {
        var list = _cfg.GetSection("PlcConfig").Get<List<PlcFileConfig>>() ?? new();
        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<LineDbContext>();
        await db.Database.EnsureCreatedAsync(ct);

        foreach (var ep in list)
        {
            var vendor = Enum.Parse<PlcVendor>(ep.Vendor, true);
            var plc = await db.Plcs.FirstOrDefaultAsync(p => p.Name == ep.Name, ct);
            if (plc is null)
            {
                plc = new PlcEndpoint { Name = ep.Name, Vendor = vendor, Address = ep.Address, Rack = ep.Rack, Slot = ep.Slot, PollMs = ep.PollMs, Enabled = ep.Enabled };
                db.Plcs.Add(plc);
                await db.SaveChangesAsync(ct);
            }
            else
            {
                plc.Vendor = vendor; plc.Address = ep.Address; plc.Rack = ep.Rack; plc.Slot = ep.Slot; plc.PollMs = ep.PollMs; plc.Enabled = ep.Enabled;
            }

            foreach (var mCfg in ep.Machines)
            {
                var m = await db.Machines.Include(x => x.Tags)
                    .FirstOrDefaultAsync(x => x.Name == mCfg.Name && x.PlcEndpointId == plc.Id, ct)
                    ?? db.Machines.Add(new Machine { Name = mCfg.Name, Plc = plc }).Entity;

                foreach (var tCfg in mCfg.Tags)
                {
                    var tag = m.Tags.FirstOrDefault(t => t.Name == tCfg.Name);
                    var tt = Enum.Parse<TagType>(tCfg.DataType, true);
                    if (tag is null) m.Tags.Add(new TagDef { Name = tCfg.Name, Address = tCfg.Address, DataType = tt, IsKpiCritical = tCfg.IsKpiCritical });
                    else { tag.Address = tCfg.Address; tag.DataType = tt; tag.IsKpiCritical = tCfg.IsKpiCritical; }
                }
            }
        }
        await db.SaveChangesAsync(ct);
    }
}
