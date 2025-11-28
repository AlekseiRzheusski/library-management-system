using LibraryManagement.Integration.Tests.Fixtures;

namespace LibraryManagement.Integration.Tests.Application.Author;

[CollectionDefinition("Author collection")]
public class DatabaseCollection : ICollectionFixture<SqliteTestDatabaseFixture>
{
    
}
