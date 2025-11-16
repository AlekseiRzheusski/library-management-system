using Microsoft.EntityFrameworkCore;
using SimpleInjector.Lifestyles;

using LibraryManagement.Domain.Entities;
using LibraryManagement.Infrastructure.Repositories.Interfaces;
using LibraryManagement.Integration.Tests.Fixtures;

namespace LibraryManagement.Integration.Tests.Infrastructure;

public class AuthorRepositoryTests : IClassFixture<SqliteTestDatabaseFixture>
{
    private readonly SqliteTestDatabaseFixture _fixture;
    public AuthorRepositoryTests(SqliteTestDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetByIdAsync_WhenIdExists_ShouldReturnEntity()
    {
        using (AsyncScopedLifestyle.BeginScope(_fixture.Container))
        {
            var repository = _fixture.Container.GetInstance<IAuthorRepository>();

            var result = await repository.GetByIdAsync(2);

            Assert.NotNull(result);
            Assert.Equal(2, result.AuthorId);
            Assert.Equal("Andrew", result.FirstName);
        }
    }

    [Fact]
    public async Task GetByIdAsync_WhenIdDoesNotExist_ShouldReturnNull()
    {
        using (AsyncScopedLifestyle.BeginScope(_fixture.Container))
        {
            var repository = _fixture.Container.GetInstance<IAuthorRepository>();
            var result = await repository.GetByIdAsync(10);

            Assert.Null(result);
        }
    }

    [Fact]
    public async Task FindAsync_WhenFirstNameExists_ShouldReturnEntity()
    {
        using (AsyncScopedLifestyle.BeginScope(_fixture.Container))
        {
            var repository = _fixture.Container.GetInstance<IAuthorRepository>();
            var resultList = await repository.FindAsync(a => a.FirstName == "George");
            Assert.Single(resultList);

            var resultEntity = resultList.First();
            Assert.Equal(4, resultEntity.AuthorId);
        }
    }

    [Fact]
    public async Task FindAsync_WhenFirstNameDoesnNotExist_ShouldReturnEmptyList()
    {
        using (AsyncScopedLifestyle.BeginScope(_fixture.Container))
        {
            var repository = _fixture.Container.GetInstance<IAuthorRepository>();
            var resultList = await repository.FindAsync(a => a.FirstName == "Viktor");
            Assert.Empty(resultList);
        }
    }

    [Fact]
    public async Task AddAsync_ShouldAddEntity()
    {
        using (AsyncScopedLifestyle.BeginScope(_fixture.Container))
        {
            var repository = _fixture.Container.GetInstance<IAuthorRepository>();
            Author author = new Author
            {
                FirstName = "Mikhail",
                LastName = "Saltykov-Shchedrin",
                DateOfBirth = new DateTime(1889, 5, 10)
            };

            await repository.AddAsync(author);
            await repository.SaveAsync();

            var resultList = await repository.FindAsync(a => a.LastName == "Saltykov-Shchedrin");


            Assert.Single(resultList);

            var resultEntity = resultList.First();
            Assert.Equal(author.FirstName, resultEntity.FirstName);
            Assert.Equal(author.DateOfBirth, resultEntity.DateOfBirth);
            Assert.Equal(author.AuthorId, resultEntity.AuthorId);
        }
    }

    [Fact]
    public async Task AddAsync_WhenIdExists_ShouldThrowDbUpdateException()
    {
        using (AsyncScopedLifestyle.BeginScope(_fixture.Container))
        {
            var repository = _fixture.Container.GetInstance<IAuthorRepository>();
            Author authorWithDuplicatedId = new Author
            {
                AuthorId = 4,
                FirstName = "Mikhail",
                LastName = "Saltykov-Shchedrin",
                DateOfBirth = new DateTime(1826, 1, 15)
            };

            await Assert.ThrowsAsync<DbUpdateException>(async () =>
            {
                await repository.AddAsync(authorWithDuplicatedId);
                await repository.SaveAsync();
            });
        }
    }

    [Fact]
    public async Task Update_WhenEntityExists_ShouldUpdateEntity()
    {
        using (AsyncScopedLifestyle.BeginScope(_fixture.Container))
        {
            var repository = _fixture.Container.GetInstance<IAuthorRepository>();
            Author entityToUpdate = new Author
            {
                FirstName = "Sergey",
                LastName = "Esenin",
                DateOfBirth = new DateTime(1895, 10, 3)
            };

            await repository.AddAsync(entityToUpdate);
            await repository.SaveAsync();

            entityToUpdate.FirstName = "Maxim";
            entityToUpdate.LastName = "Gorky";

            repository.Update(entityToUpdate);
            await repository.SaveAsync();

            var updated = await repository.GetByIdAsync(entityToUpdate.AuthorId);

            Assert.Equal("Maxim", updated!.FirstName);
            Assert.Equal("Gorky", updated!.LastName);
        }
    }

    [Fact]
    public async Task Delete_WhenEntityExists_ShouldDeleteEntity()
    {
        using (AsyncScopedLifestyle.BeginScope(_fixture.Container))
        {
            var repository = _fixture.Container.GetInstance<IAuthorRepository>();
            var entityToDelete = new Author
            {
                FirstName = "Sergey",
                LastName = "Dovlatov",
                DateOfBirth = new DateTime(1889, 5, 10)
            };
            await repository.AddAsync(entityToDelete);
            await repository.SaveAsync();

            var id = entityToDelete.AuthorId;

            repository.Delete(entityToDelete);
            await repository.SaveAsync();

            var deleted = await repository.GetByIdAsync(id);
            Assert.Null(deleted);
        }
    }

    [Fact]
    public async Task GetAllAsync_WhenEntitiesExists_ShouldReturnList()
    {
        using (AsyncScopedLifestyle.BeginScope(_fixture.Container))
        {
            var repository = _fixture.Container.GetInstance<IAuthorRepository>();

            var numberOfEntities = await repository.CountAsync();
            var list = await repository.GetAllAsync();
            Assert.NotEmpty(list);
            Assert.Equal(numberOfEntities, list.Count());
        }
    }

    [Fact]
    public async Task ExistsAsync_WhenEntityExists_ShouldReturnTrue()
    {
        using (AsyncScopedLifestyle.BeginScope(_fixture.Container))
        {
            var repository = _fixture.Container.GetInstance<IAuthorRepository>();

            var isExists = await repository.ExistsAsync(a => a.FirstName == "Anton");
            Assert.True(isExists);
        }
    }

    [Fact]
    public async Task ExistsAsync_WhenEntityDoesNotExist_ShouldReturnFalse()
    {
        using (AsyncScopedLifestyle.BeginScope(_fixture.Container))
        {
            var repository = _fixture.Container.GetInstance<IAuthorRepository>();

            var isExists = await repository.ExistsAsync(a => a.FirstName == "Igor");
            Assert.False(isExists);
        }
    }

    [Fact]
    public async Task ExistsAsync_WhenTokenIsCanceled_ShouldThrow()
    {
        using (AsyncScopedLifestyle.BeginScope(_fixture.Container))
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;
            var repository = _fixture.Container.GetInstance<IAuthorRepository>();

            cancellationTokenSource.Cancel();

            await Assert.ThrowsAsync<OperationCanceledException>(async () =>
                await repository.ExistsAsync(a => a.FirstName == "Anton", token));
        }
    }
}
