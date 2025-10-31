using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LineInsight.Domain.Enums;

namespace LineInsight.Domain.Entities;

public sealed class TagReading
{
    public long Id { get; set; }
    public int TagDefId { get; set; }
    public TagDef Tag { get; set; } = null!;
    public DateTime UtcTs { get; set; }
    public string Value { get; set; } = "";
    public ReadQuality Quality { get; set; }
}

