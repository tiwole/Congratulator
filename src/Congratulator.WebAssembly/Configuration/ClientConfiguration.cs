using System.Text.Json;
using System.Text.Json.Serialization;
using Congratulator.WebAssembly.Services;

namespace Congratulator.WebAssembly.Configuration;

public static class ClientConfiguration
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        jsonOptions.Converters.Add(new JsonStringEnumConverter());
        services.AddSingleton(jsonOptions);
        
        services.AddSingleton<DateTimeProvider>();
        services.AddSingleton<NotificationService>();

        return services;
    }
}