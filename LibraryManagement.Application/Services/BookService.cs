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
    private readonly IValidator<SearchBookCommand> _searchBookCommandValidator;
    private readonly ISearchService<Book> _bookSearchService;

    public BookService(
        IBookRepository bookRepository,
        IMapper mapper,
        IValidator<CreateBookCommand> createBookCommandValidator,
        IValidator<SearchBookCommand> searchBookCommandValidator,
        ISearchService<Book> bookSearchService)
    {
        _bookRepository = bookRepository;
        _mapper = mapper;
        _createBookCommandValidator = createBookCommandValidator;
        _searchBookCommandValidator = searchBookCommandValidator;
        _bookSearchService = bookSearchService;
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
            throw new EntityNotFoundException($"Book with ID {bookId} does not exist");
        }

        _bookRepository.Delete(book);
        await _bookRepository.SaveAsync();
    }

    public async Task<IEnumerable<BookDto>> GetBooksAsync(SearchBookCommand command, int pageSize, int pageNumber)
    {
        var validation = await _searchBookCommandValidator.ValidateAsync(command);
        if (!validation.IsValid)
        {
            var message = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage));
            throw new ValidationException(message);
        }

        var totalCount = await _bookRepository.CountAsync();
        var maxPageNumber = (int)Math.Ceiling((double)totalCount / pageSize);

        if (totalCount > 0 && (pageNumber < 0 || pageNumber > maxPageNumber))
            throw new IndexOutOfRangeException($"Page number must not exceed {maxPageNumber}");

        var expression = _bookSearchService.BuildExpression<SearchBookCommand>(command);
        var result = await _bookRepository.FindBooksAsync(expression, pageSize, pageNumber);

        if (!result.Any())
        {
            throw new EntityNotFoundException("No results match your search criteria.");
        }

        var resultDto = _mapper.Map<IEnumerable<BookDto>>(result);
        return resultDto;
    }
}
