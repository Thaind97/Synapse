using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Synapse.Infrastructure.Entities;
using Synapse.Services.Services.Abstraction;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows;
using Synapse.Shared.Constants;

namespace Synapse.Presentation.ViewModels
{
    public class SequenceEditorViewModel : ObservableObject
    {
        private readonly ISequenceService _sequenceService;
        private readonly IRunManager _runManager;

        private readonly HashSet<long> _pendingSequenceDeletes = new();
        private readonly HashSet<long> _pendingStepDeletes = new();
        private readonly HashSet<long> _pendingCommandDeletes = new();

        public SequenceEditorViewModel(ISequenceService sequenceService, IRunManager runManager)
        {
            _sequenceService = sequenceService;
            _runManager = runManager;

            Sequences = new ObservableCollection<Sequence>();
            Steps = new ObservableCollection<SequenceStep>();
            Commands = new ObservableCollection<DeviceCommand>();
            Parameters = new ObservableCollection<CommandParameter>();

            LoadSequencesCommand = new AsyncRelayCommand(LoadSequencesAsync);

            // Sequence Commands
            AddSequenceCommand = new AsyncRelayCommand(AddSequenceAsync);
            DeleteSequenceCommand = new AsyncRelayCommand(DeleteSequenceAsync, () => SelectedSequence != null);

            // Step Commands
            AddStepCommand = new AsyncRelayCommand(AddStepAsync, () => SelectedSequence != null);
            DeleteStepCommand = new AsyncRelayCommand(DeleteStepAsync, () => SelectedStep != null);
            MoveStepUpCommand = new RelayCommand(MoveStepUp, () => SelectedStep != null);
            MoveStepDownCommand = new RelayCommand(MoveStepDown, () => SelectedStep != null);

            // Device Command Commands
            AddDeviceCommandCommand = new AsyncRelayCommand(AddDeviceCommandAsync, CanEditCommands);
            AddWaitCommandCommand = new AsyncRelayCommand(AddWaitCommandAsync, CanEditCommands);
            AddLoopCommandCommand = new AsyncRelayCommand(AddLoopCommandAsync, CanEditCommands);
            DeleteDeviceCommandCommand = new AsyncRelayCommand(DeleteCommandAsync, () => CanEditCommands() && SelectedCommand != null);
            MoveCommandUpCommand = new RelayCommand(MoveCommandUp, () => CanEditCommands() && SelectedCommand != null);
            MoveCommandDownCommand = new RelayCommand(MoveCommandDown, () => CanEditCommands() && SelectedCommand != null);

            SaveChangesCommand = new AsyncRelayCommand(SaveChangesAsync);

            // Run commands
            StartRunCommand = new AsyncRelayCommand(StartRunAsync, () => SelectedSequence != null && !string.IsNullOrWhiteSpace(BatteryId));
            StopRunCommand = new AsyncRelayCommand(StopRunAsync, () => !string.IsNullOrWhiteSpace(BatteryId));

            // Default START/END when no sequence selected
            ShowDefaultSteps();
            InitializeDefaultBattery();
        }

        #region Properties

        public ObservableCollection<Sequence> Sequences { get; }
        public ObservableCollection<SequenceStep> Steps { get; }
        public ObservableCollection<DeviceCommand> Commands { get; }
        public ObservableCollection<CommandParameter> Parameters { get; }

        public List<string> Devices { get; } = new() { "Power Source 1", "Datalogger", "Multimeter", "Electronic Load" };
        public List<string> AvailableCommands { get; } = new() { "CCCV Charge", "Wait", "Set Voltage", "Set Current", "Start Log" };

        private Sequence? _selectedSequence;
        public Sequence? SelectedSequence
        {
            get => _selectedSequence;
            set
            {
                if (SetProperty(ref _selectedSequence, value))
                {
                    RefreshSteps();
                    NotifySequenceCommands();
                }
            }
        }

        private SequenceStep? _selectedStep;
        public SequenceStep? SelectedStep
        {
            get => _selectedStep;
            set
            {
                if (SetProperty(ref _selectedStep, value))
                {
                    RefreshCommands();
                    NotifyStepCommands();
                }
            }
        }

        private DeviceCommand? _selectedCommand;
        public DeviceCommand? SelectedCommand
        {
            get => _selectedCommand;
            set
            {
                if (SetProperty(ref _selectedCommand, value))
                {
                    RefreshParameters();
                    NotifyDeviceCommandCommands();
                }
            }
        }
        
