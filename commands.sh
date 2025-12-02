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

#Fluent Validation
dotnet add LibraryManagement.Application/ package FluentValidation

#To run tests
dotnet test
dotnet test --logger "console;verbosity=detailed"
#run one class
dotnet test --filter "FullyQualifiedName~LibraryManagement.Integration.Tests.Application.BookServiceTests"

#Serilog
dotnet add LibraryManagement.Api/ package Serilog.AspNetCore
dotnet add LibraryManagement.Api/ package Serilog.Sinks.Console
dotnet add LibraryManagement.Api/ package Serilog.Enrichers.Environment
dotnet add LibraryManagement.Api/ package Serilog.Enrichers.Process
dotnet add LibraryManagement.Api/ package Serilog.Enrichers.Thread

#Hangfire
dotnet add LibraryManagement.Api/ package Hangfire
dotnet add LibraryManagement.Api/ package Hangfire.AspNetCore
dotnet add LibraryManagement.Api/ package Hangfire.SQLite

#Postgresql
dotnet add LibraryManagement.Api package Npgsql.EntityFrameworkCore.PostgreSQL --version 8.0.4

#migrations
dotnet ef migrations add InitialCreate --project LibraryManagement.Infrastructure --startup-project LibraryManagement.Api
#to show SQL comands --verbose
dotnet ef database update --project LibraryManagement.Infrastructure --startup-project LibraryManagement.Api --verbose
dotnet ef migrations add SeedData --project LibraryManagement.Infrastructure --startup-project LibraryManagement.Api

#sqlite migration
dotnet ef database update   --project LibraryManagement.Migrations.Sqlite   --startup-project LibraryManagement.Api/   --context LibraryDbContext   --verbose

#rollback
dotnet ef database update 0 --startup-project LibraryManagement.Api
#delete migrations
dotnet ef migrations remove --project LibraryManagement.Migrations.Sqlite --startup-project LibraryManagement.Api

#first migration should be created in the same startup project
dotnet ef migrations add InitialPostgreSql   --project LibraryManagement.Migrations.PostgreSql/   --startup-project LibraryManagement.Migrations.PostgreSql/   --context LibraryDbContext   --verbose
