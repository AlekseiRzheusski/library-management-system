using SimpleInjector.Lifestyles;

using LibraryManagement.Application.Services.Interaces;
using LibraryManagement.Integration.Tests.Fixtures;
using LibraryManagement.Application.Services.DTOs.BookModels;
using FluentValidation;

namespace LibraryManagement.Integration.Tests.Application;

public class BookServiceTests : IClassFixture<SqliteTestDatabaseFixture>
{
    private readonly SqliteTestDatabaseFixture _fixture;
    public BookServiceTests(SqliteTestDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetBookAsync_WhenIdExists_ShouldReturnBookDto()
    {
        using (AsyncScopedLifestyle.BeginScope(_fixture.Container))
        {
            var service = _fixture.Container.GetInstance<IBookService>();
            var result = await service.GetBookAsync(2);

            Assert.NotNull(result);
            Assert.Equal("Andrew Tanenbaum", result.AuthorName);
            Assert.Equal("Programming", result.CategoryName);
            Assert.Equal("2012-07-25", result.PublishedDate);
        }
    }

    [Fact]
    public async Task GetBookAsync_WhenIdDoesNotExist_ShouldReturnNull()
    {
        using (AsyncScopedLifestyle.BeginScope(_fixture.Container))
        {
            var service = _fixture.Container.GetInstance<IBookService>();
            var result = await service.GetBookAsync(100);

            Assert.Null(result);
        }
    }

    [Fact]
    public async Task CreateBookAsync_WhenCreateBookCommandIsCorrect_ShouldAddAndReturnBookDto()
    {
        using (AsyncScopedLifestyle.BeginScope(_fixture.Container))
        {
            var service = _fixture.Container.GetInstance<IBookService>();

            var createBookCommand = new CreateBookCommand
            {
                Title = "Dance with Dragons",
                Isbn = "9780606321853",
                Description = "In the aftermath of a colossal battle, Daenerys Targaryen rules with her three dragons as queen of a city built on dust and death. But Daenerys has thousands of enemies, and many have set out to find her. Fleeing from Westeros with a price on his head, Tyrion Lannister, too, is making his way east--with new allies who may not be the ragtag band they seem. And in the frozen north, Jon Snow confronts creatures from beyond the Wall of ice and stone, and powerful foes from within the Night's Watch. In a time of rising restlessness, the tides of destiny and politics lead a grand cast of outlaws and priests, soldiers and skinchangers, nobles and slaves, to the greatest dance of all.",
                AuthorId = 4,
                CategoryId = 6,
                PublishedDate = "2013-10-29",
                PageCount = 1152
            };

            var resultBookDto = await service.CreateBookAsync(createBookCommand);

            Assert.NotNull(resultBookDto);
            Assert.Equal(createBookCommand.Title, resultBookDto.Title);
            Assert.Equal(createBookCommand.PublishedDate, resultBookDto.PublishedDate);
            Assert.Equal(createBookCommand.AuthorId, resultBookDto.AuthorId);
            Assert.Equal(createBookCommand.CategoryId, resultBookDto.CategoryId);
        }
    }

    [Fact]
    public async Task CreateBookAsync_WhenCreateBookCommandIsIncorrect_ShouldThrow()
    {
        using (AsyncScopedLifestyle.BeginScope(_fixture.Container))
        {
            var service = _fixture.Container.GetInstance<IBookService>();

            var createBookCommand = new CreateBookCommand
            {
                Title = "Dance with Dragons",
                Isbn = "9780606321853",
                Description = "In the aftermath of a colossal battle, Daenerys Targaryen rules with her three dragons as queen of a city built on dust and death. But Daenerys has thousands of enemies, and many have set out to find her. Fleeing from Westeros with a price on his head, Tyrion Lannister, too, is making his way east--with new allies who may not be the ragtag band they seem. And in the frozen north, Jon Snow confronts creatures from beyond the Wall of ice and stone, and powerful foes from within the Night's Watch. In a time of rising restlessness, the tides of destiny and politics lead a grand cast of outlaws and priests, soldiers and skinchangers, nobles and slaves, to the greatest dance of all.",
                AuthorId = 10,
                CategoryId = 6,
                PublishedDate = "10-10-10",
                PageCount = 1152
            };

            var ex = await Assert.ThrowsAsync<ValidationException>(async () =>
            {
                var resultBookDto = await service.CreateBookAsync(createBookCommand);
            });
            Assert.Equal("The ISBN should be unique; Author with such Id doesn't exist; This date cannot be parsed", ex.Message);
        }
    }

    [Fact]
    public async Task CreateBookAsync_WhenCreateBookCommandHasEmptyPublishedDate_ShouldAdd()
    {
        using (AsyncScopedLifestyle.BeginScope(_fixture.Container))
        {
            var service = _fixture.Container.GetInstance<IBookService>();

            var createBookCommand = new CreateBookCommand
            {
                Title = "Plays: Ivanov; The Seagull; Uncle Vanya; Three Sisters; The CherryOrchard (Penguin Classics)",
                Isbn = "9780140447330",
                Description = "Five masterful dramatic works from one of the world's best-loved playwrights, including The Seagull—now a major motion picture starring Saoirse Ronan, Elizabeth Moss, and Annette Bening",
                AuthorId = 4,
                CategoryId = 5,
                PublishedDate = "",
                PageCount = 1152
            };

            var resultBookDto = await service.CreateBookAsync(createBookCommand);

            Assert.NotNull(resultBookDto);
            Assert.Empty(resultBookDto.PublishedDate!);
        }
    }
}
