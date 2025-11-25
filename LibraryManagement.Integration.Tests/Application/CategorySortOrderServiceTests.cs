using LibraryManagement.Application.Services.Interaces;
using LibraryManagement.Integration.Tests.Fixtures;
using SimpleInjector.Lifestyles;

namespace LibraryManagement.Integration.Tests.Application;

public class CategorySortOrderServiceTests : IClassFixture<SqliteTestDatabaseFixture>
{
    private readonly SqliteTestDatabaseFixture _fixture;
    public CategorySortOrderServiceTests(SqliteTestDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ReorderCategoriesAsync_ShouldReorderExistingCategories()
    {
        using (AsyncScopedLifestyle.BeginScope(_fixture.Container))
        {
            var service = _fixture.Container.GetInstance<ICategorySortOrderService>();
            var categoryService = _fixture.Container.GetInstance<ICategoryService>();

            await service.ReorderCategoriesAsync();

            var categoryTree = await categoryService.GetCategoryTreeAsync();

            Assert.Equal(0, categoryTree[0].SortOrder);
            Assert.Equal(5, categoryTree[1].SortOrder);
        }
    }
}
