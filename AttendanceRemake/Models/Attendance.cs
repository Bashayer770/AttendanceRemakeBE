using System;
using System.Collections.Generic;

namespace AttendanceRemake.Models;

public partial class Attendance
{
    public long AttCode { get; set; }

    public int FingerCode { get; set; }

    public DateTime IodateTime { get; set; }

    public string NodeSerialNo { get; set; } = null!;

    public int Status { get; set; }

    public byte[]? Photo { get; set; }

    public int TrType { get; set; }

    public int CurTimPlan { get; set; }
}