        private CommandParameter? _selectedParameter;
        public CommandParameter? SelectedParameter
        {
            get => _selectedParameter;
            set => SetProperty(ref _selectedParameter, value);
        }

        private string _batteryId = string.Empty;
        public string BatteryId
        {
            get => _batteryId;
            set
            {
                if (SetProperty(ref _batteryId, value))
                {
                    StartRunCommand.NotifyCanExecuteChanged();
                    StopRunCommand.NotifyCanExecuteChanged();
                }
            }
        }

        public ObservableCollection<string> BatteryList { get; } = new ObservableCollection<string>(Enumerable.Range(1,24).Select(i => i.ToString()));

        private string _selectedBattery;
        public string SelectedBattery
        {
            get => _selectedBattery;
            set => SetProperty(ref _selectedBattery, value);
        }

        private void InitializeDefaultBattery()
        {
            if (BatteryList.Count > 0 && string.IsNullOrEmpty(SelectedBattery))
                SelectedBattery = BatteryList[0];
        }
        #endregion

        #region Commands

        public IAsyncRelayCommand LoadSequencesCommand { get; }
        public IAsyncRelayCommand AddSequenceCommand { get; }
        public IAsyncRelayCommand DeleteSequenceCommand { get; }
        
        public IAsyncRelayCommand AddStepCommand { get; }
        public IAsyncRelayCommand DeleteStepCommand { get; }
        public IRelayCommand MoveStepUpCommand { get; }
        public IRelayCommand MoveStepDownCommand { get; }

        public IAsyncRelayCommand AddDeviceCommandCommand { get; }
        public IAsyncRelayCommand AddWaitCommandCommand { get; }
        public IAsyncRelayCommand AddLoopCommandCommand { get; }
        public IAsyncRelayCommand DeleteDeviceCommandCommand { get; }
        public IRelayCommand MoveCommandUpCommand { get; }
        public IRelayCommand MoveCommandDownCommand { get; }
        
        public IAsyncRelayCommand SaveChangesCommand { get; }
        public IAsyncRelayCommand StartRunCommand { get; }
        public IAsyncRelayCommand StopRunCommand { get; }

        #endregion

        #region Methods

        public async Task LoadSequencesAsync()
        {
            var seqs = await _sequenceService.GetAllSequencesAsync();
            Sequences.Clear();
            if (seqs != null)
            {
                foreach (var s in seqs) Sequences.Add(s);
            }

            if (Sequences.Count > 0)
            {
                if (SelectedSequence == null)
                {
                    SelectedSequence = Sequences[0];
                }
            }
            else
            {
                ShowDefaultSteps();
            }
        }

        private void ShowDefaultSteps()
        {
            Steps.Clear();
            Commands.Clear();
            Parameters.Clear();

            Steps.Add(new SequenceStep { StepOrder = int.MinValue, StepType = SequenceConstants.StepTypeStart, StepName = SequenceConstants.StepTypeStart });
            Steps.Add(new SequenceStep { StepOrder = int.MaxValue, StepType = SequenceConstants.StepTypeEnd, StepName = SequenceConstants.StepTypeEnd });
        }

        private void RefreshSteps()
        {
            Steps.Clear();
            Commands.Clear();
            Parameters.Clear();

            if (SelectedSequence?.Steps == null)
            {
                ShowDefaultSteps();
                return;
            }

            var sortedSteps = SelectedSequence.Steps.OrderBy(x => x.StepOrder).ToList();

            var startStep = sortedSteps.FirstOrDefault(s => s.StepType == SequenceConstants.StepTypeStart)
                ?? new SequenceStep { StepOrder = int.MinValue, StepType = SequenceConstants.StepTypeStart, StepName = SequenceConstants.StepTypeStart };
            var endStep = sortedSteps.LastOrDefault(s => s.StepType == SequenceConstants.StepTypeEnd)
                ?? new SequenceStep { StepOrder = int.MaxValue, StepType = SequenceConstants.StepTypeEnd, StepName = SequenceConstants.StepTypeEnd };

            Steps.Add(startStep);

            foreach (var s in sortedSteps.Where(s => s.StepType != SequenceConstants.StepTypeStart && s.StepType != SequenceConstants.StepTypeEnd))
            {
                Steps.Add(s);
            }

            Steps.Add(endStep);
        }

        private bool CanEditCommands()
        {
            return SelectedStep != null && SelectedStep.StepType != SequenceConstants.StepTypeStart && SelectedStep.StepType != SequenceConstants.StepTypeEnd;
        }

