using LowFareAirDotnet.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LowFareAirDotnet.Infrastructure;

public static class InfrastructureExtension
{
    public static IServiceCollection RegisterInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("Default");

        //BTW uzywanie DbContext pooling dla lepszej wydajności. Reużywa instancji DbContext zmniejszając narzut
        services.AddDbContextPool<RelationalDbContext>(options =>
            options.UseNpgsql(connectionString));

        //services.AddDbContextFactory<RelationalDbContext>(options =>
        //    options.UseNpgsql(connectionString));

        return services;
    }
}
