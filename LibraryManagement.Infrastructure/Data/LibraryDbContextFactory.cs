using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace LibraryManagement.Infrastructure.Data;

public class LibraryDbContextFactory : IDesignTimeDbContextFactory<LibraryDbContext>
{
    public LibraryDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .Build();
        
         var provider = configuration["DatabaseProvider"]
                       ?? "PostgreSql";

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        var optionsBuilder = new DbContextOptionsBuilder<LibraryDbContext>();
        if (provider == "Sqlite")
        {
            optionsBuilder.UseSqlite(connectionString, x =>
                x.MigrationsAssembly("LibraryManagement.Migrations.Sqlite"));
        }
        else
        {
            optionsBuilder.UseNpgsql(connectionString, x =>
                x.MigrationsAssembly("LibraryManagement.Migrations.PostgreSql"));
        }
        return new LibraryDbContext(optionsBuilder.Options);
    }
}
