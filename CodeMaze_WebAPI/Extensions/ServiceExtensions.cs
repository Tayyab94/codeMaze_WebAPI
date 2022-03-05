
using Contracts;
using Entities;
using Entities.Helpes;
using Microsoft.EntityFrameworkCore;
using Repository;

namespace CodeMaze_WebAPI.Extensions
{
    public  static class ServiceExtensions
    {
        public static void ConfigureSqlServerContext(this IServiceCollection service, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            service.AddDbContext<RepositoryContext>(options => options.UseSqlServer(connectionString));
        }

        public static void ConfigureRepositoryWrapper(this IServiceCollection service)
        {
            service.AddScoped<ISortHelper<Owner>, SortHelper<Owner>>();
         
            service.AddScoped<IRepositoryWrapper, RepositoryWrapper>();

            service.AddScoped<IDataSharper<Owner>, DataSharper<Owner>>();
        }

    }
}
