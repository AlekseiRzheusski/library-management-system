using MediatR;
using LibraryManagement.Infrastructure.Repositories.Interfaces;
using LibraryManagement.Shared.Exceptions;
using FluentValidation;

namespace LibraryManagement.Application.Authors.DeleteAuthor;

public class DeleteAuthorHandler : IRequestHandler<DeleteAuthor>
{
    private readonly IAuthorRepository _authorRepository;
    public DeleteAuthorHandler(
        IAuthorRepository authorRepository
    )
    {
        _authorRepository = authorRepository;
    }

    public async Task Handle(DeleteAuthor request, CancellationToken token)
    {
        var author = await _authorRepository.GetDetailedEntityByIdAsync(request.authorId);
        if (author is null)
        {
            throw new EntityNotFoundException($"Author with ID {request.authorId} does not exist");
        }

        if (author.Books.Count() > 0)
        {
            throw new ValidationException($"This author has related books");
        }

        _authorRepository.Delete(author);
        await _authorRepository.SaveAsync();
    }
}
