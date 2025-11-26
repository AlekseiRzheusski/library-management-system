namespace LibraryManagement.Application.Services.DTOs.AuthorModels;

public class UpdateAuthorCommand
{
    public string? FirstName {get; set;}
    public string? LastName {get; set;}
    public string? Biography {get; set;}
    public string? DateOfBirth {get; set;}
    public bool? IsActive {get; set;}
}
