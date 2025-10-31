using LineInsight.Domain.Entities;
using LineInsight.Domain.Interfaces;
using S7.Net;

namespace LineInsight.Infrastructure.Connectors;
public sealed class SiemensS7Connector : IPlcConnector
{
    private readonly PlcEndpoint _cfg;
    private Plc? _plc;
    public SiemensS7Connector(PlcEndpoint cfg) => _cfg = cfg;

    public async Task ConnectAsync(CancellationToken ct)
    {
        var cpu = CpuType.S71500; // change to S71200 if needed
        _plc = new Plc(cpu, _cfg.Address, (short)_cfg.Rack, (short)_cfg.Slot);
        await Task.Run(() => _plc!.Open(), ct);
        if (!_plc!.IsConnected) throw new Exception($"Cannot connect to {_cfg.Address}");
    }

    public async Task<object?> ReadAsync(TagDef tag, CancellationToken ct)
    {
        if (_plc is null || !_plc.IsConnected) throw new InvalidOperationException("Not connected");
        return await Task.Run(() => _plc!.Read(tag.Address), ct);
    }

    public ValueTask DisposeAsync() { try { _plc?.Close(); } catch { } return ValueTask.CompletedTask; }
}
