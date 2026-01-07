using Moq;
using Grpc.Core;
using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;

using Librarymanagement;
using LibraryManagement.Api.Services;
using LibraryManagement.Application.Services.Interaces;
using LibraryManagement.Shared.Exceptions;
using LibraryManagement.Api.Mappings;
using LibraryManagement.Application.Services.DTOs.CategoryModels;

namespace LibraryManagement.Integration.Tests.Api;

public class GrpcCategoryServiceTests
{
    private readonly Mock<ICategoryService> _categoryServiceMock;
    private readonly IMapper _mapper;
    private readonly GrpcCategoryService _grpcCategoryservice;
    public GrpcCategoryServiceTests()
    {
        _categoryServiceMock = new Mock<ICategoryService>();

        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<GrpcCategoryMappingProfile>();
        }, loggerFactory);

        mapperConfig.AssertConfigurationIsValid();
        _mapper = mapperConfig.CreateMapper();

        _grpcCategoryservice = new GrpcCategoryService(
            _categoryServiceMock.Object,
            _mapper);
    }

    [Fact]
    public async Task GetCategoryTree_IfCategoriesAreExist_ShouldReturnTree()
    {
        var tree = new List<CategoryTreeDto>
        {
            new()
            {
                CategoryId = 1,
                Name = "Root",
                Description = "Root",
                SortOrder = 1,
                IsActive =true,
                BookCount = 1,
                SubCategories =
                {
                    new CategoryTreeDto {
                        CategoryId = 2,
                        Name = "Child",
                        Description = "Child",
                        ParentCategoryId = 1,
                        ParentCategoryName = "Root",
                        SortOrder = 2,
                        IsActive = true,
                        BookCount = 2
                    }
                }
            }
        };

        _categoryServiceMock
            .Setup(s => s.GetCategoryTreeAsync())
            .ReturnsAsync(tree);

        var context = Mock.Of<ServerCallContext>();

        var result = await _grpcCategoryservice.GetCategoryTree(new CategoryTreeRequest(), context);

        Assert.Single(result.Categories);
        Assert.Equal(tree[0].Name, result.Categories[0].Name);
        Assert.Equal(tree[0].CategoryId, result.Categories[0].CategoryId);

        _categoryServiceMock.Verify(s =>
            s.GetCategoryTreeAsync(),
            Times.Once);
    }

    [Fact]
    public async Task GetCategories_IfCategoriesAreExist_ShouldReturnList()
    {
        var categories = new List<CategoryDto>
        {
            new()
            {
                CategoryId = 2,
                Name = "Child",
                Description = "Child",
                ParentCategoryId = 1,
                ParentCategoryName = "Root",
                SortOrder = 2,
                IsActive = true,
                BookCount = 2
            }
        };

        _categoryServiceMock
            .Setup(s => s.GetCategoriesAsync(It.IsAny<SearchCategoryCommand>()))
            .ReturnsAsync(categories);

        var context = Mock.Of<ServerCallContext>();

        var result = await _grpcCategoryservice.GetCategories(new CategorySearchRequest(), context);

        Assert.Single(result.Categories);
        Assert.Equal(categories[0].Name, result.Categories[0].Name);
        Assert.Equal(categories[0].CategoryId, result.Categories[0].CategoryId);

        _categoryServiceMock.Verify(s =>
            s.GetCategoriesAsync(It.IsAny<SearchCategoryCommand>()),
            Times.Once);
    }

    [Fact]
    public async Task GetCategories_IfServiceMethodThrowsValidationException_ShouldThrowInvalidArgument()
    {
        _categoryServiceMock
            .Setup(s => s.GetCategoriesAsync(It.IsAny<SearchCategoryCommand>()))
            .ThrowsAsync(new ValidationException("Category with such Id doesn't exist."));

        var context = Mock.Of<ServerCallContext>();

        var ex = await Assert.ThrowsAsync<RpcException>(() =>
            _grpcCategoryservice.GetCategories(new CategorySearchRequest(), context));

        Assert.Equal(StatusCode.InvalidArgument, ex.StatusCode);
    }

    [Fact]
    public async Task GetCategories_IfServiceMethodThrowsEntityNotFoundException_ShouldThrowNotFound()
    {
        _categoryServiceMock
            .Setup(s => s.GetCategoriesAsync(It.IsAny<SearchCategoryCommand>()))
            .ThrowsAsync(new EntityNotFoundException("No results match your search criteria."));

        var context = Mock.Of<ServerCallContext>();

        var ex = await Assert.ThrowsAsync<RpcException>(() =>
            _grpcCategoryservice.GetCategories(new CategorySearchRequest(), context));

        Assert.Equal(StatusCode.NotFound, ex.StatusCode);
    }

    [Fact]
    public async Task CreateCategory_IfCategoryIsCorrect_ShouldReturnCategory()
    {
        var category = new CategoryDto
        {
            CategoryId = 1,
            Name = "Sci-Fi",
            Description = "Sci-Fi",
            ParentCategoryId = 1,
            ParentCategoryName = "Root",
            SortOrder = 3,
            IsActive = true,
            BookCount = 0
        };

        _categoryServiceMock
            .Setup(s => s.CreateCategoryAsync(It.IsAny<CreateCategoryCommand>()))
            .ReturnsAsync(category);

        var context = Mock.Of<ServerCallContext>();

        var result = await _grpcCategoryservice.CreateCategory(new CreateCategoryRequest(), context);

        Assert.Equal(category.Name, result.Name);
        Assert.Equal(category.CategoryId, result.CategoryId);

        _categoryServiceMock.Verify(s =>
            s.CreateCategoryAsync(It.IsAny<CreateCategoryCommand>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateCategory_IfServiceMethodThrowsValidationException_ShouldThrowInvalidArgument()
    {
        _categoryServiceMock
            .Setup(s => s.CreateCategoryAsync(It.IsAny<CreateCategoryCommand>()))
            .ThrowsAsync(new ValidationException("Category with such Id doesn't exist."));

        var context = Mock.Of<ServerCallContext>();

        var ex = await Assert.ThrowsAsync<RpcException>(() =>
            _grpcCategoryservice.CreateCategory(new CreateCategoryRequest(), context));

        Assert.Equal(StatusCode.InvalidArgument, ex.StatusCode);
    }

    [Fact]
    public async Task DeleteCategory_IfCategoryExists_ShouldReturnMessage()
    {
        _categoryServiceMock
            .Setup(s => s.DeleteCategoryAsync(1))
            .Returns(Task.CompletedTask);

        var context = Mock.Of<ServerCallContext>();

        var result = await _grpcCategoryservice.DeleteCategory(
            new CategoryDeleteRequest { CategoryId = 1 },
            context);

        Assert.Contains("successfully deleted", result.Message);

        _categoryServiceMock.Verify(s =>
            s.DeleteCategoryAsync(1),
            Times.Once);
    }

    [Fact]
    public async Task DeleteCategory_IfServiceMethodThrowsValidationException_ShouldThrowInvalidArgument()
    {
        _categoryServiceMock
            .Setup(s => s.DeleteCategoryAsync(It.IsAny<long>()))
            .ThrowsAsync(new ValidationException("This category has related subcategories"));

        var context = Mock.Of<ServerCallContext>();

        var ex = await Assert.ThrowsAsync<RpcException>(() =>
            _grpcCategoryservice.DeleteCategory(new CategoryDeleteRequest { CategoryId = 1 }, context));

        Assert.Equal(StatusCode.InvalidArgument, ex.StatusCode);
    }

    [Fact]
    public async Task DeleteCategory_IfServiceMethodThrowsEntityNotFoundExceptionShouldReturnNotFound()
    {
        _categoryServiceMock
            .Setup(s => s.DeleteCategoryAsync(It.IsAny<long>()))
            .ThrowsAsync(new EntityNotFoundException("Category with ID 1 does not exist"));
        
        var context = Mock.Of<ServerCallContext>();

        var ex = await Assert.ThrowsAsync<RpcException>(() =>
            _grpcCategoryservice.DeleteCategory(new CategoryDeleteRequest { CategoryId = 1 }, context));

        Assert.Equal(StatusCode.NotFound, ex.StatusCode);
    }
}
