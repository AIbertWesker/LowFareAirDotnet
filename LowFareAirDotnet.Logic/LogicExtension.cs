using LowFareAirDotnet.Logic.Abstractions.Logic;
using LowFareAirDotnet.Logic.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LowFareAirDotnet.Logic;

public static class LogicExtension
{
    public static IServiceCollection RegisterLogic(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
