using SimpleInjector;
using AutoMapper;
using Microsoft.Extensions.Logging;

using LibraryManagement.Application.Mappings;
using LibraryManagement.Application.Services;
using LibraryManagement.Application.Services.Interaces;

namespace LibraryManagement.Application;

public static class DependencyInjection
{
    public static void AddApplication(this Container container)
    {
        //Temporary logger, to create MapperConfiguration, unless
        //'MapperConfiguration' does not contain a constructor that takes 1 arguments
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<BookMappingProfile>();
        }, loggerFactory);

        IMapper mapper = config.CreateMapper();
        container.RegisterInstance(mapper);

        container.Register<IBookService, BookService>(Lifestyle.Scoped);
    }
}
