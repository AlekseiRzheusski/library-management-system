namespace LibraryManagement.Application.Services.DTOs.CategoryModels;

public class SearchCategoryCommand
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public long? ParentCategoryId { get; set; }
    public bool? IsActive { get; set; }
}
