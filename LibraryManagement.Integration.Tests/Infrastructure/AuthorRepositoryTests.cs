using Microsoft.EntityFrameworkCore;

using LibraryManagement.Domain.Entities;
using LibraryManagement.Infrastructure.Repositories;
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
        using (var context = _fixture.CreateContext())
        {
            var repository = new AuthorRepository(context);
            var result = await repository.GetByIdAsync(2);
            Assert.NotNull(result);
            Assert.Equal(2, result.AuthorId);
            Assert.Equal("Andrew", result.FirstName);
        }
    }

    [Fact]
    public async Task GetByIdAsync_WhenIdDoesNotExist_ShouldReturnNull()
    {
        using (var context = _fixture.CreateContext())
        {
            var repository = new AuthorRepository(context);
            var result = await repository.GetByIdAsync(10);

            Assert.Null(result);
        }
    }

    [Fact]
    public async Task FindAsync_WhenFirstNameExists_ShouldReturnEntity()
    {
        using (var context = _fixture.CreateContext())
        {
            var repository = new AuthorRepository(context);
            var resultList = await repository.FindAsync(a => a.FirstName == "George");
            Assert.Single(resultList);

            var resultEntity = resultList.First();
            Assert.Equal(4, resultEntity.AuthorId);
        }
    }

    [Fact]
    public async Task FindAsync_WhenFirstNameDoesnNotExist_ShouldReturnEmptyList()
    {
        using (var context = _fixture.CreateContext())
        {
            var repository = new AuthorRepository(context);
            var resultList = await repository.FindAsync(a => a.FirstName == "Viktor");
            Assert.Empty(resultList);
        }
    }


    [Fact]
    public async Task AddAsync_ShouldAddEntity()
    {
        using (var context = _fixture.CreateContext())
        {
            var repository = new AuthorRepository(context);
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
            Assert.Equal(6, resultEntity.AuthorId);
        }
    }

    [Fact]
    public async Task AddAsync_WhenIdExists_ShouldThrowDbUpdateException()
    {
        using (var context = _fixture.CreateContext())
        {
            var repository = new AuthorRepository(context);
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
        using (var context = _fixture.CreateContext())
        {
            var repository = new AuthorRepository(context);
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
        using (var context = _fixture.CreateContext())
        {
            var repository = new AuthorRepository(context);
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
        using (var context = _fixture.CreateContext())
        {
            var repository = new AuthorRepository(context);

            var numberOfEntities = await repository.CountAsync();
            var list = await repository.GetAllAsync();
            Assert.NotEmpty(list);
            Assert.Equal(numberOfEntities, list.Count());
        }
    }
}
