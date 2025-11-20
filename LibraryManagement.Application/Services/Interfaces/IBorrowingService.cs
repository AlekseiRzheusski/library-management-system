using System.Security.Cryptography.X509Certificates;
using LibraryManagement.Application.Services.DTOs.BorrowingModels;

namespace LibraryManagement.Application.Services.Interaces;

public interface IBorrowingService
{
    public Task<BorrowingDto> BorrowBookAsync(BorrowBookCommand command);
    public Task<BorrowingDto> ReturnBookAsync(long borrowingId);
    public Task CheckExpiredBorrowingsAsync();
}
