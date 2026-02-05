using Synapse.Service.Tasking.HttpClients;
using Synapse.Services.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Synapse.Services
{
    /// <summary>
    /// Dashboard Service that calls third-party API via TaskingHttpObject
    /// </summary>
    public class MockDashboardService : IDashboardService
    {
        private readonly TaskingHttpObject _taskingHttpObject;

        public MockDashboardService(TaskingHttpObject taskingHttpObject)
        {
            _taskingHttpObject = taskingHttpObject;
        }

        public async Task<DashboardData> GetDashboardDataAsync()
        {
            await Task.Delay(100); // Simulate network delay
            return new DashboardData
            {
                PowerGenerationTrend = new List<double> { 10, 15, 12, 18, 20, 25, 23, 30, 28, 35, 32, 40 },
                ChartLabels = new List<string> { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" },
                AlarmCount = 5,
                TopStats = new List<StatItem>
                {
                    new StatItem { Title = "Active Alarms", Value = "12", TrendValue = "2" },
                    new StatItem { Title = "Total Capacity (GWp)", Value = "45.2", TrendValue = "0.5" },
                    new StatItem { Title = "Daily Generation", Value = "234", TrendValue = "12" },
                    new StatItem { Title = "Revenue", Value = "$12,430", TrendValue = "$450" }
                },
                RunningStatuses = new List<StatusItem>
                {
                    new StatusItem { Title = "ESS Status", Value = "Normal", Details = new List<StatusDetail> { new StatusDetail { Label = "SOC", Value = "85%" }, new StatusDetail { Label = "Power", Value = "1.2MW" } } },
                    new StatusItem { Title = "Inverter Status", Value = "Running", Details = new List<StatusDetail> { new StatusDetail { Label = "Active", Value = "124" }, new StatusDetail { Label = "Idle", Value = "2" } } },
                    new StatusItem { Title = "Grid Status", Value = "Online", Details = new List<StatusDetail> { new StatusDetail { Label = "Freq", Value = "60.0Hz" }, new StatusDetail { Label = "Volt", Value = "110kV" } } }
                },
                Rankings = new List<string> { "Site Alpha", "Site Beta", "Site Gamma", "Site Delta" }
            };
        }
    }
}
