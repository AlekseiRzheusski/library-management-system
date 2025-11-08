using LibraryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagement.Infrastructure.Data.Configurations;

public class AuthorConfiguration : IEntityTypeConfiguration<Author>
{
    public void Configure(EntityTypeBuilder<Author> builder)
    {
        builder.HasKey(a => a.AuthorId);

        builder.Property(a => a.FirstName)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(a => a.LastName)
            .IsRequired()
            .HasMaxLength(50);
    }
}
