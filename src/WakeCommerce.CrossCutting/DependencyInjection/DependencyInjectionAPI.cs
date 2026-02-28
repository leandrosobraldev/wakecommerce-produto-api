using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WakeCommerce.Application.Interfaces;
using WakeCommerce.Application.Mappings;
using WakeCommerce.Application.Services;
using WakeCommerce.Domain.Interfaces;
using WakeCommerce.Infrastructure.Context;
using WakeCommerce.Infrastructure.EntitiesConfiguration;
using WakeCommerce.Infrastructure.Repositories;

namespace WakeCommerce.IoC.DependencyInjection
{
    public static class DependencyInjectionAPI
    {
        public static IServiceCollection AddInfrastructureAPI(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")
    ));

            services.AddScoped<IProdutoRepository, ProdutoRepository>();
            services.AddScoped<IProdutoService, ProdutoService>();
            

            services.AddAutoMapper(typeof(DomainDTOMapping));

            return services;
        }

        public static async Task EnsureDatabaseSeededAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await context.Database.MigrateAsync();
            await DbSeeder.SeedAsync(context);
        }
    }
}
