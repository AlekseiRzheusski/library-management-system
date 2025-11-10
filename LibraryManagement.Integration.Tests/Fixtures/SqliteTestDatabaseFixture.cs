using LibraryManagement.Infrastructure.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Integration.Tests.Fixtures;

public class SqliteTestDatabaseFixture : IAsyncLifetime
{
    public LibraryDbContext Context { get; private set; } = null!;
    private DbContextOptions<LibraryDbContext> _options = null!;
    private SqliteConnection _connection = null!;

    public async Task InitializeAsync()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        await _connection.OpenAsync();

        _options = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseSqlite(_connection)
            .Options;

        Context =  new LibraryDbContext(_options);
        await Context.Database.MigrateAsync();

    }
    public LibraryDbContext CreateContext()
    {
        return new LibraryDbContext(_options);
    }

    public async Task DisposeAsync()
    {
        await Context.DisposeAsync();
        await _connection.CloseAsync();
        _connection.Dispose();
    }
}
