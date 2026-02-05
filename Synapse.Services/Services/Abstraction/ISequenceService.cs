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
    }
}
