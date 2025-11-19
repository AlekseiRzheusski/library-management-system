using AutoMapper;

using LibraryManagement.Domain.Entities;
using LibraryManagement.Application.Services.DTOs.BookModels;

namespace LibraryManagement.Application.Mappings;

public class BookMappingProfile : Profile
{
    public BookMappingProfile()
    {
        CreateMap<Book, BookDto>()
            .ForMember(
                dest => dest.AuthorId,
                opt => opt.MapFrom(src => src.Author != null ? src.Author.AuthorId : 0))
            .ForMember(
                dest => dest.AuthorName,
                opt => opt.MapFrom(src => src.Author != null ? $"{src.Author.FirstName} {src.Author.LastName}" : string.Empty))
            .ForMember(
                dest => dest.CategoryId,
                opt => opt.MapFrom(src => src.Category != null ? src.Category.CategoryId : 0))
            .ForMember(
                dest => dest.CategoryName,
                opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty))
            .ForMember(
                dest => dest.PublishedDate,
                opt => opt.MapFrom(src => src.PublishedDate.HasValue
                    ? src.PublishedDate.Value.ToString("yyyy-MM-dd")
                    : string.Empty));

        CreateMap<CreateBookCommand, Book>()
            .ForMember(dest => dest.PublishedDate,
                opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.PublishedDate) ? DateTime.Parse(src.PublishedDate) : (DateTime?)null));

        CreateMap<UpdateBookCommand, Book>()
            .ForMember(dest => dest.PublishedDate,
                opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.PublishedDate) ? DateTime.Parse(src.PublishedDate) : (DateTime?)null))
            .ForMember(dest => dest.CategoryId,
                opt => opt.MapFrom((src, dest) => src.CategoryId ?? dest.CategoryId))
            .ForAllMembers(
                opt => opt.Condition((src, dest, srcMember) => srcMember != null));
    }
}
