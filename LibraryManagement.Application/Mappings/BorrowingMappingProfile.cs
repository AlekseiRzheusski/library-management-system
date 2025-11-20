using AutoMapper;
using LibraryManagement.Application.Services.DTOs.BorrowingModels;
using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Application.Mappings;

public class BorrowingMappingProfile : Profile
{
    public BorrowingMappingProfile()
    {
        CreateMap<Borrowing, BorrowingDto>()
            .ForMember(
                dest => dest.BookId,
                opt => opt.MapFrom(src => src.Book != null ? src.Book.BookId : 0))
            .ForMember(
                dest => dest.BookTitle,
                opt => opt.MapFrom(src => src.Book != null ? src.Book.Title : ""))
            .ForMember(
                dest => dest.BorrowDate,
                opt => opt.MapFrom(src => src.BorrowDate.ToString("yyyy-MM-dd")))
            .ForMember(
                dest => dest.DueDate,
                opt => opt.MapFrom(src => src.DueDate.ToString("yyyy-MM-dd")))
            .ForMember(dest => dest.ReturnDate,
                opt => opt.MapFrom(src => src.ReturnDate.HasValue
                    ? src.ReturnDate.Value.ToString("yyyy-MM-dd")
                    : string.Empty))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
    }
}
