using Moq;
using AutoMapper;
using Grpc.Core;
using MediatR;
using FluentValidation;
using Microsoft.Extensions.Logging;

using Librarymanagement;
using LibraryManagement.Api.Mappings;
using LibraryManagement.Application.Authors.CreateAuthor;
using LibraryManagement.Application.Authors.DeleteAuthor;
using LibraryManagement.Application.Authors.GetAuthor;
using LibraryManagement.Application.Authors.GetAuthors;
using LibraryManagement.Application.Authors.UpdateAuthor;
using LibraryManagement.Application.Services.DTOs.AuthorModels;
using LibraryManagement.Shared.Exceptions;

namespace LibraryManagement.Integration.Tests.Api;

public class GrpcAuthorServiceTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly IMapper _mapper;
    private readonly GrpcAuthorService _grpcAuthorService;

    public GrpcAuthorServiceTests()
    {
        _mediatorMock = new Mock<IMediator>();
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<GrpcAuthorMappingProfile>();
        }, loggerFactory);

        _mapper = config.CreateMapper();

        config.AssertConfigurationIsValid();

        _grpcAuthorService = new GrpcAuthorService(_mediatorMock.Object, _mapper);
    }

    [Fact]
    public async Task GetAuthor_IfAuthorExists_ShouldReturnDto()
    {
        var request = new AuthorGetRequest
        {
            AuthorId = 1
        };

        var author = new AuthorDto
        {
            AuthorId = 1,
            FirstName = "Anton",
            LastName = "Chekhov",
            Biography = "Was a Russian playwright and short-story writer, widely considered to be one of the greatest writers of all time.",
            DateOfBirth = "1860-01-29",
            IsActive = false
        };
        var context = Mock.Of<ServerCallContext>();

        _mediatorMock
            .Setup(s => s.Send(
                It.IsAny<GetAuthor>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);

        var result = await _grpcAuthorService.GetAuthor(request, context);

        Assert.NotNull(result);
        Assert.Equal(100, result.Author.AuthorId);

        _mediatorMock.Verify(s =>
            s.Send(
                It.Is<GetAuthor>(q => q.AuthorId == 1),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetAuthor_WhenServiceThrowsEntityNotFoundException_ShouldThrowNotFound()
    {
        var request = new AuthorGetRequest { AuthorId = 99 };
        var context = Mock.Of<ServerCallContext>();

        _mediatorMock
            .Setup(s => s.Send(
                It.IsAny<GetAuthor>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new EntityNotFoundException($"Author with ID {request.AuthorId} does not exist"));

        var ex = await Assert.ThrowsAsync<RpcException>(() =>
            _grpcAuthorService.GetAuthor(request, context));

        Assert.Equal(StatusCode.NotFound, ex.StatusCode);
    }

    [Fact]
    public async Task GetAuthors_IfAuthorsExist_ShouldReturnPage()
    {
        var request = new AuthorPageRequest
        {
            SearchRequest = new AuthorSearchRequest(),
            PageNumber = 10,
            PageSize = 1
        };

        var authors = new List<AuthorDto>
        {
            new()
            {
                AuthorId = 1,
                FirstName = "Anton",
                LastName = "Chekhov",
                Biography = "Was a Russian playwright and short-story writer, widely considered to be one of the greatest writers of all time.",
                DateOfBirth = "1860-01-29",
                IsActive = false
            }
        };

        var context = Mock.Of<ServerCallContext>();

        _mediatorMock
            .Setup(s => s.Send(
                It.IsAny<GetAuthors>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((10, 10, authors));

        var result = await _grpcAuthorService.GetAuthors(request, context);

        Assert.NotNull(result);
        Assert.Equal(10, result.TotalCount);
        Assert.Equal(1, result.PageSize);

        var author = result.Authors.FirstOrDefault();

        Assert.Equal(1, author!.AuthorId);

        _mediatorMock.Verify(s =>
            s.Send(
                It.Is<GetAuthors>(q => q.PageNumber == 10 && q.PageSize == 1),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetAuthors_WhenServiceThrowsValidationException_ShouldThrowInvalidArgument()
    {
        var request = new AuthorPageRequest();
        var context = Mock.Of<ServerCallContext>();

        _mediatorMock
            .Setup(s => s.Send(It.IsAny<GetAuthors>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ValidationException("This date cannot be parsed"));

        var ex = await Assert.ThrowsAsync<RpcException>(() =>
            _grpcAuthorService.GetAuthors(request, context));

        Assert.Equal(StatusCode.InvalidArgument, ex.StatusCode);
    }

    [Fact]
    public async Task GetAuthors_WhenServiceThrowsIndexOutOfRangeException_ShouldThrowOutOfRange()
    {
        var request = new AuthorPageRequest();
        var context = Mock.Of<ServerCallContext>();

        _mediatorMock
            .Setup(s => s.Send(It.IsAny<GetAuthors>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new IndexOutOfRangeException());

        var ex = await Assert.ThrowsAsync<RpcException>(() =>
            _grpcAuthorService.GetAuthors(request, context));

        Assert.Equal(StatusCode.OutOfRange, ex.StatusCode);
    }

    [Fact]
    public async Task GetAuthors_WhenServiceThrowsEntityNotFoundException_ShouldThrowNotFound()
    {
        var request = new AuthorPageRequest();
        var context = Mock.Of<ServerCallContext>();

        _mediatorMock
            .Setup(s => s.Send(It.IsAny<GetAuthors>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new EntityNotFoundException("No results match your search criteria."));

        var ex = await Assert.ThrowsAsync<RpcException>(() =>
            _grpcAuthorService.GetAuthors(request, context));

        Assert.Equal(StatusCode.NotFound, ex.StatusCode);
    }

    [Fact]
    public async Task CreateAuthor_IfRequestIsCorrect_ShouldReturnAuthor()
    {
        var request = new CreateAuthorRequest
        {
            FirstName = "Leo",
            LastName = "Tolstoy",
            Biography = "Russian writer",
            DateOfBirth = "1828-09-09",
        };

        var createdAuthor = new AuthorDto
        {
            AuthorId = 2,
            FirstName = "Leo",
            LastName = "Tolstoy",
            Biography = "Russian writer",
            DateOfBirth = "1828-09-09",
            IsActive = true
        };

        var context = Mock.Of<ServerCallContext>();

        _mediatorMock
            .Setup(s => s.Send(
                It.IsAny<CreateAuthor>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdAuthor);

        var result = await _grpcAuthorService.CreateAuthor(request, context);

        Assert.Equal(2, result.AuthorId);

        _mediatorMock.Verify(s =>
            s.Send(It.Is<CreateAuthor>(q => q.Command.FirstName == request.FirstName),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAuthor_WhenServiceThrowsValidationException_ShouldThrowInvalidArgument()
    {
        var request = new CreateAuthorRequest();
        var context = Mock.Of<ServerCallContext>();

        _mediatorMock
            .Setup(s => s.Send(It.IsAny<CreateAuthor>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ValidationException("This date cannot be parsed"));

        var ex = await Assert.ThrowsAsync<RpcException>(() =>
            _grpcAuthorService.CreateAuthor(request, context));

        Assert.Equal(StatusCode.InvalidArgument, ex.StatusCode);
    }

    [Fact]
    public async Task UpdateAuthor_IfAuthorExists_ShouldReturnUpdatedAuthor()
    {
        var request = new UpdateAuthorRequest
        {
            AuthorId = 3,
            FirstName = "Updated",
            LastName = "Author",
            Biography = "Updated bio",
            DateOfBirth = "1900-01-01",
            IsActive = true
        };

        var updatedAuthor = new AuthorDto
        {
            AuthorId = 3,
            FirstName = "Updated",
            LastName = "Author",
            Biography = "Updated bio",
            DateOfBirth = "1900-01-01",
            IsActive = true
        };

        var context = Mock.Of<ServerCallContext>();

        _mediatorMock
            .Setup(s => s.Send(
                It.IsAny<UpdateAuthor>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(updatedAuthor);

        var result = await _grpcAuthorService.UpdateAuthor(request, context);

        Assert.Equal(3, result.AuthorId);
        Assert.Equal("Updated", result.FirstName);

        _mediatorMock.Verify(s =>
            s.Send(
                It.Is<UpdateAuthor>(q => q.authorId == 3 && q.Command.FirstName == request.FirstName),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAuthor_WhenServiceThrowsValidationException_ShouldThrowInvalidArgument()
    {
        var request = new UpdateAuthorRequest { AuthorId = 1 };
        var context = Mock.Of<ServerCallContext>();

        _mediatorMock
            .Setup(s => s.Send(It.IsAny<UpdateAuthor>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ValidationException("This date cannot be parsed"));

        var ex = await Assert.ThrowsAsync<RpcException>(() =>
            _grpcAuthorService.UpdateAuthor(request, context));

        Assert.Equal(StatusCode.InvalidArgument, ex.StatusCode);
    }

    [Fact]
    public async Task UpdateAuthor_WhenServiceThrowsEntityNotFoundException_ShouldThrowNotFound()
    {
        var request = new UpdateAuthorRequest { AuthorId = 1 };
        var context = Mock.Of<ServerCallContext>();

        _mediatorMock
            .Setup(s => s.Send(It.IsAny<UpdateAuthor>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new EntityNotFoundException("No results match your search criteria."));

        var ex = await Assert.ThrowsAsync<RpcException>(() =>
            _grpcAuthorService.UpdateAuthor(request, context));

        Assert.Equal(StatusCode.NotFound, ex.StatusCode);
    }

    [Fact]
    public async Task DeleteAuthor_IfAuthorExists_ShouldReturnSuccessMessage()
    {
        var request = new AuthorDeleteRequest
        {
            AuthorId = 7
        };

        var context = Mock.Of<ServerCallContext>();

        _mediatorMock
            .Setup(s => s.Send(
                It.IsAny<DeleteAuthor>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _grpcAuthorService.DeleteAuthor(request, context);

        Assert.Contains("successfully deleted", result.Message);

        _mediatorMock.Verify(s =>
            s.Send(
                It.Is<DeleteAuthor>(q => q.authorId == 7),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAuthor_WhenServiceValidationException_ShouldThrowInvalidArgument()
    {
        var request = new AuthorDeleteRequest { AuthorId = 1 };
        var context = Mock.Of<ServerCallContext>();

        _mediatorMock
            .Setup(s => s.Send(It.IsAny<DeleteAuthor>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ValidationException("This author has related books"));

        var ex = await Assert.ThrowsAsync<RpcException>(() =>
            _grpcAuthorService.DeleteAuthor(request, context));

        Assert.Equal(StatusCode.InvalidArgument, ex.StatusCode);
    }

    [Fact]
    public async Task DeleteAuthor_WhenServiceThrowsEntityNotFoundException_ShouldThrowNotFound()
    {
        var request = new AuthorDeleteRequest { AuthorId = 99 };
        var context = Mock.Of<ServerCallContext>();

        _mediatorMock
            .Setup(s => s.Send(It.IsAny<DeleteAuthor>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new EntityNotFoundException($"Author with ID {request.AuthorId} does not exist"));

        var ex = await Assert.ThrowsAsync<RpcException>(() =>
            _grpcAuthorService.DeleteAuthor(request, context));

        Assert.Equal(StatusCode.NotFound, ex.StatusCode);
    }
}
