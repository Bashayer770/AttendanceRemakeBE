using System;
using System.Collections.Generic;

namespace AttendanceRemake.Models;

public partial class EmpAllow
{
    public long Serial { get; set; }

    public int EmpNo { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime RealStartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int AllowTimeCode { get; set; }

    public int Status { get; set; }

    public int DedHr { get; set; }

    public virtual Employee EmpNoNavigation { get; set; } = null!;
}
