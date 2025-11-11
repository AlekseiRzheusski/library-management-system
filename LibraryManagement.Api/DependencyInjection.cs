using SimpleInjector;
using AutoMapper;

using LibraryManagement.Application.Mappings;
using LibraryManagement.Api.Mappings;

namespace LibraryManagement.Api;

public static class DependencyInjection
{
    public static void AddAutoMapper(this Container container)
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
            cfg.AddProfile<GrpcBookMappingProfile>();
        }, loggerFactory);

        IMapper mapper = config.CreateMapper();
        container.RegisterInstance(mapper);
    }
}
