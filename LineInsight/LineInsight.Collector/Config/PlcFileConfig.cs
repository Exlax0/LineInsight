using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LineInsight.Collector.Config;

public sealed class PlcFileConfig
{
    public string Name { get; set; } = "";
    public string Vendor { get; set; } = "SiemensS7";
    public string Address { get; set; } = "";
    public int Rack { get; set; }
    public int Slot { get; set; }
    public int PollMs { get; set; } = 1000;
    public bool Enabled { get; set; } = true;
    public List<MachineFileConfig> Machines { get; set; } = new();
}
public sealed class MachineFileConfig
{
    public string Name { get; set; } = "";
    public List<TagFileConfig> Tags { get; set; } = new();
}
public sealed class TagFileConfig
{
    public string Name { get; set; } = "";
    public string Address { get; set; } = "";
    public string DataType { get; set; } = "Bool";
    public bool IsKpiCritical { get; set; }
}
