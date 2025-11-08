namespace LibraryManagement.Domain.Entities;

public class Category
{
    public long CategoryId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public long? ParentCategoryId { get; set; }
    public Category? ParentCategory { get; set; }
    public ICollection<Category> SubCategories { get; set; } = new List<Category>();

    public int SortOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;

    public ICollection<Book> Books { get; set; } = new List<Book>();
}
