using AutoMapper;

using LibraryManagement.Domain.Entities;
using LibraryManagement.Application.Services.DTOs.AuthorModels;

public class AuthorMappingProfile : Profile
{
    public AuthorMappingProfile()
    {
        CreateMap<Author, AuthorDto>()
            .ForMember(dest => dest.BookCount,
                opt => opt.MapFrom(src => src.Books.Count)) 
            .ForMember(
                dest => dest.DateOfBirth,
                opt => opt.MapFrom(src => src.DateOfBirth.HasValue
                    ? src.DateOfBirth.Value.ToString("yyyy-MM-dd")
                    : string.Empty));
        
        //TODO check is we can use DateTimeOffset instead of DateTime
        CreateMap<CreateAuthorCommand, Author>()
            .ForMember(dest => dest.DateOfBirth,
                opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.DateOfBirth) ? DateTime.SpecifyKind(DateTime.Parse(src.DateOfBirth), DateTimeKind.Utc) : (DateTime?)null));
        
        CreateMap<UpdateAuthorCommand, Author>()
            .ForMember(dest => dest.DateOfBirth,
                opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.DateOfBirth) ? DateTime.SpecifyKind(DateTime.Parse(src.DateOfBirth), DateTimeKind.Utc) : (DateTime?)null))
            .ForAllMembers(
                opt => opt.Condition((src, dest, srcMember) => srcMember != null));;
    }
}
