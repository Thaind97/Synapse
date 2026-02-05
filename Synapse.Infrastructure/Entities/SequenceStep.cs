using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Synapse.Infrastructure.Entities;

public class SequenceStep
{
    [Key]
    public long Id { get; set; }

    public long SequenceId { get; set; }

    [ForeignKey(nameof(SequenceId))]
    public Sequence Sequence { get; set; } = null!;

    public int StepOrder { get; set; }

    [Required]
    public string StepType { get; set; } = string.Empty;

    public string? StepName { get; set; }
    
    public ICollection<DeviceCommand> DeviceCommands { get; set; } = new List<DeviceCommand>();
    public ICollection<EndCondition> EndConditions { get; set; } = new List<EndCondition>();
}
