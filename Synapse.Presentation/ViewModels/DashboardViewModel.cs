using CommunityToolkit.Mvvm.ComponentModel;
using LiveCharts;
using LiveCharts.Wpf;
using Synapse.Services;
using Synapse.Services.Models;

namespace Synapse.Presentation.ViewModels
{
    public class DashboardViewModel : ObservableObject
    {
        private readonly IDashboardService _dashboardService;
        
        private DashboardData _data;
        public DashboardData Data
        {
            get => _data;
            set => SetProperty(ref _data, value);
        }

        public SeriesCollection SeriesCollection { get; set; }
        public List<string> Labels { get; set; }
        public ChartValues<int> AlarmValues { get; set; }

        public DashboardViewModel(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
            LoadDataAsync();
        }

        private async void LoadDataAsync()
        {
            Data = await _dashboardService.GetDashboardDataAsync();
            
            // Setup Chart
            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Solar",
                    Values = new ChartValues<double>(Data.PowerGenerationTrend),
                    PointGeometry = null
                },
                new LineSeries
                {
                    Title = "Storage",
                    Values = new ChartValues<double>(new List<double>{ 20, 30, 25, 40, 35, 50, 45, 60, 55, 70, 65, 80 }),
                    PointGeometry = null
                }
            };
            Labels = Data.ChartLabels;
            
            AlarmValues = new ChartValues<int> { Data.AlarmCount };
            OnPropertyChanged(nameof(SeriesCollection));
            OnPropertyChanged(nameof(Labels));
            OnPropertyChanged(nameof(AlarmValues));
        }
    }
}
