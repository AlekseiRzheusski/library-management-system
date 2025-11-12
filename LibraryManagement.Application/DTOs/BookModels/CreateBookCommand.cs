namespace LibraryManagement.Application.Services.DTOs.BookModels;

public class CreateBookCommand
{
    public string Title { get; set; } = null!;
    public string Isbn { get; set; } = null!;
    public string? Description { get; set; }
    public long AuthorId { get; set; }
    public long CategoryId { get; set; }
    public string? PublishedDate { get; set; }
    public int PageCount { get; set; }
}
