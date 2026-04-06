using System.Text.Json;
using System.Text.Json.Serialization;
using Blazor.Sonner.Extensions;
using Congratulator.WebAssembly.Services;
using LumexUI.Extensions;

namespace Congratulator.WebAssembly.Configuration;

public static class ClientConfiguration
{
    public static void AddServices(this IServiceCollection services)
    {
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        jsonOptions.Converters.Add(new JsonStringEnumConverter());
        services.AddSingleton(jsonOptions);
        
        services.AddSingleton<DateTimeProvider>();
        
        services.AddSonner();
        services.AddLumexServices();
    }
}