using Microsoft.EntityFrameworkCore;
using Synapse.Infrastructure.Entities;
using Microsoft.Data.Sqlite;

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
    }
}
