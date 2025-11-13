using AutoMapper;
using FluentValidation;
using LibraryManagement.Application.Services.DTOs.BookModels;
using LibraryManagement.Application.Services.Interaces;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Infrastructure.Repositories.Interfaces;
using LibraryManagement.Shared.Exceptions;

namespace LibraryManagement.Application.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateBookCommand> _createBookCommandValidator;

    public BookService(
        IBookRepository bookRepository,
        IMapper mapper,
        IValidator<CreateBookCommand> createBookCommandValidator)
    {
        _bookRepository = bookRepository;
        _mapper = mapper;
        _createBookCommandValidator = createBookCommandValidator;
    }

    public async Task<BookDto?> GetBookAsync(long bookId)
    {
        var book = await _bookRepository.GetDetailedBookInfo(bookId);
        return book == null ? null : _mapper.Map<BookDto>(book);
    }

    public async Task<BookDto?>CreateBookAsync(CreateBookCommand command)
    {
        var validation = await _createBookCommandValidator.ValidateAsync(command);
        if (!validation.IsValid)
        {
            var message = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage));
            throw new ValidationException(message);
        }

        var newBook = _mapper.Map<Book>(command);
        await _bookRepository.AddAsync(newBook);
        await _bookRepository.SaveAsync();

        var detailedBook = await _bookRepository.GetDetailedBookInfo(newBook.BookId);
        var newBookDto = _mapper.Map<BookDto>(detailedBook);

        return newBookDto;
    }

    public async Task DeleteBookAsync(long bookId)
    {
        var book = await _bookRepository.GetDetailedBookInfo(bookId);
        if (book is null)
        {
            throw new IdNotFoundInDatabaseException($"BookId: {bookId}, doesn't exist");
        }

        _bookRepository.Delete(book);
        await _bookRepository.SaveAsync();
    }
}
