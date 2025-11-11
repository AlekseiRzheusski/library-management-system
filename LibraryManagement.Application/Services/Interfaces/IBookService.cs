using LibraryManagement.Application.Services.DTOs.BookModels;

namespace LibraryManagement.Application.Services.Interaces;

public interface IBookService
{
    Task<BookDto?> GetBookAsync(long bookId);
}
