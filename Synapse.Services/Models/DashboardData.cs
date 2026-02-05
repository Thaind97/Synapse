using System.Collections.Generic;

namespace Synapse.Services.Models
{
    public class DashboardData
    {
        public List<double> PowerGenerationTrend { get; set; } = new();
        public List<string> ChartLabels { get; set; } = new();
        public int AlarmCount { get; set; }
        
        public List<StatItem> TopStats { get; set; } = new();
        public List<StatusItem> RunningStatuses { get; set; } = new();
        public List<string> Rankings { get; set; } = new();
    }
}
