using Microsoft.EntityFrameworkCore;
using Synapse.Infrastructure.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Synapse.Infrastructure.Persistence
{
    public static class CreateDatabase
    {
        public static void EnsureDatabaseCreated()
        {
            var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var dbFolder = Path.Combine(folderPath, Synapse.Shared.Const.APPLICATION_FOLDER_NAME);
            Directory.CreateDirectory(dbFolder);
            var dbPath = Path.Combine(dbFolder, Synapse.Shared.Const.DATABASE_FILE_NAME);

            var options = new DbContextOptionsBuilder<SynapseDbContext>()
                .UseSqlite($"Data Source={dbPath}")
                .Options;

            using var context = new SynapseDbContext(options);
            context.Database.EnsureCreated();

            EnsureLatestTables(context);
            EnsureDeviceCommandsHasParentColumn(context);

            // Seed admin user if not exists
            if (!context.UserData.Any(u => u.Username == "admin"))
            {
                context.UserData.Add(new UserData
                {
                    Username = "admin",
                    Password = "admin123"
                });
                context.SaveChanges();
            }

            SeedBatteries(context);
        }

        private static void EnsureLatestTables(SynapseDbContext context)
        {
            using var connection = context.Database.GetDbConnection();
            connection.Open();

            var existingTables = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT name FROM sqlite_master WHERE type='table';";
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    existingTables.Add(reader.GetString(0));
                }
            }

            var creator = context.GetService<IRelationalDatabaseCreator>();
            var script = creator.GenerateCreateScript();
            var statements = script.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            foreach (var statement in statements)
            {
                if (!statement.StartsWith("CREATE TABLE", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var tableName = TryGetTableName(statement);
                if (!string.IsNullOrWhiteSpace(tableName) && existingTables.Contains(tableName))
                {
                    continue;
                }

                context.Database.ExecuteSqlRaw(statement + ";");
            }
        }

        private static string? TryGetTableName(string commandText)
        {
            const string token = "CREATE TABLE \"";
            var startIndex = commandText.IndexOf(token, StringComparison.OrdinalIgnoreCase);
            if (startIndex < 0) return null;

            startIndex += token.Length;
            var endIndex = commandText.IndexOf('"', startIndex);
            if (endIndex <= startIndex) return null;

            return commandText[startIndex..endIndex];
        }

        private static void EnsureDeviceCommandsHasParentColumn(SynapseDbContext context)
        {
            using var connection = (SqliteConnection)context.Database.GetDbConnection();
            connection.Open();

            var hasParentCommandIdColumn = false;
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "PRAGMA table_info(DeviceCommands);";
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (reader.GetString(1) == "ParentCommandId")
                    {
                        hasParentCommandIdColumn = true;
                        break;
                    }
                }
            }

            if (!hasParentCommandIdColumn)
            {
                using var alterCommand = connection.CreateCommand();
                alterCommand.CommandText = "ALTER TABLE DeviceCommands ADD COLUMN ParentCommandId INTEGER NULL;";
                alterCommand.ExecuteNonQuery();
            }
        }

        private static void SeedBatteries(SynapseDbContext context)
        {
            var existingChannels = context.Batteries.Select(b => b.Channel).ToHashSet();
            var added = false;

            for (var channel = 1; channel <= 24; channel++)
            {
                if (existingChannels.Contains(channel))
                {
                    continue;
                }

                context.Batteries.Add(new Battery
                {
                    Channel = channel,
                    Name = $"Battery {channel}",
                    Description = string.Empty,
                    IsActive = true
                });
                added = true;
            }

            if (added)
            {
                context.SaveChanges();
            }
        }
    }
}
