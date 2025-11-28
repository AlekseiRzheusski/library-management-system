using AutoMapper;
using FluentValidation;
using LibraryManagement.Application.Authors.UpdateAuthor;
using LibraryManagement.Application.Services.DTOs.AuthorModels;
using LibraryManagement.Application.Validation;
using LibraryManagement.Shared.Exceptions;
using LibraryManagement.Infrastructure.Repositories.Interfaces;
using MediatR;

namespace LibraryManagement.Application.Authors.UpdateAuthor;

public class UpdateAuthorHandler : IRequestHandler<UpdateAuthor, AuthorDto>
{
    private readonly IMapper _mapper;
    private readonly IAuthorRepository _authorRepository;
    private readonly IValidator<UpdateAuthorCommand> _updateAuthorCommandValidator;

    public UpdateAuthorHandler(
        IMapper mapper,
        IAuthorRepository authorRepository,
        IValidator<UpdateAuthorCommand> updateAuthorCommandValidator
    )
    {
        _mapper = mapper;
        _authorRepository = authorRepository;
        _updateAuthorCommandValidator = updateAuthorCommandValidator;
    }

    public async Task<AuthorDto> Handle(
        UpdateAuthor request, 
        CancellationToken cancellationToken)
    {
        var validation = await _updateAuthorCommandValidator.ValidateAsync(request.Command);
        if (!validation.IsValid)
        {
            var message = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage));
            throw new ValidationException(message);
        }

        var author = await _authorRepository.GetByIdAsync(request.authorId);
        if (author is null)
        {
            throw new EntityNotFoundException("No results match your search criteria.");
        }

        _mapper.Map(request.Command, author);
        await _authorRepository.SaveAsync();

        var detailedAuthor = await _authorRepository.GetDetailedAuthorByIdAsync(request.authorId);

        return _mapper.Map<AuthorDto>(detailedAuthor);
    }
}
