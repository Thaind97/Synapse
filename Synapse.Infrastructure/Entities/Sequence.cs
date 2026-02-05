using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Synapse.Infrastructure.Entities;

public class Sequence
{
    [Key]
    public long Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public string CreatedAt { get; set; } = string.Empty;

    public string? UpdatedAt { get; set; }

    public ICollection<SequenceStep> Steps { get; set; } = new List<SequenceStep>();
}
