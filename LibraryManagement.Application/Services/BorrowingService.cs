using AutoMapper;
using FluentValidation;
using LibraryManagement.Application.Services.DTOs.BorrowingModels;
using LibraryManagement.Application.Services.Interaces;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Infrastructure.Repositories.Interfaces;
using LibraryManagement.Shared.Exceptions;
using Microsoft.Extensions.Logging;
using SimpleInjector.Lifestyles;

namespace LibraryManagement.Application.Services;

public class BorrowingService : IBorrowingService
{
    private readonly ILogger<BorrowingService> _logger;
    private readonly IMapper _mapper;
    private readonly IBorrowingRepository _borrowingRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IValidator<BorrowBookCommand> _borrowBookCommandValidator;
    private const int DefaultDaysToReturn = 14;
    public BorrowingService(
        ILogger<BorrowingService> logger,
        IMapper mapper,
        IBorrowingRepository borrowingRepository,
        IBookRepository bookRepository,
        IValidator<BorrowBookCommand> borrowBookCommandValidator)
    {
        _logger = logger;
        _mapper = mapper;
        _borrowingRepository = borrowingRepository;
        _bookRepository = bookRepository;
        _borrowBookCommandValidator = borrowBookCommandValidator;
    }

    public async Task<BorrowingDto> BorrowBookAsync(BorrowBookCommand command)
    {
        var validation = await _borrowBookCommandValidator.ValidateAsync(command);
        if (!validation.IsValid)
        {
            var message = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage));
            throw new ValidationException(message);
        }

        var borrowDate = DateTime.Today;
        var dueDate = borrowDate.AddDays(command.daysToReturn ?? DefaultDaysToReturn);

        var book = await _bookRepository.GetByIdAsync(command.BookId);

        var borrowing = new Borrowing
        {
            Book = book!,
            UserId = command.UserId,
            BorrowDate = borrowDate,
            DueDate = dueDate,
        };

        book!.IsAvailable = false;

        await _borrowingRepository.AddAsync(borrowing);
        await _bookRepository.SaveAsync();

        var detailedBorrowing = await _borrowingRepository.GetDetailedBorrowing(borrowing.BorrowingId);

        return _mapper.Map<BorrowingDto>(detailedBorrowing);
    }

    public async Task<BorrowingDto> ReturnBookAsync(long borrowingId)
    {
        var detailedBorrowing = await _borrowingRepository.GetDetailedBorrowing(borrowingId);
        if (detailedBorrowing is null)
        {
            throw new EntityNotFoundException($"Book with ID {borrowingId} does not exist");
        }
        if (detailedBorrowing.Status == BorrowingStatus.Returned)
        {
            throw new ValidationException("This borrowing is already has the returned status");
        }

        detailedBorrowing.Status = BorrowingStatus.Returned;
        detailedBorrowing.ReturnDate = DateTime.Today;
        detailedBorrowing.Book.IsAvailable = true;

        await _borrowingRepository.SaveAsync();

        return _mapper.Map<BorrowingDto>(detailedBorrowing);
    }

    public async Task CheckExpiredBorrowingsAsync()
    {
        _logger.LogInformation($"Checking expired borrowings at {DateTime.Now}");
        var currentDate = DateTime.Today;
        var activeBorrowings = await _borrowingRepository.FindAndAddToContextAsync(
            b => b.Status == BorrowingStatus.Active && b.DueDate<=currentDate);

        if (!activeBorrowings.Any())
        {
            _logger.LogInformation("No Expired active Borrowings");
            return;
        }

        foreach(var borrowing in activeBorrowings)
        {
            borrowing.Status = BorrowingStatus.Overdue;
        }

        await _borrowingRepository.SaveAsync();
        _logger.LogInformation("Borrowing status updated");
    }
}
