namespace LibraryManagement.Application.Services.DTOs.CategoryModels;

public class CategoryDto
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public long? ParentCategoryId { get; set; }
    public string? ParentCategoryName { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    public int BookCount { get; set; }
}
