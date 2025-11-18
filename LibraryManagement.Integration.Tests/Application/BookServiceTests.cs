using SimpleInjector.Lifestyles;

using LibraryManagement.Application.Services.Interaces;
using LibraryManagement.Integration.Tests.Fixtures;
using LibraryManagement.Application.Services.DTOs.BookModels;
using FluentValidation;
using LibraryManagement.Shared.Exceptions;

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
    public async Task GetBookAsync_WhenIdDoesNotExist_ShouldThrow()
    {
        using (AsyncScopedLifestyle.BeginScope(_fixture.Container))
        {
            var service = _fixture.Container.GetInstance<IBookService>();
            await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
            {
                var result = await service.GetBookAsync(100);
            });
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
                PageCount = -1152
            };

            var ex = await Assert.ThrowsAsync<ValidationException>(async () =>
            {
                var resultBookDto = await service.CreateBookAsync(createBookCommand);
            });
            Assert.Equal("The ISBN should be unique; Author with such Id doesn't exist; Page number must be greater than 0; This date cannot be parsed", ex.Message);
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

    [Fact]
    public async Task DeleteBookAsync_WhenIdExists_ShouldDelete()
    {
        using (AsyncScopedLifestyle.BeginScope(_fixture.Container))
        {
            var service = _fixture.Container.GetInstance<IBookService>();

            await service.DeleteBookAsync(5);

            await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
            {
                var result = await service.GetBookAsync(5);
            });
        }
    }

    [Fact]
    public async Task DeleteBookAsync_WhenIdDoesNotExist_ShouldThrow()
    {
        using (AsyncScopedLifestyle.BeginScope(_fixture.Container))
        {
            var service = _fixture.Container.GetInstance<IBookService>();
            await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
            {
                await service.DeleteBookAsync(100);
            });
        }
    }

    [Fact]
    public async Task GetBooksAsync_WhenPageIsFull_ShouldReturnPage()
    {
        using (AsyncScopedLifestyle.BeginScope(_fixture.Container))
        {
            var service = _fixture.Container.GetInstance<IBookService>();
            var bookSearchDto = new SearchBookCommand
            {
                Title = "Book",
                ISBN = "97814000404"
            };

            var (_, numberOfPages, searchResultDtos) = await service.GetBooksAsync(bookSearchDto, 2, 1);

            Assert.NotEmpty(searchResultDtos);
            Assert.Equal(2, searchResultDtos.Count());
            Assert.Equal(3, numberOfPages);
        }
    }

    [Fact]
    public async Task GetBooksAsync_WhenPageIsNotFull_ShouldReturnPage()
    {
        using (AsyncScopedLifestyle.BeginScope(_fixture.Container))
        {
            var service = _fixture.Container.GetInstance<IBookService>();
            var bookSearchDto = new SearchBookCommand
            {
                Title = "Book",
                ISBN = "97814000404"
            };

            var (_, numberOfPages, searchResultDtos) = await service.GetBooksAsync(bookSearchDto, 4, 2);

            Assert.NotEmpty(searchResultDtos);
            Assert.Equal(2, searchResultDtos.Count());
            Assert.Equal(2, numberOfPages);
        }
    }

    [Fact]
    public async Task GetBooksAsync_WhenPageOutOfRange_ShouldReturnPage()
    {
        using (AsyncScopedLifestyle.BeginScope(_fixture.Container))
        {
            var service = _fixture.Container.GetInstance<IBookService>();
            var bookSearchDto = new SearchBookCommand
            {
                Title = "Book",
                ISBN = "97814000404"
            };
            await Assert.ThrowsAsync<IndexOutOfRangeException>(async () =>
            {
                var (totalCount, numberOfPages, searchResultDtos) = await service.GetBooksAsync(bookSearchDto, 4, 6);
            });
        }
    }

    [Fact]
    public async Task GetBooksAsync_WhenBooksDoesNotExist_ShouldThrow()
    {
        using (AsyncScopedLifestyle.BeginScope(_fixture.Container))
        {
            var service = _fixture.Container.GetInstance<IBookService>();
            var bookSearchDto = new SearchBookCommand
            {
                Title = "A ",
                AuthorId = 4,
                CategoryId = 6,
                ISBN = "444444444"
            };

            await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
            {
                var (totalCount, numberOfPages, searchResultDtos) = await service.GetBooksAsync(bookSearchDto, 100, 1);
            });
        }
    }

    [Fact]
    public async Task GetBooksAsync_WhenDateIsIncorrect_ShouldThrow()
    {
        using (AsyncScopedLifestyle.BeginScope(_fixture.Container))
        {
            var service = _fixture.Container.GetInstance<IBookService>();
            var bookSearchDto = new SearchBookCommand
            {
                PublishedDate = "10-10"
            };

            await Assert.ThrowsAsync<ValidationException>(async () =>
            {
                var (totalCount, numberOfPages, searchResultDtos) = await service.GetBooksAsync(bookSearchDto, 100, 1);
            });
        }
    }

    [Fact]
    public async Task UpdateBookAsync_WhenBookExists_ShouldUpdate()
    {
        using (AsyncScopedLifestyle.BeginScope(_fixture.Container))
        {
            var service = _fixture.Container.GetInstance<IBookService>();
            var updateBookCommand = new UpdateBookCommand
            {
                Title = "Programming Book",
                PublishedDate = "2002-10-12",
                CategoryId = 4,
                PageCount = 128
            };

            var result = await service.UpdateBookAsync(updateBookCommand, 1);

            Assert.Equal(updateBookCommand.Title, result.Title);
            Assert.Equal(updateBookCommand.CategoryId, result.CategoryId);
            Assert.Equal(updateBookCommand.PublishedDate, result.PublishedDate);
            Assert.Equal(updateBookCommand.PageCount, result.PageCount);
        }
    }

    [Fact]
    public async Task UpdateBookAsync_WhenCommandIsInvalid_ShouldThrow()
    {
        using (AsyncScopedLifestyle.BeginScope(_fixture.Container))
        {
            var service = _fixture.Container.GetInstance<IBookService>();
            var updateBookCommand = new UpdateBookCommand
            {
                PublishedDate = "202-12",
                CategoryId = 100,
                PageCount = -128
            };

            var ex = await Assert.ThrowsAsync<ValidationException>(async () =>
            {
                var result = await service.UpdateBookAsync(updateBookCommand, 1);
            });

            Assert.Equal("Category with such Id doesn't exist.; Page number must be greater than 0; This date cannot be parsed", ex.Message);
        }
    }
}
