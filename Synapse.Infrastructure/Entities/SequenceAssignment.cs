using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Synapse.Infrastructure.Entities;

public class SequenceAssignment
{
    [Key]
    public long Id { get; set; }

    public long SequenceId { get; set; }

    [ForeignKey(nameof(SequenceId))]
    public Sequence Sequence { get; set; } = null!;

    // Battery channel number (1..24)
    public int BatteryChannel { get; set; }

    // Optional FK to Battery master data
    public long? BatteryId { get; set; }

    [ForeignKey(nameof(BatteryId))]
    public Battery? Battery { get; set; }

    public string AssignedAt { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
}
