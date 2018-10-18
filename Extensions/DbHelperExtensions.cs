
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

        //public static IServiceCollection AddDbHelper<TDbContext,TProfile>(this IServiceCollection services, System.IServiceProvider provider) where TDbContext : DbContext where TProfile : IContextInjectable<TDbContext>
        //{
        //    services.AddScoped<IDbHelper, Services.DbHelper>();
        //    services.AddScoped<DbContext, TDbContext>();
        //    services.AddAutoMapper();

        //    var profile = provider.GetService<TProfile>();
        //    profile.SetContext(provider.GetService<TDbContext>());

        //    //services.AddScoped(provider => new MapperConfiguration(cfg =>
        //    //{
        //    //    cfg.AddProfile(new TProfile(provider.GetService<TDbContext>()));
        //    //}).CreateMapper());

        //    return services;
        //}
    }
}
