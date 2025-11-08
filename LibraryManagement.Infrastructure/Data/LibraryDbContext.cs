using Microsoft.EntityFrameworkCore;

using LibraryManagement.Domain.Entities;
using LibraryManagement.Infrastructure.Data.Configurations;

namespace LibraryManagement.Infrastructure.Data;

public class LibraryDbContext : DbContext
{
    public DbSet<Author> Authors { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<Borrowing> Borrowings { get; set; }
    public DbSet<Category> Categories { get; set; }

    public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
            : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AuthorConfiguration());
        modelBuilder.ApplyConfiguration(new BookConfiguration());
        modelBuilder.ApplyConfiguration(new BorrowingConfiguration());
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());
    }
}
