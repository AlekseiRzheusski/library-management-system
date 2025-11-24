using AutoMapper;

using LibraryManagement.Application.Services.DTOs.CategoryModels;
using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Application.Mappings;

public class CategoryMappingProfile: Profile
{
    public CategoryMappingProfile()
    {
        CreateMap<Category, CategoryTreeDto>()
            .ForMember(
                dest => dest.Description, 
                opt => opt.MapFrom(src => src.Description != null ? src.Description : string.Empty))
            .ForMember(dest => dest.BookCount,
                opt => opt.MapFrom(src => src.Books.Count)) 
            .ForMember(
                dest => dest.ParentCategoryName,
                opt => opt.MapFrom(src => src.ParentCategory != null ? src.ParentCategory.Name : null))
            .ForMember(dest => dest.SubCategories,
                opt => opt.MapFrom(src => src.SubCategories))
            .PreserveReferences();
        
        CreateMap<Category, CategoryDto>()
            .ForMember(
                dest => dest.Description, 
                opt => opt.MapFrom(src => src.Description != null ? src.Description : string.Empty))
            .ForMember(dest => dest.BookCount,
                opt => opt.MapFrom(src => src.Books.Count)) 
            .ForMember(
                dest => dest.ParentCategoryName,
                opt => opt.MapFrom(src => src.ParentCategory != null ? src.ParentCategory.Name : null));
            
        CreateMap<CreateCategoryCommand, Category>();
    }
}
