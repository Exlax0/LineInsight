using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LineInsight.Domain.Entities;

public sealed class Machine
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int PlcEndpointId { get; set; }
    public PlcEndpoint Plc { get; set; } = null!;
    public ICollection<TagDef> Tags { get; set; } = new List<TagDef>();
}
