using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Synapse.Infrastructure.Entities;

public class DeviceCommand
{
    [Key]
    public long Id { get; set; }

    public long StepId { get; set; }

    [ForeignKey(nameof(StepId))]
    public SequenceStep Step { get; set; } = null!;

    [Required]
    public string Device { get; set; } = string.Empty;

    [Required]
    public string Command { get; set; } = string.Empty;

    public long? ParentCommandId { get; set; }

    [ForeignKey(nameof(ParentCommandId))]
    public DeviceCommand? ParentCommand { get; set; }

    public ICollection<DeviceCommand> ChildCommands { get; set; } = new List<DeviceCommand>();
    
    public ICollection<CommandParameter> CommandParameters { get; set; } = new List<CommandParameter>();
}
