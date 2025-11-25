using AutoMapper;
using Librarymanagement;
using LibraryManagement.Application.Services.DTOs.CategoryModels;

namespace LibraryManagement.Api.Mappings;
public class GrpcCategoryMappingProfile: Profile
{
    public GrpcCategoryMappingProfile()
    {
        CreateMap<CategoryTreeDto, CategoryTreeResponse>()
            .ForMember(dest => dest.SubCategories,
                opt => opt.MapFrom(src => src.SubCategories))
            .PreserveReferences();
        
        CreateMap<CategorySearchRequest, SearchCategoryCommand>();
        CreateMap<CategoryDto, CategoryResponse>();
        CreateMap<CreateCategoryRequest, CreateCategoryCommand>();
    }
}
