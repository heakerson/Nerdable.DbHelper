
using AutoMapper;
using Nerdable.DbHelper.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Nerdable.DbHelper.Extensions
{
    public static class DbHelperExtensions
    {
        public static IServiceCollection AddDbHelper<TDbContext>(this IServiceCollection services) where TDbContext : DbContext
        {
            services.AddScoped<IDbHelper, Services.DbHelper>();
            services.AddScoped<DbContext, TDbContext>();
            services.AddAutoMapper();

            return services;
        }

    }
}
