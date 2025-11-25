using System.Collections;
using AutoMapper;
using FluentValidation;
using Grpc.Core;
using Librarymanagement;
using LibraryManagement.Application.Services.DTOs.CategoryModels;
using LibraryManagement.Application.Services.Interaces;
using LibraryManagement.Shared.Exceptions;

namespace LibraryManagement.Api.Services;

public class GrpcCategoryService : CategoryService.CategoryServiceBase
{
    private readonly ICategoryService _categoryService;
    private readonly IMapper _mapper;

    public GrpcCategoryService(ICategoryService categoryService, IMapper mapper)
    {
        _categoryService = categoryService;
        _mapper = mapper;
    }

    public override async Task<CategoryTreeListResponse> GetCategoryTree(CategoryTreeRequest request, ServerCallContext context)
    {
        try
        {
            var categoryTreeDtos = await _categoryService.GetCategoryTreeAsync();

            var categoryTree = _mapper.Map<List<CategoryTreeResponse>>(categoryTreeDtos);

            var response = new CategoryTreeListResponse();
            response.Categories.AddRange(categoryTree);

            return response;
        }
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.Internal, ex.Message));
        }
    }

    public override async Task<CategoryListResponse> GetCategories(CategorySearchRequest request, ServerCallContext context)
    {
        try
        {
            var categorySearchCommand = _mapper.Map<SearchCategoryCommand>(request);

            var searchResultDtos = await _categoryService.GetCategoriesAsync(categorySearchCommand);

            var searchResult = _mapper.Map<IEnumerable<CategoryResponse>>(searchResultDtos);

            var response = new CategoryListResponse();
            response.Categories.AddRange(searchResult);

            return response;
        }
        catch (ValidationException ex)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
        }
        catch (EntityNotFoundException ex)
        {
            throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
        }
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.Internal, ex.Message));
        }
    }

    public override async Task<CategoryResponse> CreateCategory(CreateCategoryRequest request, ServerCallContext context)
    {
        try
        {
            var createCategoryCommand = _mapper.Map<CreateCategoryCommand>(request);

            var newCategoryDto = await _categoryService.CreateCategoryAsync(createCategoryCommand);

            return _mapper.Map<CategoryResponse>(newCategoryDto);
        }
        catch (ValidationException ex)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
        }
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.Internal, ex.Message));
        }
    }
}
