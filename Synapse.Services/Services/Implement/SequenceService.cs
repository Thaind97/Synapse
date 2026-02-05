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

        public async Task AssignSequenceToBatteryAsync(long sequenceId, int batteryChannel)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            // Remove existing assignment for that battery
            var exist = await context.SequenceAssignments.FirstOrDefaultAsync(a => a.BatteryChannel == batteryChannel);
            if (exist != null)
            {
                context.SequenceAssignments.Remove(exist);
            }

            // Try to find battery master record
            var battery = await context.Batteries.FirstOrDefaultAsync(b => b.Channel == batteryChannel);
            var assign = new SequenceAssignment { SequenceId = sequenceId, BatteryChannel = batteryChannel, AssignedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") };
            if (battery != null)
            {
                assign.BatteryId = battery.Id;
            }

            context.SequenceAssignments.Add(assign);
            await context.SaveChangesAsync();
        }

        public async Task UnassignSequenceFromBatteryAsync(long sequenceId, int batteryChannel)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var exist = await context.SequenceAssignments.FirstOrDefaultAsync(a => a.SequenceId == sequenceId && a.BatteryChannel == batteryChannel);
            if (exist != null)
            {
                context.SequenceAssignments.Remove(exist);
                await context.SaveChangesAsync();
            }
        }

        public async Task<List<SequenceAssignment>> GetAssignmentsAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.SequenceAssignments.Include(a => a.Battery).AsNoTracking().ToListAsync();
        }

        public async Task<List<Battery>> GetBatteriesAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Batteries.AsNoTracking().OrderBy(b => b.Channel).ToListAsync();
        }

        public async Task<Battery> CreateBatteryAsync(Battery battery)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            context.Batteries.Add(battery);
            await context.SaveChangesAsync();
            return battery;
        }

        public async Task UpdateBatteryAsync(Battery battery)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            context.Batteries.Update(battery);
            await context.SaveChangesAsync();
        }

        public async Task DeleteBatteryAsync(long batteryId)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var b = await context.Batteries.FindAsync(batteryId);
            if (b != null)
            {
                context.Batteries.Remove(b);
                await context.SaveChangesAsync();
            }
        }
    }
}
