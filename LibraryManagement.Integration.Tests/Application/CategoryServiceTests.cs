using FluentValidation;
using SimpleInjector.Lifestyles;

using LibraryManagement.Application.Services.DTOs.CategoryModels;
using LibraryManagement.Application.Services.Interaces;
using LibraryManagement.Infrastructure.Repositories.Interfaces;
using LibraryManagement.Integration.Tests.Fixtures;

namespace LibraryManagement.Integration.Tests.Application;

public class CategoryServiceTests : IClassFixture<SqliteTestDatabaseFixture>
{
    private readonly SqliteTestDatabaseFixture _fixture;
    public CategoryServiceTests(SqliteTestDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetCategoryTreeAsync_WhenCategoriesExist_ShouldReturnDto()
    {
        using (AsyncScopedLifestyle.BeginScope(_fixture.Container))
        {
            var service = _fixture.Container.GetInstance<ICategoryService>();

            var result = await service.GetCategoryTreeAsync();

            Assert.NotEmpty(result);
            Assert.Equal(2, result.Count());
            Assert.Equal(2, result[0].SubCategories.Count());
            Assert.Equal(2, result[1].SubCategories.Count());
        }
    }

    [Fact]
    public async Task GetCategoriesAsync_WhenCategoriesExists_ShouldReturnDto()
    {
        using (AsyncScopedLifestyle.BeginScope(_fixture.Container))
        {
            var service = _fixture.Container.GetInstance<ICategoryService>();

            var command = new SearchCategoryCommand
            {
                ParentCategoryId = 2,
                IsActive = true
            };

            var result = await service.GetCategoriesAsync(command);

            Assert.NotEmpty(result);
            Assert.Equal(2, result.Count());
        }
    }

    [Fact]
    public async Task GetCategoriesAsync_IfParentCategoryIdDoesNotExists_ShouldThrow()
    {
        using (AsyncScopedLifestyle.BeginScope(_fixture.Container))
        {
            var service = _fixture.Container.GetInstance<ICategoryService>();

            var command = new SearchCategoryCommand
            {
                ParentCategoryId = 100,
            };

            await Assert.ThrowsAsync<ValidationException>(async () =>
            {
                var result = await service.GetCategoriesAsync(command);
            });
        }
    }

    [Fact]
    public async Task CreateCategoryAsync_IfCommandIsCorrect_ShouldAdd()
    {
        using (AsyncScopedLifestyle.BeginScope(_fixture.Container))
        {
            var service = _fixture.Container.GetInstance<ICategoryService>();
            var categoryRepository = _fixture.Container.GetInstance<ICategoryRepository>();

            var command = new CreateCategoryCommand
            {
                Name = "Geometry",
                Description = "Is a branch of mathematics concerned with properties of space such as the distance, shape, size, and relative position of figures.",
                ParentCategoryId = 3
            };

            var result = await service.CreateCategoryAsync(command);

            Assert.True(await categoryRepository.ExistsAsync(c=> c.CategoryId == result.CategoryId));
        }
    }

    [Fact]
    public async Task CreateCategoryAsync_IfNameExists_ShouldThrow()
    {
        using (AsyncScopedLifestyle.BeginScope(_fixture.Container))
        {
            var service = _fixture.Container.GetInstance<ICategoryService>();
            var categoryRepository = _fixture.Container.GetInstance<ICategoryRepository>();

            var command = new CreateCategoryCommand
            {
                Name = "Math",
                Description = "Is a branch of mathematics concerned with properties of space such as the distance, shape, size, and relative position of figures.",
                ParentCategoryId = 1
            };
            var ex = await Assert.ThrowsAsync<ValidationException>(async () =>
            {
                var result = await service.CreateCategoryAsync(command);    
            });

            Assert.Equal("The Category name should be unique.", ex.Message);
        }
    }

    [Fact]
    public async Task DeleteCategoryAsync_IfCategoryHasRelatedBooks_ShouldThrow()
    {
        using (AsyncScopedLifestyle.BeginScope(_fixture.Container))
        {
            var service = _fixture.Container.GetInstance<ICategoryService>();

            var ex = await Assert.ThrowsAsync<ValidationException>(async () =>
            {
                await service.DeleteCategoryAsync(6);
            });

            Assert.Equal("This category has related books; This category has related subcategories", ex.Message);
        }
    }

    [Fact]
    public async Task DeleteCategoryAsync_IfCategoryDoesNotHaveRelatedBooksOrSubcategories_ShouldDelete()
    {
        using (AsyncScopedLifestyle.BeginScope(_fixture.Container))
        {
            var service = _fixture.Container.GetInstance<ICategoryService>();
            var categoryRepository = _fixture.Container.GetInstance<ICategoryRepository>();

            var command = new CreateCategoryCommand
            {
                Name = "News paper",
                Description = "News paper",
                ParentCategoryId = 3
            };

            var result = await service.CreateCategoryAsync(command);

            await service.DeleteCategoryAsync(result.CategoryId);
            var deletedEntity = await categoryRepository.ExistsAsync(c=>c.CategoryId==result.CategoryId);
            Assert.False(deletedEntity);
        }
    }
}
