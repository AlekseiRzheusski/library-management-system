namespace LibraryManagement.Application.Services.DTOs.BorrowingModels;

public class UserBorrowingsCommand
{
    public long UserId { get; set; }
    public string? Status { get; set; }
}
