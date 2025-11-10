// Failing with DotNet.Testcontainers.Containers.ResourceReaperException : Initialization has been cancelled.
using Testcontainers.PostgreSql;

using LibraryManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Integration.Tests.Fixtures;

public class PostgreSqlTestDatabaseFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = null!;
    public LibraryDbContext Context { get; private set; } = null!;

    public PostgreSqlTestDatabaseFixture()
    {
        _container = new PostgreSqlBuilder()
            .WithDatabase("testdb")
            .WithUsername("postgres")
            .WithPassword("password")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        var optionsBuilder = new DbContextOptionsBuilder<LibraryDbContext>();
        optionsBuilder.UseNpgsql(_container.GetConnectionString());

        Context = new LibraryDbContext(optionsBuilder.Options);
        await Context.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await Context.DisposeAsync();
        await _container.StopAsync();
    }
}
