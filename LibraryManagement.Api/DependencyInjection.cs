using SimpleInjector;
using AutoMapper;

using LibraryManagement.Application.Mappings;
using LibraryManagement.Api.Mappings;

namespace LibraryManagement.Api;

public static class DependencyInjection
{
    public static void AddAutoMapper(this Container container)
    {
        container.RegisterSingleton<IMapper>(() =>
        {
            var loggerFactory = container.GetInstance<ILoggerFactory>();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<BookMappingProfile>();
                cfg.AddProfile<GrpcBookMappingProfile>();
                cfg.AddProfile<BorrowingMappingProfile>();
                cfg.AddProfile<GrpcBorrowingMappingProfile>();
            }, loggerFactory);

            return config.CreateMapper();
        });
    }
}
