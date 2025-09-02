using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AttendanceRemake.Models;

public partial class Location
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
    public int Code { get; set; }


    [JsonPropertyName("descA")] 
    [Required]
    public string DescA { get; set; } = string.Empty;

    [JsonPropertyName("descE")] 
    [Required]
    public string DescE { get; set; } = string.Empty;
}
