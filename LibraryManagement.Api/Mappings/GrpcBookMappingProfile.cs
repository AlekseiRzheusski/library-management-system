using AutoMapper;
using Librarymanagement;
using LibraryManagement.Application.Mappings;
using LibraryManagement.Application.Services.DTOs.BookModels;

namespace LibraryManagement.Api.Mappings;

public class GrpcBookMappingProfile : Profile
{
    public GrpcBookMappingProfile()
    {
        CreateMap<BookDto, BookResponse>();
        CreateMap<CreateBookRequest, CreateBookCommand>();
        CreateMap<BookSearchRequest, SearchBookCommand>();
    }
}
