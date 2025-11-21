using System.Runtime.InteropServices;
using FluentValidation;
using LibraryManagement.Application.Services.DTOs.BorrowingModels;
using LibraryManagement.Application.Services.Interaces;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Infrastructure.Repositories.Interfaces;
using LibraryManagement.Integration.Tests.Fixtures;
using SimpleInjector.Lifestyles;

namespace LibraryManagement.Integration.Tests.Application;

public class BorrowingServiceTests : IClassFixture<SqliteTestDatabaseFixture>
{
    private readonly SqliteTestDatabaseFixture _fixture;

    public BorrowingServiceTests(SqliteTestDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task BorrowBookAsync_WhenBorrowingIsAvailable_ShouldReturnDto()
    {
        using (AsyncScopedLifestyle.BeginScope(_fixture.Container))
        {
            var service = _fixture.Container.GetInstance<IBorrowingService>();
            var bookRepository = _fixture.Container.GetInstance<IBookRepository>();

            var command = new BorrowBookCommand
            {
                BookId = 1,
                UserId = 1,
                daysToReturn = 6
            };
            
            var result = await service.BorrowBookAsync(command);

            var borrowDate = DateTime.Today;
            var dueDate = borrowDate.AddDays(6);

            var book = await bookRepository.GetByIdAsync(command.BookId);

            Assert.Equal(command.BookId, result.BookId);
            Assert.Equal(borrowDate.ToString("yyyy-MM-dd"), result.BorrowDate);
            Assert.Equal(dueDate.ToString("yyyy-MM-dd"), result.DueDate);

            Assert.False(book!.IsAvailable);
        }
    }

    [Fact]
    public async Task BorrowBookAsync_WhenBorrowingIsUnavailable_ShouldTrow()
    {
        using (AsyncScopedLifestyle.BeginScope(_fixture.Container))
        {
            var service = _fixture.Container.GetInstance<IBorrowingService>();

            var command = new BorrowBookCommand
            {
                BookId = 7,
                UserId = 1,
                daysToReturn = 6
            };

            var ex = await Assert.ThrowsAsync<ValidationException>(async () =>
            {
                var result = await service.BorrowBookAsync(command);
            });
            Assert.Equal("Book is not available", ex.Message);
        }
    }

    [Fact]
    public async Task ReturnBookAsync_WhenBorrowingExists_ShouldReturnDto()
    {
        using (AsyncScopedLifestyle.BeginScope(_fixture.Container))
        {
            var service = _fixture.Container.GetInstance<IBorrowingService>();
            var bookRepository = _fixture.Container.GetInstance<IBookRepository>();
            var borrowingId = 2;

            var result = await service.ReturnBookAsync(borrowingId);
            var book = await bookRepository.GetByIdAsync(result.BookId);

            Assert.Equal(BorrowingStatus.Returned.ToString(), result.Status);
            Assert.True(book!.IsAvailable);
        }
    }

    [Fact]
    public async Task ReturnBookAsync_WhenBorrowingReturned_ShouldThrow()
    {
        using (AsyncScopedLifestyle.BeginScope(_fixture.Container))
        {
            var service = _fixture.Container.GetInstance<IBorrowingService>();

            await Assert.ThrowsAsync<ValidationException>(async () =>
            {
                var result = await service.ReturnBookAsync(3);
            });
        }
    }

    [Fact]
    public async Task CheckExpiredBorrowingsAsync_WhenExpiredBorrowingExists_ShouldUpdate()
    {
        using (AsyncScopedLifestyle.BeginScope(_fixture.Container))
        {
            var service = _fixture.Container.GetInstance<IBorrowingService>();
            var borrowingRepository = _fixture.Container.GetInstance<IBorrowingRepository>();

            await service.CheckExpiredBorrowingsAsync();

            var borrowing = await borrowingRepository.GetByIdAsync(4);

            Assert.Equal(BorrowingStatus.Overdue, borrowing!.Status);
        }
    }
}
