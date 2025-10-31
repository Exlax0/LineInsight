using LineInsight.Domain.Entities;
using LineInsight.Domain.Enums;
using LineInsight.Domain.Interfaces;
using LineInsight.Infrastructure.Connectors;

namespace LineInsight.Infrastructure.Factories;
public sealed class PlcConnectorFactory : IPlcConnectorFactory
{
    public IPlcConnector Create(PlcEndpoint ep) => ep.Vendor switch
    {
        PlcVendor.SiemensS7 => new SiemensS7Connector(ep),
        _ => throw new NotSupportedException($"Vendor {ep.Vendor} not implemented yet")
    };
}
