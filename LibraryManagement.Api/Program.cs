using SimpleInjector;
using SimpleInjector.Lifestyles;
using Microsoft.EntityFrameworkCore;

using LibraryManagement.Api.Services;
using LibraryManagement.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

var container = new Container();
container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

container.Register<LibraryDbContext>(() =>
{
    var optionsBuilder = new DbContextOptionsBuilder<LibraryDbContext>();
    optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    return new LibraryDbContext(optionsBuilder.Options);
}, Lifestyle.Scoped);

// Add services to the container.
builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
app.Services.UseSimpleInjector(container);

app.Run();
