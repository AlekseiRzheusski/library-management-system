using LibraryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagement.Infrastructure.Data.Configurations;

public class BorrowingConfiguration : IEntityTypeConfiguration<Borrowing>
{
    public void Configure(EntityTypeBuilder<Borrowing> builder)
    {
        builder.HasKey(b => b.BorrowingId);

        builder.HasOne(b => b.Book)
            .WithMany()
            .HasForeignKey(b => b.BookId);

        builder.HasIndex(b => b.UserId);
        builder.HasIndex(b => b.Status);
        builder.HasIndex(b => b.DueDate);
    }
}
