using System.Reflection;
using Application.Receiver.Services.Receiver;
using Domain.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Receiver.DI;

public static class ApplicationServices
{
    public static IServiceCollection AddApplicationService(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMediatR(o => o.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
        services.AddScoped<IReceiverService, ReceiverService>();
        services.Configure<SignalRClientSettings>(configuration.GetSection("SignalRClientSettings"));
        return services;
    }
}