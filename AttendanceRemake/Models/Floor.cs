using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AttendanceRemake.Models;

public partial class Floor
{
    [Key]
    public string Floor1 { get; set; } = null!;

    public string DescA { get; set; } = null!;

    public string DescB { get; set; } = null!;
}
