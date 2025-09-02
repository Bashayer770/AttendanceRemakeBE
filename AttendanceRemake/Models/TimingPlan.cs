using System;
using System.Collections.Generic;

namespace AttendanceRemake.Models;

public partial class TimingPlan
{
    public int Code { get; set; }

    public string DescA { get; set; } = null!;

    public string DescE { get; set; } = null!;

    public string FromTime { get; set; } = null!;

    public string ToTime { get; set; } = null!;

    public string RmdFromTime { get; set; } = null!;

    public string RmdToTime { get; set; } = null!;

    public bool IsRamadan { get; set; }

    public bool CanGoBefore { get; set; }

    public bool? Activity { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