        private void RefreshCommands()
        {
            Commands.Clear();
            Parameters.Clear();

            if (SelectedStep == null)
            {
                return;
            }

            // Always show START/END placeholders in commands column
            Commands.Add(new DeviceCommand
            {
                Command = SequenceConstants.StepTypeStart,
                Device = SequenceConstants.SystemDevice
            });

            if (SelectedStep.StepType != SequenceConstants.StepTypeStart && SelectedStep.StepType != SequenceConstants.StepTypeEnd)
            {
                if (SelectedStep.DeviceCommands != null)
                {
                    var allCommands = SelectedStep.DeviceCommands.ToList();

                    foreach (var cmd in allCommands)
                    {
                        cmd.ChildCommands.Clear();
                    }

                    foreach (var cmd in allCommands.Where(c => c.ParentCommandId != null))
                    {
                        var parent = allCommands.FirstOrDefault(c => c.Id == cmd.ParentCommandId);
                        parent?.ChildCommands.Add(cmd);
                    }

                    var rootCommands = allCommands.Where(c => c.ParentCommandId == null).ToList();

                    foreach (var cmd in rootCommands)
                    {
                        Commands.Add(cmd);
                    }
                }
            }

            Commands.Add(new DeviceCommand
            {
                Command = SequenceConstants.StepTypeEnd,
                Device = SequenceConstants.SystemDevice
            });
        }

        private void RefreshParameters()
        {
            Parameters.Clear();
            if (SelectedCommand?.CommandParameters == null) return;

            foreach (var p in SelectedCommand.CommandParameters) Parameters.Add(p);
        }

