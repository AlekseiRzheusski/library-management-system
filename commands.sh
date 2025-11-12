#Add internal dependency
dotnet add LibraryManagement.Api/ reference LibraryManagement.Infrastructure/

#Required packages
#EntityFramework
dotnet add LibraryManagement.Infrastructure package Microsoft.EntityFrameworkCore
dotnet add LibraryManagement.Infrastructure package Microsoft.EntityFrameworkCore.Sqlite
dotnet add LibraryManagement.Infrastructure package Microsoft.EntityFrameworkCore.Design
dotnet add LibraryManagement.Integration.Tests/ package Npgsql.EntityFrameworkCore.PostgreSQL

#For json config parse
dotnet add LibraryManagement.Infrastructure/ package Microsoft.Extensions.Configuration
dotnet add LibraryManagement.Infrastructure/ package Microsoft.Extensions.Configuration.FileExtensions
dotnet add LibraryManagement.Infrastructure/ package Microsoft.Extensions.Configuration.Json

#SimpleInjector
dotnet add LibraryManagement.Api package SimpleInjector
dotnet add LibraryManagement.Api package SimpleInjector.Integration.ServiceCollection
dotnet add LibraryManagement.Api package SimpleInjector.Integration.AspNetCore

#Test containers
dotnet add LibraryManagement.Integration.Tests package Testcontainers
dotnet add LibraryManagement.Integration.Tests package Testcontainers.PostgreSql

#Logging
#to use AddConsole
dotnet add LibraryManagement.Application/ package Microsoft.Extensions.Logging.Console

#Automapper
dotnet add LibraryManagement.Application/ package AutoMapper

#To run tests
dotnet test
dotnet test --logger "console;verbosity=detailed"
#run one class
dotnet test --filter "FullyQualifiedName~LibraryManagement.Integration.Tests.Application.BookServiceTests"

#migrations
dotnet ef migrations add InitialCreate --project LibraryManagement.Infrastructure --startup-project LibraryManagement.Api
#to show SQL comands --verbose
dotnet ef database update --project LibraryManagement.Infrastructure --startup-project LibraryManagement.Api --verbose
dotnet ef migrations add SeedData --project LibraryManagement.Infrastructure --startup-project LibraryManagement.Api
