using AutoMapper;
using Librarymanagement;
using LibraryManagement.Application.Services.DTOs.AuthorModels;

namespace LibraryManagement.Api.Mappings;

public class GrpcAuthorMappingProfile : Profile
{
    public GrpcAuthorMappingProfile()
    {
        CreateMap<AuthorDto, AuthorResponse>();
        CreateMap<AuthorSearchRequest, SearchAuthorCommand>();
        CreateMap<CreateAuthorRequest, CreateAuthorCommand>();
        CreateMap<UpdateAuthorRequest, UpdateAuthorCommand>();
    }
}
