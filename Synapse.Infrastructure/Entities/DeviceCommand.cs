using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Synapse.Shared.Constants;

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

    [NotMapped]
    public string DisplayCommand
    {
        get
        {
            if (string.Equals(Command, SequenceConstants.CommandLoop, StringComparison.OrdinalIgnoreCase))
            {
                var loopCount = CommandParameters?.FirstOrDefault(p => p.Name == SequenceConstants.LoopCountParameterName)?.Value;
                var suffix = string.IsNullOrWhiteSpace(loopCount) ? string.Empty : $" x{loopCount}";
                return $"{Command}{suffix}";
            }

            return Command;
        }
    }
}
