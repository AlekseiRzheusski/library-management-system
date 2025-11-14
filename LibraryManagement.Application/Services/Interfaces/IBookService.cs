using LibraryManagement.Application.Services.DTOs.BookModels;

namespace LibraryManagement.Application.Services.Interaces;

public interface IBookService
{
    Task<BookDto?> GetBookAsync(long bookId);
    Task<BookDto?> CreateBookAsync(CreateBookCommand command);
    Task DeleteBookAsync(long bookId);
    Task<IEnumerable<BookDto>> GetBooksAsync(SearchBookCommand bookSearchDto);
}
