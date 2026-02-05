using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Synapse.Infrastructure.Entities;

public class CommandParameter
{
    [Key]
    public long Id { get; set; }

    public long CommandId { get; set; }

    [ForeignKey(nameof(CommandId))]
    public DeviceCommand Command { get; set; } = null!;

    [Required]
    public string Name { get; set; } = string.Empty;

    public string? Value { get; set; }

    public string? Unit { get; set; }

    public long? VariableId { get; set; }

    [ForeignKey(nameof(VariableId))]
    public Variable? Variable { get; set; }
}
