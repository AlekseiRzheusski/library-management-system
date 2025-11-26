using AutoMapper;
using FluentValidation;

using LibraryManagement.Shared.Exceptions;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Infrastructure.Repositories.Interfaces;
using LibraryManagement.Application.Services.Interaces;
using LibraryManagement.Application.Services.DTOs.AuthorModels;

namespace LibraryManagement.Application.Services;

public class AuthorService : IAuthorService
{
    private readonly IMapper _mapper;
    private readonly IAuthorRepository _authorRepository;
    private readonly IValidator<SearchAuthorCommand> _searchAuthorCommandValidator;
    private readonly IValidator<CreateAuthorCommand> _createAuthorCommandValidator;
    private readonly IValidator<UpdateAuthorCommand> _updateAuthorCommandValidator;
    private readonly ISearchService<Author> _authorSearchService;

    public AuthorService(
        IMapper mapper,
        IAuthorRepository authorRepository,
        IValidator<SearchAuthorCommand> searchAuthorCommandValidator,
        IValidator<CreateAuthorCommand> createAuthorCommandValidator,
        IValidator<UpdateAuthorCommand> updateAuthorCommandValidator,
        ISearchService<Author> authorSearchService
    )
    {
        _mapper = mapper;
        _authorRepository = authorRepository;
        _searchAuthorCommandValidator = searchAuthorCommandValidator;
        _createAuthorCommandValidator = createAuthorCommandValidator;
        _updateAuthorCommandValidator = updateAuthorCommandValidator;
        _authorSearchService = authorSearchService;
    }

    public async Task<AuthorDto> GetAuthorAsync(long authorId)
    {
        var author = await _authorRepository.GetDetailedAuthorByIdAsync(authorId);
        if (author is null)
            throw new EntityNotFoundException($"Author with ID {authorId} does not exist");

        return _mapper.Map<AuthorDto>(author);
    }

    public async Task<(int totalCount, int numberOfPages, IEnumerable<AuthorDto>)> GetAuthorsAsync(
        SearchAuthorCommand command, 
        int pageSize, 
        int pageNumber)
    {
        if (pageSize <= 0)
        {
            throw new ValidationException("Page Size must be greater than 0");
        }

        var validation = await _searchAuthorCommandValidator.ValidateAsync(command);
        if (!validation.IsValid)
        {
            var message = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage));
            throw new ValidationException(message);
        }

        var expression = _authorSearchService.BuildExpression<SearchAuthorCommand>(command);

        int totalCount = await _authorRepository.GetQueryCountAsync(expression);
        int maxPageNumber = (int)Math.Ceiling((double)totalCount / pageSize);

        if (totalCount > 0 && (pageNumber < 0 || pageNumber > maxPageNumber))
            throw new IndexOutOfRangeException($"Page number must not exceed {maxPageNumber}");

        var result = await _authorRepository.FindAuthorAsync(expression, pageSize, pageNumber);

        if (!result.Any())
        {
            throw new EntityNotFoundException("No results match your search criteria.");
        }

        var resultDtoPage =  _mapper.Map<IEnumerable<AuthorDto>>(result);
        return (totalCount, maxPageNumber, resultDtoPage);
    }

    public async Task<AuthorDto> CreateAuthorAsync(CreateAuthorCommand command)
    {
        var validation = await _createAuthorCommandValidator.ValidateAsync(command);
        if (!validation.IsValid)
        {
            var message = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage));
            throw new ValidationException(message);
        }

        var newAuthor = _mapper.Map<Author>(command);
        await _authorRepository.AddAsync(newAuthor);
        await _authorRepository.SaveAsync();

        var detailedAuthor = await _authorRepository.GetDetailedAuthorByIdAsync(newAuthor.AuthorId);
        var newAuthorDto = _mapper.Map<AuthorDto>(detailedAuthor);

        return newAuthorDto;
    }

    public async Task<AuthorDto> UpdateAuthorAsync(UpdateAuthorCommand command, long authorId)
    {
        var validation = await _updateAuthorCommandValidator.ValidateAsync(command);
        if (!validation.IsValid)
        {
            var message = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage));
            throw new ValidationException(message);
        }

        var author = await _authorRepository.GetByIdAsync(authorId);
        if (author is null)
        {
            throw new EntityNotFoundException("No results match your search criteria.");
        }

        _mapper.Map(command, author);
        await _authorRepository.SaveAsync();

        var detailedAuthor = await _authorRepository.GetDetailedAuthorByIdAsync(authorId);

        return _mapper.Map<AuthorDto>(detailedAuthor);
    }
}
