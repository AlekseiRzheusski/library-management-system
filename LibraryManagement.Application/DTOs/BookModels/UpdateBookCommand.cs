namespace LibraryManagement.Application.Services.DTOs.BookModels;

public class UpdateBookCommand
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public long? CategoryId { get; set; }
    public string? PublishedDate { get; set; }
    public int? PageCount { get; set; }
}
