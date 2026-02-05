using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Synapse.Infrastructure.Entities;

[Index(nameof(TestRunId), nameof(BatteryChannel), nameof(TimeSec))]
public class MeasurementLog
{
    [Key]
    public long Id { get; set; }

    public long TestRunId { get; set; }

    [ForeignKey(nameof(TestRunId))]
    public TestRun TestRun { get; set; } = null!;

    public int BatteryChannel { get; set; }

    public int TimeSec { get; set; }

    public double? Voltage { get; set; }

    public double? Current { get; set; }

    public double? Temperature { get; set; }
}
