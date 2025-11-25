namespace LibraryManagement.Application.Services.DTOs.CategoryModels;

public class CreateCategoryCommand
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public long? ParentCategoryId { get; set; }
}
