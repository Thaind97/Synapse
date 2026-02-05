using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Synapse.Infrastructure.Entities;

public class SystemLog
{
    [Key]
    public long Id { get; set; }

    public long? TestRunId { get; set; }

    [ForeignKey(nameof(TestRunId))]
    public TestRun? TestRun { get; set; }

    [Required]
    public string LogTime { get; set; } = string.Empty;

    [Required]
    public string Level { get; set; } = string.Empty;

    [Required]
    public string Message { get; set; } = string.Empty;
}
