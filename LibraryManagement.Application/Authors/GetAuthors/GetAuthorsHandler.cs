using AutoMapper;
using FluentValidation;
using LibraryManagement.Application.Services.DTOs.AuthorModels;
using LibraryManagement.Application.Services.Interaces;
using LibraryManagement.Application.Validation;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Infrastructure.Repositories.Interfaces;
using LibraryManagement.Shared.Exceptions;
using MediatR;

namespace LibraryManagement.Application.Authors.GetAuthors;

public class GetAuthorsHandler : IRequestHandler<GetAuthors, (int, int, IEnumerable<AuthorDto>)>
{
    private readonly IMapper _mapper;
    private readonly IValidator<SearchAuthorCommand> _searchAuthorCommandValidator;
    private readonly ISearchService<Author> _authorSearchService;
    private readonly IAuthorRepository _authorRepository;

    public GetAuthorsHandler(
        IMapper mapper,
        IValidator<SearchAuthorCommand> searchAuthorCommandValidator,
        ISearchService<Author> authorSearchService,
        IAuthorRepository authorRepository)
    {
        _mapper = mapper;
        _searchAuthorCommandValidator = searchAuthorCommandValidator;
        _authorSearchService = authorSearchService;
        _authorRepository = authorRepository;
    }

    public async Task<(int, int, IEnumerable<AuthorDto>)> Handle(
        GetAuthors request,
        CancellationToken cancellationToken)
    {
        if (request.PageSize <= 0)
        {
            throw new ValidationException("Page Size must be greater than 0");
        }

        var validation = await _searchAuthorCommandValidator.ValidateAsync(request.Command);
        if (!validation.IsValid)
        {
            var message = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage));
            throw new ValidationException(message);
        }

        var expression = _authorSearchService.BuildExpression<SearchAuthorCommand>(request.Command);

        int totalCount = await _authorRepository.GetQueryCountAsync(expression);
        int maxPageNumber = (int)Math.Ceiling((double)totalCount / request.PageSize);

        if (totalCount > 0 && (request.PageNumber < 0 || request.PageNumber > maxPageNumber))
            throw new IndexOutOfRangeException($"Page number must not exceed {maxPageNumber}");

        var result = await _authorRepository.FindDetaliedEntitiesPageAsync(expression, request.PageSize, request.PageNumber);

        if (!result.Any())
        {
            throw new EntityNotFoundException("No results match your search criteria.");
        }

        var resultDtoPage =  _mapper.Map<IEnumerable<AuthorDto>>(result);
        return (totalCount, maxPageNumber, resultDtoPage);
    }
}
