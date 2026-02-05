using Microsoft.EntityFrameworkCore;
using Synapse.Infrastructure.Entities;
using Synapse.Infrastructure.Persistence;
using Synapse.Services.Services.Abstraction;

namespace Synapse.Services.Services.Implement
{
    public class SequenceService : ISequenceService
    {
        private readonly IDbContextFactory<SynapseDbContext> _contextFactory;

        public SequenceService(IDbContextFactory<SynapseDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<Sequence>> GetAllSequencesAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Sequences
                .Include(s => s.Steps)
                .ThenInclude(st => st.DeviceCommands)
                .ThenInclude(c => c.CommandParameters)
                .ThenInclude(p => p.Variable)
                .Include(s => s.Steps)
                .ThenInclude(st => st.EndConditions)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Sequence?> GetSequenceByIdAsync(long id)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Sequences
                .Include(s => s.Steps)
                .ThenInclude(st => st.DeviceCommands)
                .ThenInclude(c => c.CommandParameters)
                .Include(s => s.Steps)
                .ThenInclude(st => st.EndConditions)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Sequence> CreateSequenceAsync(Sequence sequence)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            context.Sequences.Add(sequence);
            await context.SaveChangesAsync();
            return sequence;
        }

        public async Task UpdateSequenceAsync(Sequence sequence)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            context.Sequences.Update(sequence);
            await context.SaveChangesAsync();
        }

        public async Task DeleteSequenceAsync(long id)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var seq = await context.Sequences.FindAsync(id);
            if (seq != null)
            {
                context.Sequences.Remove(seq);
                await context.SaveChangesAsync();
            }
        }

        public async Task<SequenceStep> AddStepAsync(SequenceStep step)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            context.SequenceSteps.Add(step);
            await context.SaveChangesAsync();
            return step;
        }

        public async Task UpdateStepAsync(SequenceStep step)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            context.SequenceSteps.Update(step);
            await context.SaveChangesAsync();
        }

        public async Task DeleteStepAsync(long id)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var step = await context.SequenceSteps.FindAsync(id);
            if (step != null)
            {
                context.SequenceSteps.Remove(step);
                await context.SaveChangesAsync();
            }
        }

        public async Task<DeviceCommand> AddCommandAsync(DeviceCommand command)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            context.DeviceCommands.Add(command);
            await context.SaveChangesAsync();
            return command;
        }

        public async Task UpdateCommandAsync(DeviceCommand command)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            context.DeviceCommands.Update(command);
            await context.SaveChangesAsync();
        }

        public async Task DeleteCommandAsync(long id)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var cmd = await context.DeviceCommands.FindAsync(id);
            if (cmd != null)
            {
                context.DeviceCommands.Remove(cmd);
                await context.SaveChangesAsync();
            }
        }
    }
}
