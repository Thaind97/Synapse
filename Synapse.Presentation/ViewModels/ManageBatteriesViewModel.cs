using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Synapse.Infrastructure.Entities;
using Synapse.Services.Services.Abstraction;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Synapse.Presentation.ViewModels
{
    public class ManageBatteriesViewModel : ObservableObject
    {
        private readonly ISequenceService _sequenceService;

        public ManageBatteriesViewModel(ISequenceService sequenceService)
        {
            _sequenceService = sequenceService;
            Batteries = new ObservableCollection<Battery>();
            Sequences = new ObservableCollection<Sequence>();
            Assignments = new ObservableCollection<SequenceAssignment>();

            AddBatteryCommand = new AsyncRelayCommand(AddBatteryAsync);
            DeleteBatteryCommand = new AsyncRelayCommand(DeleteBatteryAsync, () => SelectedBattery != null);
            AssignCommand = new AsyncRelayCommand(AssignAsync, () => SelectedBattery != null && SelectedSequence != null);
            UnassignCommand = new AsyncRelayCommand(UnassignAsync, () => SelectedAssignment != null);

            // Start loading data (fire-and-forget)
            _ = LoadDataAsync();
        }

        public ObservableCollection<Battery> Batteries { get; }
        public ObservableCollection<Sequence> Sequences { get; }
        public ObservableCollection<SequenceAssignment> Assignments { get; }

        private Battery? _selectedBattery;
        public Battery? SelectedBattery
        {
            get => _selectedBattery;
            set
            {
                if (SetProperty(ref _selectedBattery, value))
                {
                    DeleteBatteryCommand.NotifyCanExecuteChanged();
                    AssignCommand.NotifyCanExecuteChanged();
                }
            }
        }

        private Sequence? _selectedSequence;
        public Sequence? SelectedSequence
        {
            get => _selectedSequence;
            set
            {
                if (SetProperty(ref _selectedSequence, value))
                {
                    AssignCommand.NotifyCanExecuteChanged();
                }
            }
        }

        private SequenceAssignment? _selectedAssignment;
        public SequenceAssignment? SelectedAssignment
        {
            get => _selectedAssignment;
            set
            {
                if (SetProperty(ref _selectedAssignment, value))
                {
                    UnassignCommand.NotifyCanExecuteChanged();
                }
            }
        }

        public IAsyncRelayCommand AddBatteryCommand { get; }
        public IAsyncRelayCommand DeleteBatteryCommand { get; }
        public IAsyncRelayCommand AssignCommand { get; }
        public IAsyncRelayCommand UnassignCommand { get; }

        public async Task LoadDataAsync()
        {
            var beds = await _sequenceService.GetBatteriesAsync();
            Batteries.Clear();
            foreach (var b in beds) Batteries.Add(b);

            var seqs = await _sequenceService.GetAllSequencesAsync();
            Sequences.Clear();
            foreach (var s in seqs) Sequences.Add(s);

            var assigns = await _sequenceService.GetAssignmentsAsync();
            Assignments.Clear();
            foreach (var a in assigns) Assignments.Add(a);

            // select defaults
            if (Batteries.Count > 0 && SelectedBattery == null) SelectedBattery = Batteries[0];
            if (Sequences.Count > 0 && SelectedSequence == null) SelectedSequence = Sequences[0];
        }

        private async Task AddBatteryAsync()
        {
            var newB = new Battery { Channel = Batteries.Count + 1, Name = $"Battery {Batteries.Count + 1}" };
            await _sequenceService.CreateBatteryAsync(newB);
            Batteries.Add(newB);
            SelectedBattery = newB;
        }

        private async Task DeleteBatteryAsync()
        {
            if (SelectedBattery == null) return;
            await _sequenceService.DeleteBatteryAsync(SelectedBattery.Id);
            Batteries.Remove(SelectedBattery);
            SelectedBattery = Batteries.FirstOrDefault();
        }

        private async Task AssignAsync()
        {
            if (SelectedBattery == null || SelectedSequence == null) return;
            await _sequenceService.AssignSequenceToBatteryAsync(SelectedSequence.Id, SelectedBattery.Channel);
            await LoadDataAsync();
        }

        private async Task UnassignAsync()
        {
            if (SelectedAssignment == null) return;
            await _sequenceService.UnassignSequenceFromBatteryAsync(SelectedAssignment.SequenceId, SelectedAssignment.BatteryChannel);
            await LoadDataAsync();
        }
    }
}
