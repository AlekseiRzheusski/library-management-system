namespace LibraryManagement.Domain.Entities;

public class Book
{
    public long BookId { get; set; }
    public string Title { get; set; } = null!;
    public string ISBN { get; set; } = null!;
    public string? Description { get; set; }

    public long AuthorId { get; set; }
    public Author Author { get; set; } = null!;

    public long CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public DateTime? PublishedDate { get; set; }
    public int? PageCount { get; set; }
    public bool IsAvailable { get; set; } = true;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedDate { get; set; }
}
