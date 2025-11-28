using AutoMapper;
using FluentValidation;
using LibraryManagement.Application.Services.DTOs.AuthorModels;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Infrastructure.Repositories.Interfaces;
using MediatR;

namespace LibraryManagement.Application.Authors.CreateAuthor;

public class CreateAuthorHandler : IRequestHandler<CreateAuthor, AuthorDto>
{
    private readonly IMapper _mapper;
    private readonly IAuthorRepository _authorRepository;
    private readonly IValidator<CreateAuthorCommand> _createAuthorCommandValidator;

    public CreateAuthorHandler(
        IMapper mapper,
        IAuthorRepository authorRepository,
        IValidator<CreateAuthorCommand> createAuthorCommandValidator)
    {
        _mapper = mapper;
        _authorRepository = authorRepository;
        _createAuthorCommandValidator = createAuthorCommandValidator;
    }

    public async Task<AuthorDto> Handle(CreateAuthor request, CancellationToken token)
    {
        var validation = await _createAuthorCommandValidator.ValidateAsync(request.Command);
        if (!validation.IsValid)
        {
            var message = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage));
            throw new ValidationException(message);
        }

        var newAuthor = _mapper.Map<Author>(request.Command);
        await _authorRepository.AddAsync(newAuthor);
        await _authorRepository.SaveAsync();

        var detailedAuthor = await _authorRepository.GetDetailedEntityByIdAsync(newAuthor.AuthorId);
        var newAuthorDto = _mapper.Map<AuthorDto>(detailedAuthor);

        return newAuthorDto;
    }
}
