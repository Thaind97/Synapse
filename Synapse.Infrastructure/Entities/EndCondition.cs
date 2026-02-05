using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Synapse.Infrastructure.Entities;

public class EndCondition
{
    [Key]
    public long Id { get; set; }

    public long StepId { get; set; }

    [ForeignKey(nameof(StepId))]
    public SequenceStep Step { get; set; } = null!;

    [Required]
    public string LogicOp { get; set; } = string.Empty;

    public string? Device { get; set; }

    public string? Item { get; set; }

    public string? Operator { get; set; }

    public double? Value { get; set; }

    public string? Unit { get; set; }

    public string? Action { get; set; }
}
