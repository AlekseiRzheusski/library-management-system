using System.Globalization;
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
    private readonly IValidator<UpdateBookCommand> _updateBookCommandValidator;
    private readonly ISearchService<Book> _bookSearchService;

    public BookService(
        IBookRepository bookRepository,
        IMapper mapper,
        IValidator<CreateBookCommand> createBookCommandValidator,
        IValidator<SearchBookCommand> searchBookCommandValidator,
        IValidator<UpdateBookCommand> updateBookCommandValidator,
        ISearchService<Book> bookSearchService)
    {
        _bookRepository = bookRepository;
        _mapper = mapper;
        _createBookCommandValidator = createBookCommandValidator;
        _searchBookCommandValidator = searchBookCommandValidator;
        _updateBookCommandValidator = updateBookCommandValidator;
        _bookSearchService = bookSearchService;
    }

    public async Task<BookDto?> GetBookAsync(long bookId)
    {
        var book = await _bookRepository.GetDetailedBookInfoAsync(bookId);
        if (book == null)
        {
            throw new EntityNotFoundException($"Book with ID {bookId} does not exist");
        }
        return _mapper.Map<BookDto>(book);
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

        var detailedBook = await _bookRepository.GetDetailedBookInfoAsync(newBook.BookId);
        var newBookDto = _mapper.Map<BookDto>(detailedBook);

        return newBookDto;
    }

    public async Task DeleteBookAsync(long bookId)
    {
        var book = await _bookRepository.GetDetailedBookInfoAsync(bookId);
        if (book is null)
        {
            throw new EntityNotFoundException($"Book with ID {bookId} does not exist");
        }

        _bookRepository.Delete(book);
        await _bookRepository.SaveAsync();
    }

    public async Task<(int totalCount, int numberOfPages, IEnumerable<BookDto> searchResultPage)> GetBooksAsync(SearchBookCommand command, int pageSize, int pageNumber)
    {
        var validation = await _searchBookCommandValidator.ValidateAsync(command);
        if (!validation.IsValid)
        {
            var message = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage));
            throw new ValidationException(message);
        }

        var expression = _bookSearchService.BuildExpression<SearchBookCommand>(command);

        int totalCount = await _bookRepository.GetQueryCountAsync(expression);
        int maxPageNumber = (int)Math.Ceiling((double)totalCount / pageSize);

        if (totalCount > 0 && (pageNumber < 0 || pageNumber > maxPageNumber))
            throw new IndexOutOfRangeException($"Page number must not exceed {maxPageNumber}");

        var resultPage = await _bookRepository.FindBooksAsync(expression, pageSize, pageNumber);

        if (!resultPage.Any())
        {
            throw new EntityNotFoundException("No results match your search criteria.");
        }

        var resultDtoPage = _mapper.Map<IEnumerable<BookDto>>(resultPage);
        return (totalCount, maxPageNumber, resultDtoPage);
    }

    public async Task<BookDto> UpdateBookAsync(UpdateBookCommand command, long bookId)
    {
        var validation = await _updateBookCommandValidator.ValidateAsync(command);
        if (!validation.IsValid)
        {
            var message = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage));
            throw new ValidationException(message);
        }

        var book = await _bookRepository.GetByIdAsync(bookId);
        if (book is null)
        {
            throw new EntityNotFoundException("No results match your search criteria.");
        }

        _mapper.Map(command, book);
        await _bookRepository.SaveAsync();

        var detailedBook = await _bookRepository.GetDetailedBookInfoAsync(bookId);

        return _mapper.Map<BookDto>(detailedBook);
    }
}
