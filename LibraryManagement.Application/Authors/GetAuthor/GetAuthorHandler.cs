using AutoMapper;
using LibraryManagement.Application.Services.DTOs.AuthorModels;
using LibraryManagement.Infrastructure.Repositories.Interfaces;
using LibraryManagement.Shared.Exceptions;
using MediatR;

namespace LibraryManagement.Application.Authors.GetAuthor;

public class GetAuthorHandler: IRequestHandler<GetAuthor, AuthorDto>
{
    private readonly IAuthorRepository _authorRepository;
    private readonly IMapper _mapper;
    public GetAuthorHandler(IAuthorRepository authorRepository, IMapper mapper)
    {
        _authorRepository = authorRepository;
        _mapper = mapper;
    }

    public async Task<AuthorDto> Handle(
        GetAuthor request,
        CancellationToken token
    )
    {
        var author = await _authorRepository.GetDetailedAuthorByIdAsync(request.AuthorId);
        if (author is null)
            throw new EntityNotFoundException($"Author with ID {request.AuthorId} does not exist");

        return _mapper.Map<AuthorDto>(author);
    }
}
