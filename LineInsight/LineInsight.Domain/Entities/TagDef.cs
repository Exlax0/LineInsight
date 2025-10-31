using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LineInsight.Domain.Enums;

namespace LineInsight.Domain.Entities;

public sealed class TagDef
{
    public int Id { get; set; }
    public int MachineId { get; set; }
    public Machine Machine { get; set; } = null!;
    public string Name { get; set; } = "";
    public string Address { get; set; } = "";  // e.g. "DB1.DBX0.0"
    public TagType DataType { get; set; }
    public bool IsKpiCritical { get; set; } = false;
}
