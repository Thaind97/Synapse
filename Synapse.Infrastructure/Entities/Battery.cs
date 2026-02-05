using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Synapse.Infrastructure.Entities;

public class Battery
{
    [Key]
    public long Id { get; set; }

    // Channel number (1..24) - unique
    public int Channel { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}
