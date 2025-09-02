using System;
using System.Collections.Generic;

namespace AttendanceRemake.Models;

public partial class Node
{
    public string SerialNo { get; set; } = null!;

    public string DescA { get; set; } = null!;

    public string DescE { get; set; } = null!;

    public int LocCode { get; set; }

    public string? Floor { get; set; }
}
