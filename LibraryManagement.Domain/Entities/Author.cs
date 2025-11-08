namespace LibraryManagement.Domain.Entities;

public class Author
{
    public long AuthorId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;

    public string? Biography { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<Book> Books { get; set; } = new List<Book>();
}
