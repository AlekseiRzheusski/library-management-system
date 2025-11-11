namespace LibraryManagement.Application.Services.DTOs.BookModels;

public class BookDto
{
    public long BookId { get; set; }
    public string? Title { get; set; }
    public string? Isbn { get; set; }
    public string Description { get; set; } = null!;
    public long AuthorId { get; set; }
    public string? AuthorName { get; set; }
    public long CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string? PublishedDate { get; set; }
    public int PageCount { get; set; }
    public bool IsAvailable { get; set; }
}
