using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WakeCommerce.Application.Interfaces;
using WakeCommerce.Application.Mappings;
using WakeCommerce.Application.Services;
using WakeCommerce.Domain.Interfaces;
using WakeCommerce.Infrastructure.Context;
using WakeCommerce.Infrastructure.Respositories;

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
    }
}
