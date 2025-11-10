#infrastructure dependencies
dotnet add LibraryManagement.Infrastructure reference LibraryManagement.Domain

dotnet add LibraryManagement.Infrastructure package Microsoft.EntityFrameworkCore
dotnet add LibraryManagement.Infrastructure package Microsoft.EntityFrameworkCore.Sqlite
dotnet add LibraryManagement.Infrastructure package Microsoft.EntityFrameworkCore.Design
dotnet add LibraryManagement.Infrastructure/ package Microsoft.Extensions.Configuration
dotnet add LibraryManagement.Infrastructure/ package Microsoft.Extensions.Configuration.FileExtensions
dotnet add LibraryManagement.Infrastructure/ package Microsoft.Extensions.Configuration.Json


#Test dependencies
dotnet add LibraryManagement.Integration.Tests package Testcontainers
dotnet add LibraryManagement.Integration.Tests package Testcontainers.PostgreSql
dotnet add LibraryManagement.Integration.Tests/ reference LibraryManagement.Infrastructure/
dotnet add LibraryManagement.Integration.Tests/ package Microsoft.EntityFrameworkCore
dotnet add LibraryManagement.Integration.Tests/ package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add LibraryManagement.Integration.Tests/ package Microsoft.EntityFrameworkCore.Sqlite
dotnet add LibraryManagement.Integration.Tests/ reference LibraryManagement.Domain
dotnet test


#Api dependencies
dotnet add LibraryManagement.Api/ reference LibraryManagement.Infrastructure/

dotnet add LibraryManagement.Api package SimpleInjector
dotnet add LibraryManagement.Api package SimpleInjector.Integration.ServiceCollection
dotnet add LibraryManagement.Api package SimpleInjector.Integration.AspNetCore
dotnet add LibraryManagement.Api package Microsoft.EntityFrameworkCore
dotnet add LibraryManagement.Api package Microsoft.EntityFrameworkCore.Sqlite
dotnet add LibraryManagement.Api package Microsoft.EntityFrameworkCore.Design

#migrations
dotnet ef migrations add InitialCreate --project LibraryManagement.Infrastructure --startup-project LibraryManagement.Api
#to show SQL comands --verbose
dotnet ef database update --project LibraryManagement.Infrastructure --startup-project LibraryManagement.Api --verbose
dotnet ef migrations add SeedData --project LibraryManagement.Infrastructure --startup-project LibraryManagement.Api
