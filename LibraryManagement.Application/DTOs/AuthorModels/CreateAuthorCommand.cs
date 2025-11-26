namespace LibraryManagement.Application.Services.DTOs.AuthorModels;

public class CreateAuthorCommand
{
    public string FirstName {get; set;} = null!;
    public string LastName {get; set;} = null!;
    public string? Biography {get; set;}
    public string? DateOfBirth {get; set;}
}
