using AutoMapper;
using Librarymanagement;
using LibraryManagement.Application.Services.DTOs.BorrowingModels;

namespace LibraryManagement.Api.Mappings;
public class GrpcBorrowingMappingProfile: Profile
{
    public GrpcBorrowingMappingProfile()
    {
        CreateMap<BorrowingDto, BorrowingResponse>();
        CreateMap<BorrowBookRequest, BorrowBookCommand>();
        CreateMap<UserBorrowingsRequest, UserBorrowingsCommand>();
    }
}
