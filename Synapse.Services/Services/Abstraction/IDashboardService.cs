using Synapse.Services.Models;
using System.Threading.Tasks;

namespace Synapse.Services
{
    public interface IDashboardService
    {
        Task<DashboardData> GetDashboardDataAsync();
    }
}
