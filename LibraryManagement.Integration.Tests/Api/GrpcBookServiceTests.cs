using AutoMapper;
using Azure;
using FluentValidation;
using Grpc.Core;
using Librarymanagement;
using LibraryManagement.Api.Mappings;
using LibraryManagement.Api.Services;
using LibraryManagement.Application.Mappings;
using LibraryManagement.Application.Services.DTOs.BookModels;
using LibraryManagement.Application.Services.Interaces;
using LibraryManagement.Shared.Exceptions;
using Microsoft.Extensions.Logging;
using Moq;

namespace LibraryManagement.Integration.Tests.Api;

public class GrpcBookServiceTests
{
    private readonly Mock<IBookService> _bookServiceMock;
    private readonly IMapper _mapper;
    private readonly GrpcBookService _grpcBookService;

    public GrpcBookServiceTests()
    {
        _bookServiceMock = new Mock<IBookService>();

        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<GrpcBookMappingProfile>();
        }, loggerFactory);

        _mapper = config.CreateMapper();

        config.AssertConfigurationIsValid();

        _grpcBookService = new GrpcBookService(_bookServiceMock.Object, _mapper);
    }

    [Fact]
    public async Task GetBook_IfBookExists_ShouldReturnDto()
    {
        var book = new BookDto
        {
            BookId = 1, 
            Title = "Test Book", 
            Isbn = "1234567890123",
            Description = "",
            AuthorId = 1,
            AuthorName = "Test Author",
            CategoryId = 1,
            CategoryName = "Test Category",
            PublishedDate = "2021-10-19",
            PageCount = 12,
            IsAvailable = true
        };

        _bookServiceMock.Setup(s => 
            s.GetBookAsync(1))
            .ReturnsAsync(book);
        
        var request = new BookGetRequest
        {
            BookId = 1
        };
        var context = Mock.Of<ServerCallContext>();

        var result = await _grpcBookService.GetBook(request, context);
        Assert.Equal(book.BookId, result.Book.BookId);

        _bookServiceMock.Verify(s =>
            s.GetBookAsync(1),
            Times.Once);
    }

    [Fact]
    public async Task GetBook_IfServiceMethodThrowsEntityNotFoundException_ShouldThrowNotFound()
    {
        var request = new BookGetRequest
        {
            BookId = 1
        };        
        var context = Mock.Of<ServerCallContext>();

        _bookServiceMock.Setup(s => 
            s.GetBookAsync(request.BookId))
            .ThrowsAsync(new EntityNotFoundException("No results match your search criteria."));

        var ex = await Assert.ThrowsAsync<RpcException>(() =>
            _grpcBookService.GetBook(request, context));
        
        Assert.Equal(StatusCode.NotFound, ex.StatusCode);
    }

    [Fact]
    public async Task CreateBook_IfBookIsCorrect_ShouldReturnEntity()
    {

        var request = new CreateBookRequest
        {
            Title = "Test Book", 
            Isbn = "1234567890123",
            Description = "",
            AuthorId = 1,
            CategoryId = 1,
            PublishedDate = "2021-10-19",
            PageCount = 12,
        };
        var book = new BookDto
        {
            BookId = 1, 
            Title = "Test Book", 
            Isbn = "1234567890123",
            Description = "",
            AuthorId = 1,
            AuthorName = "Test Author",
            CategoryId = 1,
            CategoryName = "Test Category",
            PublishedDate = "2021-10-19",
            PageCount = 12,
            IsAvailable = true
        };

        _bookServiceMock.Setup(s => 
            s.CreateBookAsync(
                It.IsAny<CreateBookCommand>()))
            .ReturnsAsync(book);
        
        var context = Mock.Of<ServerCallContext>();
        var result = await _grpcBookService.CreateBook(request, context);

        Assert.Equal(book.BookId, result.BookId);

        _bookServiceMock.Verify(s =>
            s.CreateBookAsync(It.IsAny<CreateBookCommand>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateBook_IfServiceMethodThrowsValidationException_ShouldThrowInvalidArgument()
    {
        _bookServiceMock.Setup(s => 
            s.CreateBookAsync(
                It.IsAny<CreateBookCommand>()))
            .ThrowsAsync(new ValidationException("Author with such Id doesn't exist"));
        
        var request = new CreateBookRequest();
        var context = Mock.Of<ServerCallContext>();

        var ex = await Assert.ThrowsAsync<RpcException>(() =>
            _grpcBookService.CreateBook(request, context));

        Assert.Equal(StatusCode.InvalidArgument, ex.StatusCode);
    }

    [Fact]
    public async Task DeleteBook_IfBookExists_ShouldReturnMessage()
    {

        var request = new BookDeleteRequest
        {
            BookId = 1
        };
        _bookServiceMock.Setup(s=>
            s.DeleteBookAsync(request.BookId));

        var context = Mock.Of<ServerCallContext>();

        var result = await _grpcBookService.DeleteBook(request, context);

        Assert.Equal("1 was successfully deleted.", result.Message);

        _bookServiceMock.Verify(s =>
            s.DeleteBookAsync(1),
            Times.Once);
    }

        [Fact]
    public async Task DeleteBook_IfServiceThrowsEntityNotFoundException_ShouldThrowNotFound()
    {
        var request = new BookDeleteRequest
        {
            BookId = 100
        };

        _bookServiceMock.Setup(s=>
            s.DeleteBookAsync(request.BookId))
            .ThrowsAsync(new EntityNotFoundException("No results match your search criteria."));

        var context = Mock.Of<ServerCallContext>();

        var ex = await Assert.ThrowsAsync<RpcException>(() => 
            _grpcBookService.DeleteBook(request, context));

        Assert.Equal(StatusCode.NotFound, ex.StatusCode);
    }

    [Fact]
    public async Task GetBooks_IfSearchResultIsExists_ShouldReturnCorrectResponse()
    {
        var request = new BookSearchRequest{
            PageNumber = 1,
            PageSize = 10
        };

        var books = new List<BookDto>
        {
            new () 
            {
                BookId = 1, 
                Title = "Test Book", 
                Isbn = "1234567890123",
                Description = "",
                AuthorId = 1,
                AuthorName = "Test Author",
                CategoryId = 1,
                CategoryName = "Test Category",
                PublishedDate = "2021-10-19",
                PageCount = 12,
                IsAvailable = true
            } 
        };

        _bookServiceMock.Setup(s => 
            s.GetBooksAsync(
                It.IsAny<SearchBookCommand>(), 
                request.PageSize, 
                request.PageNumber))
            .ReturnsAsync((1, 1, books));
        
        var context = Mock.Of<ServerCallContext>();

        var result = await _grpcBookService.GetBooks(request, context);

        Assert.Equal(1, result.TotalCount);
        Assert.Equal(10, result.PageSize);

        var book = result.Books.FirstOrDefault();

        Assert.Equal(1, book!.BookId);

        _bookServiceMock.Verify(s =>
            s.GetBooksAsync(It.IsAny<SearchBookCommand>(), 10, 1),
            Times.Once);
    }

    [Fact]
    public async Task GetBooks_WhenServiceThrowsValidationException_ShouldThrowInvalidArgument()
    {
        _bookServiceMock.Setup(s => 
            s.GetBooksAsync(
                It.IsAny<SearchBookCommand>(), 
                It.IsAny<int>(), 
                It.IsAny<int>()))
            .ThrowsAsync(new ValidationException("Author with such Id doesn't exist"));
        
        var request = new BookSearchRequest();
        var context = Mock.Of<ServerCallContext>();

        var ex = await Assert.ThrowsAsync<RpcException>(() =>
            _grpcBookService.GetBooks(request, context));

        Assert.Equal(StatusCode.InvalidArgument, ex.StatusCode);
    }

    [Fact]
    public async Task GetBooks_WhenServiceThrowsIndexOutOfRangeException_ShouldThrowOutOfRange()
    {
        _bookServiceMock.Setup(s => 
            s.GetBooksAsync(
                It.IsAny<SearchBookCommand>(), 
                It.IsAny<int>(), 
                It.IsAny<int>()))
            .ThrowsAsync(new IndexOutOfRangeException("Page number must not exceed 4"));
        
        var request = new BookSearchRequest();
        var context = Mock.Of<ServerCallContext>();

        var ex = await Assert.ThrowsAsync<RpcException>(() =>
            _grpcBookService.GetBooks(request, context));

        Assert.Equal(StatusCode.OutOfRange, ex.StatusCode);
    }

    [Fact]
    public async Task GetBooks_WhenServiceThrowsEntityNotFoundException_ShouldThrowNotFound()
    {
        _bookServiceMock.Setup(s => 
            s.GetBooksAsync(
                It.IsAny<SearchBookCommand>(), 
                It.IsAny<int>(), 
                It.IsAny<int>()))
            .ThrowsAsync(new EntityNotFoundException("No results match your search criteria."));
        
        var request = new BookSearchRequest();
        var context = Mock.Of<ServerCallContext>();

        var ex = await Assert.ThrowsAsync<RpcException>(() =>
            _grpcBookService.GetBooks(request, context));

        Assert.Equal(StatusCode.NotFound, ex.StatusCode);
    }

    [Fact]
    public async Task UpdateBook_WhenRequestIsCorrect_ShouldReturnDto()
    {
        var request = new UpdateBookRequest
        {
            BookId = 1, 
            Title = "Test Book", 
            PublishedDate = "2021-10-19",
            PageCount = 12,
        };
        var book = new BookDto
        {
            BookId = 1, 
            Title = "Test Book", 
            Isbn = "1234567890123",
            Description = "",
            AuthorId = 1,
            AuthorName = "Test Author",
            CategoryId = 1,
            CategoryName = "Test Category",
            PublishedDate = "2021-10-19",
            PageCount = 12,
            IsAvailable = true
        };

        _bookServiceMock.Setup(s => 
            s.UpdateBookAsync(
                It.IsAny<UpdateBookCommand>(), 
                request.BookId))
            .ReturnsAsync(book);
        
        var context = Mock.Of<ServerCallContext>();

        var result = await _grpcBookService.UpdateBook(request, context);

        Assert.Equal(book.BookId, result.BookId);

        _bookServiceMock.Verify(s =>
            s.UpdateBookAsync(It.IsAny<UpdateBookCommand>(), request.BookId),
            Times.Once);
    }

    [Fact]
    public async Task UpdateBook_WhenRequestIsIncorrect_ShouldThrow()
    {
        var request = new UpdateBookRequest
        {
            BookId = 1, 
            Title = "Test Book", 
            PublishedDate = "2021-10-19",
            PageCount = 12,
        };

        _bookServiceMock.Setup(s => 
            s.UpdateBookAsync(
                It.IsAny<UpdateBookCommand>(), 
                It.IsAny<long>()))
            .ThrowsAsync(new ValidationException("Category with such Id doesn't exist."));
        
        var context = Mock.Of<ServerCallContext>();

        var ex = await Assert.ThrowsAsync<RpcException>(() =>
            _grpcBookService.UpdateBook(request, context));

        Assert.Equal(StatusCode.InvalidArgument, ex.StatusCode);
    }
}
