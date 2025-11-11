using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SimpleInjector;
using SimpleInjector.Lifestyles;

using LibraryManagement.Infrastructure.Data;
using LibraryManagement.Infrastructure.Repositories;
using LibraryManagement.Infrastructure.Repositories.Interfaces;
using LibraryManagement.Infrastructure;
using LibraryManagement.Application;
using LibraryManagement.Api;

namespace LibraryManagement.Integration.Tests.Fixtures;

public class SqliteTestDatabaseFixture : IAsyncLifetime
{
    public Container Container { get; private set; } = null!;
    private SqliteConnection _connection = null!;

    public async Task InitializeAsync()
    {
        Container = new Container();
        Container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

        _connection = new SqliteConnection("DataSource=:memory:");
        await _connection.OpenAsync();

        var options = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseSqlite(_connection)
            .Options;

        Container.AddInfrastructure(options);
        Container.AddApplication();
        Container.AddAutoMapper();

        using (AsyncScopedLifestyle.BeginScope(Container))
        {
            var context = Container.GetInstance<LibraryDbContext>();
            await context.Database.MigrateAsync();
        }
        Container.Verify();
    }

    public async Task DisposeAsync()
    {
        await _connection.DisposeAsync();
        Container.Dispose();
    }
}
