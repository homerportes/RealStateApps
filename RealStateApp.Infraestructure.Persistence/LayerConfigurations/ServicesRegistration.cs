using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RealStateApp.Core.Domain.Interfaces;
using RealStateApp.Infraestructure.Persistence.Contexts;
using RealStateApp.Infraestructure.Persistence.Repositories;


namespace RealStateApp.Infraestructure.Persistence.LayerConfigurations
{
    public static class ServicesRegistration
    {
        public static void AddPersistenceLayer(this IServiceCollection services, IConfiguration config)
        {

            if (config.GetValue<bool>("UseInMemoryDatabase"))
            {
                services.AddDbContext<RealStateContext>(options =>
                    options.UseInMemoryDatabase("BankingDB"));
            }
            else
            {
                services.AddDbContext<RealStateContext>(options =>
                    options.UseSqlServer(
                        config.GetConnectionString("DefaultConnection"),
                        m => m.MigrationsAssembly(typeof(RealStateContext).Assembly.FullName)));
            }


            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IPropertyRepository, PropertyRepository>();
            services.AddScoped<IPropertyTypeRepository, PropertyTypeRepository>();
            services.AddScoped<IImprovementRepository, ImprovementRepository>();
            services.AddScoped<ISaleTypeRepository, SaleTypeRepository>();
            services.AddScoped<IMessageRepository,MessageRepository>();
            services.AddScoped<IFavoritePropertyRepository,FavoritePropertyRepoitory>();
            services.AddScoped<IOfferRepository,OfferRepository>();
            services.AddScoped<IPropertyImprovementRepository,PropertyImprovementRepository>();
            services.AddScoped<IPropertyPhotoRepository,PropertyPhotoRepository>();
            services.AddScoped<IMessageRepository,MessageRepository>();

        }
    }
}
