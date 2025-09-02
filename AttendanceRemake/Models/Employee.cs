using System;
using System.Collections.Generic;

namespace AttendanceRemake.Models;

public partial class Employee
{
    public int EmpNo { get; set; }

    public int FingerCode { get; set; }

    public int DeptCode { get; set; }

    public string NameA { get; set; } = null!;

    public string NameE { get; set; } = null!;

    public int TimingCode { get; set; }

    public int JobType { get; set; }

    public short Sex { get; set; }

    public short CheckLate { get; set; }

    public bool HasAllow { get; set; }

    public int Status { get; set; }

    public bool InLeave { get; set; }

    public bool? HasPass { get; set; }

    public int? LocCode { get; set; }

    public int? RegNo { get; set; }

    public virtual ICollection<EmpAllow> EmpAllows { get; set; } = new List<EmpAllow>();

    public virtual TimingPlan TimingCodeNavigation { get; set; } = null!;
}
