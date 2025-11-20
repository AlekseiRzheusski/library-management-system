namespace LibraryManagement.Application.Services.DTOs.BorrowingModels;

public class BorrowBookCommand
{
    public long BookId { get; set; }
    public long UserId { get; set; }
    public int? daysToReturn { get; set; }
}
