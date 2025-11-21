using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using LibraryManagement.Application.Services.DTOs.BorrowingModels;
using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Application.Services.Interaces;

public interface IBorrowingService
{
    public Task<BorrowingDto> BorrowBookAsync(BorrowBookCommand command);
    public Task<BorrowingDto> ReturnBookAsync(long borrowingId);
    public Task<(int totalCount, int maxPageNumber, IEnumerable<BorrowingDto>)> GetUserBorrowingsAsync(
        UserBorrowingsCommand command,
        int pageNumber, 
        int pageSize);
    public Task<(int totalCount, int maxPageNumber, IEnumerable<BorrowingDto>)> GetOverdueBooksAsync(
        int pageNumber, 
        int pageSize);
    public Task CheckExpiredBorrowingsAsync();
}
