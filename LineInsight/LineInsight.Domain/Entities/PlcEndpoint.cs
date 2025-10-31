using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LineInsight.Domain.Enums;

namespace LineInsight.Domain.Entities;

public sealed class PlcEndpoint
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public PlcVendor Vendor { get; set; } = PlcVendor.SiemensS7;
    public string Address { get; set; } = "";
    public int Rack { get; set; } = 0;
    public int Slot { get; set; } = 1;
    public int PollMs { get; set; } = 1000;
    public bool Enabled { get; set; } = true;
}

