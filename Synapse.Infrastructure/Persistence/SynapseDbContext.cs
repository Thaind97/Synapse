using Microsoft.EntityFrameworkCore;
using Synapse.Infrastructure.Entities;
using Synapse.Shared;
namespace Synapse.Infrastructure.Persistence
{
    public class SynapseDbContext : BaseDbContext<SynapseDbContext>
    {
        private static readonly string FolderPath =
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        public DbSet<UserData> UserData => Set<UserData>();
        public DbSet<Sequence> Sequences => Set<Sequence>();
        public DbSet<SequenceStep> SequenceSteps => Set<SequenceStep>();
        public DbSet<DeviceCommand> DeviceCommands => Set<DeviceCommand>();
        public DbSet<Variable> Variables => Set<Variable>();
        public DbSet<CommandParameter> CommandParameters => Set<CommandParameter>();
        public DbSet<EndCondition> EndConditions => Set<EndCondition>();
        public DbSet<TestRun> TestRuns => Set<TestRun>();
        public DbSet<MeasurementLog> MeasurementLogs => Set<MeasurementLog>();
        public DbSet<SystemLog> SystemLogs => Set<SystemLog>();
        public DbSet<SequenceAssignment> SequenceAssignments => Set<SequenceAssignment>();
        public DbSet<Battery> Batteries => Set<Battery>();

        public SynapseDbContext(DbContextOptions<SynapseDbContext> options)
            : base(options)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine(
                $"{nameof(SynapseDbContext)} created - {GetHashCode()}");
#endif
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured)
                return;

            var dbFolder = Path.Combine(FolderPath, Const.APPLICATION_FOLDER_NAME);
            Directory.CreateDirectory(dbFolder);

            var dbPath = Path.Combine(dbFolder, Const.DATABASE_FILE_NAME);

            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<SequenceAssignment>(b =>
            {
                b.HasIndex(x => x.BatteryChannel).IsUnique();
                b.ToTable("SequenceAssignments");
            });
            builder.Entity<Battery>(b =>
            {
                b.HasIndex(x => x.Channel).IsUnique();
                b.ToTable("Batteries");
            });
        }
    }
}
