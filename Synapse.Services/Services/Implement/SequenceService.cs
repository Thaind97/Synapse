using Microsoft.EntityFrameworkCore;
using Synapse.Infrastructure;
using Synapse.Infrastructure.Entities;
using Synapse.Services.Services.Abstraction;

namespace Synapse.Services.Services.Implement
{
    public class SequenceService : ISequenceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SequenceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<Sequence>> GetAllSequencesAsync()
        {
            var context = _unitOfWork.DbContext;
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
            var context = _unitOfWork.DbContext;
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
            _unitOfWork.DbContext.ChangeTracker.Clear();
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var context = _unitOfWork.DbContext;
                context.Sequences.Add(sequence);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitAsync();
                return sequence;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateSequenceAsync(Sequence sequence)
        {
            _unitOfWork.DbContext.ChangeTracker.Clear();
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var context = _unitOfWork.DbContext;
                context.Sequences.Update(sequence);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteSequenceAsync(long id)
        {
            _unitOfWork.DbContext.ChangeTracker.Clear();
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var context = _unitOfWork.DbContext;
                var seq = await context.Sequences.FindAsync(id);
                if (seq != null)
                {
                    context.Sequences.Remove(seq);
                    await _unitOfWork.SaveAsync();
                }

                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<SequenceStep> AddStepAsync(SequenceStep step)
        {
            _unitOfWork.DbContext.ChangeTracker.Clear();
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var context = _unitOfWork.DbContext;
                context.SequenceSteps.Add(step);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitAsync();
                return step;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateStepAsync(SequenceStep step)
        {
            _unitOfWork.DbContext.ChangeTracker.Clear();
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var context = _unitOfWork.DbContext;
                context.SequenceSteps.Update(step);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteStepAsync(long id)
        {
            _unitOfWork.DbContext.ChangeTracker.Clear();
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var context = _unitOfWork.DbContext;
                var step = await context.SequenceSteps.FindAsync(id);
                if (step != null)
                {
                    context.SequenceSteps.Remove(step);
                    await _unitOfWork.SaveAsync();
                }

                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<DeviceCommand> AddCommandAsync(DeviceCommand command)
        {
            _unitOfWork.DbContext.ChangeTracker.Clear();
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var context = _unitOfWork.DbContext;
                context.DeviceCommands.Add(command);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitAsync();
                return command;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateCommandAsync(DeviceCommand command)
        {
            _unitOfWork.DbContext.ChangeTracker.Clear();
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var context = _unitOfWork.DbContext;
                context.DeviceCommands.Update(command);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteCommandAsync(long id)
        {
            _unitOfWork.DbContext.ChangeTracker.Clear();
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var context = _unitOfWork.DbContext;
                var cmd = await context.DeviceCommands.FindAsync(id);
                if (cmd != null)
                {
                    context.DeviceCommands.Remove(cmd);
                    await _unitOfWork.SaveAsync();
                }

                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task AssignSequenceToBatteryAsync(long sequenceId, int batteryChannel)
        {
            _unitOfWork.DbContext.ChangeTracker.Clear();
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var context = _unitOfWork.DbContext;
                var exist = await context.SequenceAssignments.FirstOrDefaultAsync(a => a.BatteryChannel == batteryChannel);
                if (exist != null)
                {
                    context.SequenceAssignments.Remove(exist);
                }

                var battery = await context.Batteries.FirstOrDefaultAsync(b => b.Channel == batteryChannel);
                var assign = new SequenceAssignment { SequenceId = sequenceId, BatteryChannel = batteryChannel, AssignedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") };
                if (battery != null)
                {
                    assign.BatteryId = battery.Id;
                }

                context.SequenceAssignments.Add(assign);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task UnassignSequenceFromBatteryAsync(long sequenceId, int batteryChannel)
        {
            _unitOfWork.DbContext.ChangeTracker.Clear();
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var context = _unitOfWork.DbContext;
                var exist = await context.SequenceAssignments.FirstOrDefaultAsync(a => a.SequenceId == sequenceId && a.BatteryChannel == batteryChannel);
                if (exist != null)
                {
                    context.SequenceAssignments.Remove(exist);
                    await _unitOfWork.SaveAsync();
                }

                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<List<SequenceAssignment>> GetAssignmentsAsync()
        {
            var context = _unitOfWork.DbContext;
            return await context.SequenceAssignments.Include(a => a.Battery).AsNoTracking().ToListAsync();
        }

        public async Task<List<Battery>> GetBatteriesAsync()
        {
            var context = _unitOfWork.DbContext;
            return await context.Batteries.AsNoTracking().OrderBy(b => b.Channel).ToListAsync();
        }

        public async Task<Battery> CreateBatteryAsync(Battery battery)
        {
            _unitOfWork.DbContext.ChangeTracker.Clear();
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var context = _unitOfWork.DbContext;
                context.Batteries.Add(battery);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitAsync();
                return battery;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateBatteryAsync(Battery battery)
        {
            _unitOfWork.DbContext.ChangeTracker.Clear();
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var context = _unitOfWork.DbContext;
                context.Batteries.Update(battery);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteBatteryAsync(long batteryId)
        {
            _unitOfWork.DbContext.ChangeTracker.Clear();
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var context = _unitOfWork.DbContext;
                var b = await context.Batteries.FindAsync(batteryId);
                if (b != null)
                {
                    context.Batteries.Remove(b);
                    await _unitOfWork.SaveAsync();
                }

                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }
    }
}
