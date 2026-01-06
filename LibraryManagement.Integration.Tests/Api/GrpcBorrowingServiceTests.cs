using Moq;
using AutoMapper;
using Grpc.Core;
using FluentValidation;
using Microsoft.Extensions.Logging;

using Librarymanagement;
using LibraryManagement.Api.Mappings;
using LibraryManagement.Api.Services;
using LibraryManagement.Application.Services.DTOs.BorrowingModels;
using LibraryManagement.Application.Services.Interaces;
using LibraryManagement.Shared.Exceptions;

namespace LibraryManagement.Integration.Tests.Api;

public class GrpcBorrowingServiceTests
{
    private readonly Mock<IBorrowingService> _borrowingServiceMock;
    private readonly IMapper _mapper;
    private readonly GrpcBorrowingService _grpcBorrowingService;
    public GrpcBorrowingServiceTests()
    {
        _borrowingServiceMock = new Mock<IBorrowingService>();

        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<GrpcBorrowingMappingProfile>();
        }, loggerFactory);

        _mapper = config.CreateMapper();

        config.AssertConfigurationIsValid();

        _grpcBorrowingService = new GrpcBorrowingService(_borrowingServiceMock.Object, _mapper);
    }

    [Fact]
    public async Task BorrowBook_IfRequestIsValid_ShouldReturnNewBorrowing()
    {
        var borrowing = new BorrowingDto
        {
            BorrowingId = 1,
            BookId = 1,
            BookTitle = "Test book",
            BorrowDate = "2026-01-06",
            DueDate = "2026-01-16",
            ReturnDate = "",
            Status = "Active"
        };

        var request = new BorrowBookRequest
        {
            UserId = 1,
            BookId = 1,
            DaysToReturn = 10
        };
        var context = Mock.Of<ServerCallContext>();

        _borrowingServiceMock
            .Setup(s => s.BorrowBookAsync(It.IsAny<BorrowBookCommand>()))
            .ReturnsAsync(borrowing);

        var result = await _grpcBorrowingService.BorrowBook(request, context);

        Assert.NotNull(result);
        Assert.Equal(borrowing.BookTitle, result.BookTitle);

        _borrowingServiceMock.Verify(s =>
            s.BorrowBookAsync(It.IsAny<BorrowBookCommand>()),
            Times.Once);
    }

    [Fact]
    public async Task BorrowBook_IfServiceMethodThrowsValidationException_ShouldThrowInvalidArgument()
    {
        var request = new BorrowBookRequest
        {
            UserId = 1,
            BookId = 1,
            DaysToReturn = -10
        };
        var context = Mock.Of<ServerCallContext>();

        _borrowingServiceMock
            .Setup(s => s.BorrowBookAsync(It.IsAny<BorrowBookCommand>()))
            .ThrowsAsync(new ValidationException("Days to return must be greater than 0;"));

        var ex = await Assert.ThrowsAsync<RpcException>(() => _grpcBorrowingService.BorrowBook(request, context));
        Assert.Equal(StatusCode.InvalidArgument, ex.StatusCode);
    }

    [Fact]
    public async Task ReturnBook_IfBorrowingExists_ShouldReturnBorrowing()
    {
        var borrowing = new BorrowingDto
        {
            BorrowingId = 1,
            BookId = 1,
            BookTitle = "Test book",
            BorrowDate = "2026-01-06",
            DueDate = "2026-01-16",
            ReturnDate = "2026-01-12",
            Status = "Returned"
        };
        var request = new ReturnBookRequest { BorrowingId = 1L };
        var context = Mock.Of<ServerCallContext>();

        _borrowingServiceMock
            .Setup(s => s.ReturnBookAsync(It.IsAny<long>()))
            .ReturnsAsync(borrowing);

        var result = await _grpcBorrowingService.ReturnBook(request, context);

        Assert.NotNull(result);
        Assert.Equal(borrowing.BookTitle, result.BookTitle);

        _borrowingServiceMock.Verify(s =>
            s.ReturnBookAsync(request.BorrowingId),
            Times.Once);
    }

    [Fact]
    public async Task ReturnBook_IfServiceMethodThrowsEntityNotFoundException_ShouldThrowNotFound()
    {
        var request = new ReturnBookRequest { BorrowingId = 100 };
        var context = Mock.Of<ServerCallContext>();

        _borrowingServiceMock
            .Setup(s => s.ReturnBookAsync(It.IsAny<long>()))
            .ThrowsAsync(new EntityNotFoundException($"Book with ID {request.BorrowingId} does not exist"));

        var ex = await Assert.ThrowsAsync<RpcException>(() => _grpcBorrowingService.ReturnBook(request, context));
        Assert.Equal(StatusCode.NotFound, ex.StatusCode);
    }

    [Fact]
    public async Task ReturnBook_IfServiceMethodThrowsValidationException_ShouldThrowInvalidArgument()
    {
        var request = new ReturnBookRequest { BorrowingId = 100 };
        var context = Mock.Of<ServerCallContext>();

        _borrowingServiceMock
            .Setup(s => s.ReturnBookAsync(It.IsAny<long>()))
            .ThrowsAsync(new ValidationException("This borrowing is already has the returned status"));

        var ex = await Assert.ThrowsAsync<RpcException>(() => _grpcBorrowingService.ReturnBook(request, context));
        Assert.Equal(StatusCode.InvalidArgument, ex.StatusCode);
    }

    [Fact]
    public async Task GetUserBorrowings_IfBorrowingsExists_ShouldReturnList()
    {
        var borrowingDtoList = new List<BorrowingDto>
        {
            new BorrowingDto {
                BorrowingId = 1,
                BookId = 1,
                BookTitle = "Test book 1",
                BorrowDate = "2026-01-06",
                DueDate = "2026-01-16",
                ReturnDate = "2026-01-12",
                Status = "Returned"
            },
            new BorrowingDto {
                BorrowingId = 2,
                BookId = 2,
                BookTitle = "Test book 2",
                BorrowDate = "2026-01-06",
                DueDate = "2026-01-16",
                ReturnDate = "2026-01-12",
                Status = "Active"
            },
        };

        var request = new UserBorrowingsRequest
        {
            UserId = 1,
            PageNumber = 1,
            PageSize = 2
        };
        var context = Mock.Of<ServerCallContext>();

        _borrowingServiceMock
            .Setup(s => s.GetUserBorrowingsAsync(It.IsAny<UserBorrowingsCommand>(), 1, 2))
            .ReturnsAsync((2, 1, borrowingDtoList));

        var result = await _grpcBorrowingService.GetUserBorrowings(request, context);

        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Borrowings.Count);

        _borrowingServiceMock.Verify(s =>
            s.GetUserBorrowingsAsync(It.IsAny<UserBorrowingsCommand>(), 1, 2),
            Times.Once);
    }

    [Fact]
    public async Task GetUserBorrowings_IfServiceMethodThrowsValidationException_ShouldReturnInvalidArgument()
    {
        var request = new UserBorrowingsRequest
        {
            UserId = 1,
            PageNumber = 1,
            PageSize = 10
        };
        var context = Mock.Of<ServerCallContext>();

        _borrowingServiceMock
            .Setup(s => s.GetUserBorrowingsAsync(
                It.IsAny<UserBorrowingsCommand>(),
                It.IsAny<int>(),
                It.IsAny<int>()))
            .ThrowsAsync(new ValidationException("Page Size must be greater than 0"));

        var ex = await Assert.ThrowsAsync<RpcException>(() =>
            _grpcBorrowingService.GetUserBorrowings(request, context));

        Assert.Equal(StatusCode.InvalidArgument, ex.StatusCode);
    }

    [Fact]
    public async Task GetUserBorrowings_IfServiceMethodThrowsEntityNotFoundException_ShouldReturnNotFound()
    {
        var request = new UserBorrowingsRequest
        {
            UserId = 1,
            PageNumber = 1,
            PageSize = 10
        };
        var context = Mock.Of<ServerCallContext>();

        _borrowingServiceMock
            .Setup(s => s.GetUserBorrowingsAsync(
                It.IsAny<UserBorrowingsCommand>(),
                It.IsAny<int>(),
                It.IsAny<int>()))
            .ThrowsAsync(new EntityNotFoundException("No results match your search criteria."));

        var ex = await Assert.ThrowsAsync<RpcException>(() =>
            _grpcBorrowingService.GetUserBorrowings(request, context));

        Assert.Equal(StatusCode.NotFound, ex.StatusCode);
    }

    [Fact]
    public async Task GetUserBorrowings_IfServiceMethodThrowsIndexOutOfRangeException_ShouldReturnOutOfRange()
    {
        var request = new UserBorrowingsRequest
        {
            UserId = 1,
            PageNumber = 999,
            PageSize = 10
        };
        var context = Mock.Of<ServerCallContext>();

        _borrowingServiceMock
            .Setup(s => s.GetUserBorrowingsAsync(
                It.IsAny<UserBorrowingsCommand>(),
                It.IsAny<int>(),
                It.IsAny<int>()))
            .ThrowsAsync(new IndexOutOfRangeException("Page number must not exceed 12"));

        var ex = await Assert.ThrowsAsync<RpcException>(() =>
            _grpcBorrowingService.GetUserBorrowings(request, context));

        Assert.Equal(StatusCode.OutOfRange, ex.StatusCode);
    }

    [Fact]
    public async Task GetOverdueBooks_IfOverdueBorrowingsExists_ShouldReturnList()
    {
        var overdueList = new List<BorrowingDto>
        {
            new BorrowingDto {
                BorrowingId = 1,
                BookId = 1,
                BookTitle = "Test book",
                BorrowDate = "2026-01-01",
                DueDate = "2026-01-06",
                ReturnDate = "",
                Status = "Overdue"
            }
        };
        var request = new OverdueBooksRequest { PageNumber = 1, PageSize = 1 };
        var context = Mock.Of<ServerCallContext>();

        _borrowingServiceMock
            .Setup(s => s.GetOverdueBooksAsync(1, 1))
            .ReturnsAsync((1, 1, overdueList));

        var result = await _grpcBorrowingService.GetOverdueBooks(request, context);

        Assert.NotNull(result);
        Assert.Single(result.Borrowings);
        Assert.Equal("Test book", result.Borrowings[0].BookTitle);

        _borrowingServiceMock.Verify(s =>
            s.GetOverdueBooksAsync(1, 1),
            Times.Once);
    }

    [Fact]
    public async Task GetOverdueBooks_IfServiceMethodThrowsValidationException_ShouldReturnInvalidArgument()
    {
        var request = new OverdueBooksRequest
        {
            PageNumber = 1,
            PageSize = 10
        };
        var context = Mock.Of<ServerCallContext>();

        _borrowingServiceMock
            .Setup(s => s.GetOverdueBooksAsync(
                It.IsAny<int>(),
                It.IsAny<int>()))
            .ThrowsAsync(new ValidationException("Page Size must be greater than 0"));

        var ex = await Assert.ThrowsAsync<RpcException>(() =>
            _grpcBorrowingService.GetOverdueBooks(request, context));

        Assert.Equal(StatusCode.InvalidArgument, ex.StatusCode);
    }

    [Fact]
    public async Task GetOverdueBooks_IfServiceMethodThrowsEntityNotFoundException_ShouldReturnNotFound()
    {
        var request = new OverdueBooksRequest
        {
            PageNumber = 1,
            PageSize = 10
        };
        var context = Mock.Of<ServerCallContext>();

        _borrowingServiceMock
            .Setup(s => s.GetOverdueBooksAsync(
                It.IsAny<int>(),
                It.IsAny<int>()))
            .ThrowsAsync(new EntityNotFoundException("No results match your search criteria."));

        var ex = await Assert.ThrowsAsync<RpcException>(() =>
            _grpcBorrowingService.GetOverdueBooks(request, context));

        Assert.Equal(StatusCode.NotFound, ex.StatusCode);
    }

    [Fact]
    public async Task GetOverdueBooks_IfServiceMethodThrowsIndexOutOfRangeException_ShouldReturnOutOfRange()
    {
        var request = new OverdueBooksRequest
        {
            PageNumber = 999,
            PageSize = 10
        };
        var context = Mock.Of<ServerCallContext>();

        _borrowingServiceMock
            .Setup(s => s.GetOverdueBooksAsync(
                It.IsAny<int>(),
                It.IsAny<int>()))
            .ThrowsAsync(new IndexOutOfRangeException("Page number must not exceed 12"));

        var ex = await Assert.ThrowsAsync<RpcException>(() =>
            _grpcBorrowingService.GetOverdueBooks(request, context));

        Assert.Equal(StatusCode.OutOfRange, ex.StatusCode);
    }
}
