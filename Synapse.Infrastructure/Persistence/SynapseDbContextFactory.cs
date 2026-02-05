using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Synapse.Shared;
using System;
using System.IO;

namespace Synapse.Infrastructure.Persistence;

public class SynapseDbContextFactory : IDesignTimeDbContextFactory<SynapseDbContext>
{
    public SynapseDbContext CreateDbContext(string[] args)
    {
        var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var dbFolder = Path.Combine(folderPath, Const.APPLICATION_FOLDER_NAME);
        Directory.CreateDirectory(dbFolder);
        var dbPath = Path.Combine(dbFolder, Const.DATABASE_FILE_NAME);

        var optionsBuilder = new DbContextOptionsBuilder<SynapseDbContext>();
        optionsBuilder.UseSqlite($"Data Source={dbPath}");

        return new SynapseDbContext(optionsBuilder.Options);
    }
}
