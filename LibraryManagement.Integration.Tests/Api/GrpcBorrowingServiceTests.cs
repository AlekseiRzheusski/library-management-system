using Moq;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Grpc.Core;

using Librarymanagement;
using LibraryManagement.Api.Mappings;
using LibraryManagement.Api.Services;
using LibraryManagement.Application.Services.DTOs.BorrowingModels;
using LibraryManagement.Application.Services.Interaces;

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
}
