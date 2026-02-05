using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Synapse.Infrastructure.Entities;

[Index(nameof(Name), IsUnique = true)]
public class Variable
{
    [Key]
    public long Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string DataType { get; set; } = string.Empty;

    public string? Unit { get; set; }

    public string? DefaultValue { get; set; }

    public string? Description { get; set; }
}
