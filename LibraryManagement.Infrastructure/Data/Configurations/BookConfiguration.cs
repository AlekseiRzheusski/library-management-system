using LibraryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagement.Infrastructure.Data.Configurations;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.HasKey(b => b.BookId);

        builder.HasIndex(b => b.ISBN)
            .IsUnique();
        builder.HasIndex(b => b.AuthorId);
        builder.HasIndex(b => b.CategoryId);
        builder.HasIndex(b => b.IsAvailable);

        builder.Property(b => b.Title)
            .IsRequired();
        builder.Property(b => b.ISBN)
            .HasMaxLength(13)
            .IsRequired()
            .HasColumnType("CHAR(13)");

        builder.HasOne(b => b.Author)
            .WithMany(a => a.Books)
            .HasForeignKey(b => b.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Category)
            .WithMany(c => c.Books)
            .HasForeignKey(b => b.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
