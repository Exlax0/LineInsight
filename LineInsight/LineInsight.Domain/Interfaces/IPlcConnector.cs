using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LineInsight.Domain.Entities;

namespace LineInsight.Domain.Interfaces;

public interface IPlcConnector : IAsyncDisposable
{
    Task ConnectAsync(CancellationToken ct);
    Task<object?> ReadAsync(TagDef tag, CancellationToken ct);
}

