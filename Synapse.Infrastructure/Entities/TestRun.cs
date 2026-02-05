using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Synapse.Infrastructure.Entities;

public class TestRun
{
    [Key]
    public long Id { get; set; }

    public long SequenceId { get; set; }

    [ForeignKey(nameof(SequenceId))]
    public Sequence Sequence { get; set; } = null!;

    [Required]
    public string StartedAt { get; set; } = string.Empty;

    public string? EndedAt { get; set; }

    [Required]
    public string Status { get; set; } = string.Empty;
    
    public ICollection<MeasurementLog> MeasurementLogs { get; set; } = new List<MeasurementLog>();
    public ICollection<SystemLog> SystemLogs { get; set; } = new List<SystemLog>();
}
