using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Domain.Entities;

public class Borrowing
{
    public long BorrowingId { get; set; }
    public long BookId { get; set; }
    public Book Book { get; set; } = null!;

    public long UserId { get; set; }

    public DateTime BorrowDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }

    public BorrowingStatus Status { get; set; } = BorrowingStatus.Active;
}
