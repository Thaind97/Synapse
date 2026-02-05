using Synapse.Infrastructure.Entities;

namespace Synapse.Services.Services.Abstraction
{
    public interface ISequenceService
    {
        Task<List<Sequence>> GetAllSequencesAsync();
        Task<Sequence?> GetSequenceByIdAsync(long id);
        Task<Sequence> CreateSequenceAsync(Sequence sequence);
        Task UpdateSequenceAsync(Sequence sequence);
        Task DeleteSequenceAsync(long id);
        
        Task<SequenceStep> AddStepAsync(SequenceStep step);
        Task UpdateStepAsync(SequenceStep step);
        Task DeleteStepAsync(long id);
        
        Task<DeviceCommand> AddCommandAsync(DeviceCommand command);
        Task UpdateCommandAsync(DeviceCommand command);
        Task DeleteCommandAsync(long id);

        // Assignment
        Task AssignSequenceToBatteryAsync(long sequenceId, int batteryChannel);
        Task UnassignSequenceFromBatteryAsync(long sequenceId, int batteryChannel);
        Task<List<SequenceAssignment>> GetAssignmentsAsync();

        // Battery master data
        Task<List<Battery>> GetBatteriesAsync();
        Task<Battery> CreateBatteryAsync(Battery battery);
        Task UpdateBatteryAsync(Battery battery);
        Task DeleteBatteryAsync(long batteryId);
    }
}
