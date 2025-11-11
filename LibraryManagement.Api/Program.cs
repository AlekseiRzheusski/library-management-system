using SimpleInjector;
using SimpleInjector.Lifestyles;
using SimpleInjector.Integration.ServiceCollection;
using SimpleInjector.Integration.AspNetCore;
using Microsoft.EntityFrameworkCore;

using LibraryManagement.Api.Services;
using LibraryManagement.Infrastructure.Data;
using LibraryManagement.Infrastructure;
using LibraryManagement.Application;
using LibraryManagement.Api.Mappings;
using LibraryManagement.Api;
using AutoMapper;

var builder = WebApplication.CreateBuilder(args);

var container = new Container();
container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

builder.Services.AddSimpleInjector(container, options =>
{
    options.AddAspNetCore();
});

var options = new DbContextOptionsBuilder<LibraryDbContext>()
      .UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
      .Options;

container.AddInfrastructure(options);
container.AddApplication();
container.AddAutoMapper();

container.Register<GrpcBookService>(Lifestyle.Scoped);

builder.Services.AddScoped<GrpcBookService>(sp => container.GetInstance<GrpcBookService>());


// Add services to the container.
builder.Services.AddGrpc();

var app = builder.Build();

app.Services.UseSimpleInjector(container);
// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGrpcService<GrpcBookService>();

container.Verify();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
