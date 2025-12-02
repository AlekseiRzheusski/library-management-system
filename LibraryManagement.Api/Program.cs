using SimpleInjector;
using SimpleInjector.Lifestyles;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Hangfire;
using Hangfire.SQLite;

using LibraryManagement.Infrastructure;
using LibraryManagement.Infrastructure.Data;
using LibraryManagement.Application;
using LibraryManagement.Application.Services.Interaces;
using LibraryManagement.Api;
using LibraryManagement.Api.Services;
using LibraryManagement.Api.Hangfire;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

var container = new Container();
container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

builder.Services.AddSimpleInjector(container, options =>
{
  options.AddAspNetCore();
});

var loggerFactory = LoggerFactory.Create(builder =>
{
  builder.AddSerilog();
});

var provider = builder.Configuration.GetValue<string>("DatabaseProvider", "PostgreSql");
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

DbContextOptions<LibraryDbContext> options = provider == "Sqlite"
    ? new DbContextOptionsBuilder<LibraryDbContext>()
        .UseSqlite(connectionString, sql =>
            sql.MigrationsAssembly("LibraryManagement.Migrations.Sqlite"))
        // .EnableSensitiveDataLogging()
        // .UseLoggerFactory(loggerFactory)
        .Options
    : new DbContextOptionsBuilder<LibraryDbContext>()
        .UseNpgsql(connectionString, pg =>
            pg.MigrationsAssembly("LibraryManagement.Migrations.PostgreSql"))
        // .EnableSensitiveDataLogging()
        // .UseLoggerFactory(loggerFactory)
        .Options;

container.AddInfrastructure(options);
container.AddApplication();
container.AddAutoMapper();

container.Register<GrpcBookService>(Lifestyle.Scoped);
container.Register<GrpcBorrowingService>(Lifestyle.Scoped);
container.Register<GrpcCategoryService>(Lifestyle.Scoped);
container.Register<GrpcAuthorService>(Lifestyle.Scoped);
container.Register(typeof(ILogger<>), typeof(Logger<>), Lifestyle.Singleton);

builder.Services.AddScoped<GrpcBookService>(sp => container.GetInstance<GrpcBookService>());
builder.Services.AddScoped<GrpcBorrowingService>(sp => container.GetInstance<GrpcBorrowingService>());
builder.Services.AddScoped<GrpcCategoryService>(sp => container.GetInstance<GrpcCategoryService>());
builder.Services.AddScoped<GrpcAuthorService>(sp => container.GetInstance<GrpcAuthorService>());

// Add services to the container.
builder.Services.AddGrpc(options =>
{
  options.Interceptors.Add<LoggingInterceptor>();
});

builder.Services.AddHangfire(config =>
{
  config.
    UseSQLiteStorage(builder.Configuration.GetConnectionString("HangfireConnection"))
    .UseActivator(new SimpleInjectorJobActivator(container))
    .UseFilter(new SimpleInjectorAsyncScopeFilterAttribute(container));
});

builder.Services.AddHangfireServer(options =>
{
  options.WorkerCount = 1;
});

var app = builder.Build();

app.Services.UseSimpleInjector(container);
app.UseHangfireDashboard("/hangfire");

RecurringJob.AddOrUpdate<IBorrowingService>(
  "check-expired-borrowings",
  service => service.CheckExpiredBorrowingsAsync(),
  "0 22 * * *"
);

// Configure the HTTP request pipeline.
app.MapGrpcService<GrpcBookService>();
app.MapGrpcService<GrpcBorrowingService>();
app.MapGrpcService<GrpcCategoryService>();
app.MapGrpcService<GrpcAuthorService>();

container.Verify();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
