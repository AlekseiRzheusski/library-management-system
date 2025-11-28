using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;

using LibraryManagement.Application.Services.DTOs.BookModels;
using LibraryManagement.Application.Services.Interaces;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Infrastructure.Repositories.Interfaces;
using LibraryManagement.Shared.Exceptions;

namespace LibraryManagement.Application.Services;

public class BookService : IBookService
{
    private readonly ILogger<BookService> _logger;
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
        ISearchService<Book> bookSearchService,
        ILogger<BookService> logger)
    {
        _logger = logger;
        _bookRepository = bookRepository;
        _mapper = mapper;
        _createBookCommandValidator = createBookCommandValidator;
        _searchBookCommandValidator = searchBookCommandValidator;
        _updateBookCommandValidator = updateBookCommandValidator;
        _bookSearchService = bookSearchService;
    }

    public async Task<BookDto?> GetBookAsync(long bookId)
    {
        _logger.LogInformation("Fetching book with {0} id", bookId);
        var book = await _bookRepository.GetDetailedEntityByIdAsync(bookId);
        if (book == null)
        {
            throw new EntityNotFoundException($"Book with ID {bookId} does not exist");
        }
        return _mapper.Map<BookDto>(book);
    }

    public async Task<BookDto?> CreateBookAsync(CreateBookCommand command)
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

        var detailedBook = await _bookRepository.GetDetailedEntityByIdAsync(newBook.BookId);
        var newBookDto = _mapper.Map<BookDto>(detailedBook);

        _logger.LogInformation("Book with id {0} was successfuly created", newBookDto.BookId);

        return newBookDto;
    }

    public async Task DeleteBookAsync(long bookId)
    {
        var book = await _bookRepository.GetDetailedEntityByIdAsync(bookId);
        if (book is null)
        {
            throw new EntityNotFoundException($"Book with ID {bookId} does not exist");
        }

        _logger.LogInformation("Removing book with {0} ID", bookId);
        _bookRepository.Delete(book);
        await _bookRepository.SaveAsync();
    }

    public async Task<(int totalCount, int numberOfPages, IEnumerable<BookDto> searchResultPage)> GetBooksAsync(SearchBookCommand command, int pageSize, int pageNumber)
    {
        //TODO: add page size validation
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

        _logger.LogInformation("Fetching page {0}, with expression filter {1}", pageNumber, expression);
        var resultPage = await _bookRepository.FindDetaliedEntitiesPageAsync(expression, pageSize, pageNumber);

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

        _logger.LogInformation("Updating book with {0} ID", bookId);
        _mapper.Map(command, book);
        await _bookRepository.SaveAsync();

        var detailedBook = await _bookRepository.GetDetailedEntityByIdAsync(bookId);

        return _mapper.Map<BookDto>(detailedBook);
    }
}