        private async Task AddSequenceAsync()
        {
            var newSeq = new Sequence
            {
                Name = $"New Sequence {Sequences.Count + 1}",
                CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            
            // Add Start and End steps
            newSeq.Steps.Add(new SequenceStep { StepOrder = 0, StepType = SequenceConstants.StepTypeStart, StepName = SequenceConstants.StepTypeStart, Sequence = newSeq });
            newSeq.Steps.Add(new SequenceStep { StepOrder = 1, StepType = SequenceConstants.StepTypeEnd, StepName = SequenceConstants.StepTypeEnd, Sequence = newSeq });

            Sequences.Add(newSeq);
            SelectedSequence = newSeq;
            await Task.CompletedTask;
        }

        private async Task DeleteSequenceAsync()
        {
            if (SelectedSequence == null) return;

            var seq = SelectedSequence;
            if (seq.Id > 0)
            {
                _pendingSequenceDeletes.Add(seq.Id);
            }

            Sequences.Remove(seq);
            SelectedSequence = Sequences.FirstOrDefault();
            await Task.CompletedTask;
        }

        private async Task AddStepAsync()
        {
            if (SelectedSequence == null) return;

            // Find index of END
            var endStep = SelectedSequence.Steps.FirstOrDefault(s => s.StepType == SequenceConstants.StepTypeEnd);
            int order = endStep != null ? endStep.StepOrder : SelectedSequence.Steps.Count;

            // Shift End
            if (endStep != null)
            {
                endStep.StepOrder++;
            }

            var newStep = new SequenceStep
            {
                Sequence = SelectedSequence,
                SequenceId = SelectedSequence.Id,
                StepOrder = order,
                StepType = "PARTITION",
                StepName = "New Step"
            };

            SelectedSequence.Steps.Add(newStep);
            
            RefreshSteps();
            SelectedStep = newStep;
            await Task.CompletedTask;
        }

        private async Task DeleteStepAsync()
        {
            if (SelectedStep == null || SelectedSequence == null) return;
            if (SelectedStep.StepType == SequenceConstants.StepTypeStart || SelectedStep.StepType == SequenceConstants.StepTypeEnd) return; // Prevent deleting start/end?

            if (SelectedStep.DeviceCommands != null)
            {
                foreach (var cmd in SelectedStep.DeviceCommands.Where(c => c.Id > 0))
                {
                    _pendingCommandDeletes.Add(cmd.Id);
                }
            }

            if (SelectedStep.Id > 0)
            {
                _pendingStepDeletes.Add(SelectedStep.Id);
            }

            SelectedSequence.Steps.Remove(SelectedStep);
            RefreshSteps();
            SelectedStep = null;
            await Task.CompletedTask;
        }

        private async void MoveStepUp()
        {
            if (SelectedStep == null || SelectedSequence == null) return;
            if (SelectedStep.StepType == SequenceConstants.StepTypeStart || SelectedStep.StepType == SequenceConstants.StepTypeEnd) return;

            var list = Steps.ToList();
            var index = list.IndexOf(SelectedStep);
            if (index <= 1) return; // Cannot move above START (index 0)

            var prev = list[index - 1];
            if (prev.StepType == SequenceConstants.StepTypeStart) return;

            // Swap Orders
            (SelectedStep.StepOrder, prev.StepOrder) = (prev.StepOrder, SelectedStep.StepOrder);

            // Refesh UI list
            var newIndex = index - 1;
            Steps.Move(index, newIndex);
        }

        private async void MoveStepDown()
        {
            if (SelectedStep == null || SelectedSequence == null) return;
            if (SelectedStep.StepType == SequenceConstants.StepTypeStart || SelectedStep.StepType == SequenceConstants.StepTypeEnd) return;

            var list = Steps.ToList();
            var index = list.IndexOf(SelectedStep);
            if (index >= list.Count - 2) return; // Cannot move below END (last index)

            // Swap with next
            var next = list[index + 1];
            if (next.StepType == SequenceConstants.StepTypeEnd) return;

            // Swap Orders
            (SelectedStep.StepOrder, next.StepOrder) = (next.StepOrder, SelectedStep.StepOrder);

            // Refesh UI list
            var newIndex = index + 1;
            Steps.Move(index, newIndex);
        }

        private async void MoveCommandUp() 
        { 
             if (SelectedCommand == null || SelectedStep == null) return;
             if (SelectedCommand.Command == SequenceConstants.StepTypeStart || SelectedCommand.Command == SequenceConstants.StepTypeEnd) return;

             var ordered = SelectedStep.DeviceCommands.ToList();
             var siblings = ordered.Where(c => c.ParentCommandId == SelectedCommand.ParentCommandId).ToList();

             var index = siblings.IndexOf(SelectedCommand);
             if (index <= 0) return;

             var prev = siblings[index - 1];
             var currentIndex = ordered.IndexOf(SelectedCommand);
             var prevIndex = ordered.IndexOf(prev);

             ordered.RemoveAt(currentIndex);
             ordered.Insert(prevIndex, SelectedCommand);

             SelectedStep.DeviceCommands.Clear();
             foreach (var cmd in ordered) SelectedStep.DeviceCommands.Add(cmd);

             RefreshCommands();
        }
        
        private void MoveCommandDown() 
        { 
             if (SelectedCommand == null || SelectedStep == null) return;
             if (SelectedCommand.Command == SequenceConstants.StepTypeStart || SelectedCommand.Command == SequenceConstants.StepTypeEnd) return;

             var ordered = SelectedStep.DeviceCommands.ToList();
             var siblings = ordered.Where(c => c.ParentCommandId == SelectedCommand.ParentCommandId).ToList();

             var index = siblings.IndexOf(SelectedCommand);
             if (index < 0 || index >= siblings.Count - 1) return;

             var next = siblings[index + 1];
             var currentIndex = ordered.IndexOf(SelectedCommand);
             var nextIndex = ordered.IndexOf(next);

             ordered.RemoveAt(currentIndex);
             ordered.Insert(nextIndex, SelectedCommand);

             SelectedStep.DeviceCommands.Clear();
             foreach (var cmd in ordered) SelectedStep.DeviceCommands.Add(cmd);

             RefreshCommands();
        }

        private async Task AddGenericCommandAsync(string cmdType, string cmdName)
        {
            if (SelectedStep == null) return;

            var newCmd = new DeviceCommand
            {
                Step = SelectedStep,
                StepId = SelectedStep.Id,
                Device = cmdType,
                Command = cmdName
            };

            long? parentLoopId = null;
            if (SelectedCommand?.Command == SequenceConstants.CommandLoop)
            {
                parentLoopId = SelectedCommand.Id;
            }
            else if (SelectedCommand?.ParentCommandId != null)
            {
                var parent = SelectedStep.DeviceCommands.FirstOrDefault(c => c.Id == SelectedCommand.ParentCommandId);
                if (parent?.Command == SequenceConstants.CommandLoop)
                {
                    parentLoopId = parent.Id;
                }
            }

            if (parentLoopId != null)
            {
                newCmd.ParentCommandId = parentLoopId;
            }

            if (cmdType == SequenceConstants.CommandWait)
            {
                newCmd.CommandParameters.Add(new CommandParameter { Name = "Wait Time", Value = "10", Unit = "s" });
            }
            else if (cmdType == SequenceConstants.CommandDevice)
            {
                newCmd.CommandParameters.Add(new CommandParameter { Name = "Voltage", Value = "30", Unit = "V" });
                newCmd.CommandParameters.Add(new CommandParameter { Name = "Current", Value = "5", Unit = "A" });
            }

            SelectedStep.DeviceCommands.Add(newCmd);
            RefreshCommands();
            SelectedCommand = newCmd;
            await Task.CompletedTask;
        }

        private async Task AddDeviceCommandAsync() => await AddGenericCommandAsync(SequenceConstants.CommandDevice, "NEW COMMAND");
        private async Task AddWaitCommandAsync() => await AddGenericCommandAsync(SequenceConstants.CommandWait, "WAIT 10s");

        private async Task AddLoopCommandAsync()
        {
            if (SelectedStep == null) return;

            var loopCmd = new DeviceCommand
            {
                Step = SelectedStep,
                StepId = SelectedStep.Id,
                Device = SequenceConstants.CommandLoop,
                Command = SequenceConstants.CommandLoop
            };

            loopCmd.CommandParameters.Add(new CommandParameter
            {
                Name = SequenceConstants.LoopCountParameterName,
                Value = "1",
                Unit = "times"
            });

            SelectedStep.DeviceCommands.Add(loopCmd);
            RefreshCommands();
            SelectedCommand = loopCmd;
            await Task.CompletedTask;
        }

        private async Task DeleteCommandAsync()
        {
            if (SelectedCommand == null || SelectedStep == null) return;

            var toRemove = SelectedStep.DeviceCommands
                .Where(c => c.Id == SelectedCommand.Id || c.ParentCommandId == SelectedCommand.Id)
                .ToList();

            foreach (var cmd in toRemove)
            {
                if (cmd.Id > 0)
                {
                    _pendingCommandDeletes.Add(cmd.Id);
                }

                SelectedStep.DeviceCommands.Remove(cmd);
            }

            RefreshCommands();
            SelectedCommand = null;
            await Task.CompletedTask;
        }

        private async Task SaveChangesAsync()
        {
            try
            {
                await SavePendingDeletesAsync();

                foreach (var sequence in Sequences)
                {
                    sequence.UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    if (sequence.Id == 0)
                    {
                        await _sequenceService.CreateSequenceAsync(sequence);
                    }
                    else
                    {
                        await _sequenceService.UpdateSequenceAsync(sequence);
                    }
                }

                _pendingCommandDeletes.Clear();
                _pendingStepDeletes.Clear();
                _pendingSequenceDeletes.Clear();

                MessageBox.Show("Lưu sequence thành công.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lưu sequence thất bại: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task SavePendingDeletesAsync()
        {
            foreach (var commandId in _pendingCommandDeletes)
            {
                await _sequenceService.DeleteCommandAsync(commandId);
            }

            foreach (var stepId in _pendingStepDeletes)
            {
                await _sequenceService.DeleteStepAsync(stepId);
            }

            foreach (var sequenceId in _pendingSequenceDeletes)
            {
                await _sequenceService.DeleteSequenceAsync(sequenceId);
            }
        }

        private async Task StartRunAsync()
        {
            if (SelectedSequence == null || string.IsNullOrWhiteSpace(BatteryId)) return;
            await _runManager.EnqueueRunAsync(SelectedSequence.Id, BatteryId);
        }

        private async Task StopRunAsync()
        {
            if (string.IsNullOrWhiteSpace(BatteryId)) return;
            await _runManager.StopRunAsync(BatteryId);
        }

        private void NotifySequenceCommands()
        {
            DeleteSequenceCommand.NotifyCanExecuteChanged();
            AddStepCommand.NotifyCanExecuteChanged();
            StartRunCommand?.NotifyCanExecuteChanged();
            StopRunCommand?.NotifyCanExecuteChanged();
        }

        private void NotifyStepCommands()
        {
            DeleteStepCommand.NotifyCanExecuteChanged();
            MoveStepUpCommand.NotifyCanExecuteChanged();
            MoveStepDownCommand.NotifyCanExecuteChanged();
            AddDeviceCommandCommand.NotifyCanExecuteChanged();
            AddWaitCommandCommand.NotifyCanExecuteChanged();
            AddLoopCommandCommand.NotifyCanExecuteChanged();
        }

        private void NotifyDeviceCommandCommands()
        {
            DeleteDeviceCommandCommand.NotifyCanExecuteChanged();
            MoveCommandUpCommand.NotifyCanExecuteChanged();
            MoveCommandDownCommand.NotifyCanExecuteChanged();
        }

        #endregion
    }
}
