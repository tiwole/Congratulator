namespace Congratulator.Api.Configurations;

/// <summary>
/// Static class for CORS Configuration
/// </summary>
public static class CorsConfiguration
{
    /// <summary>
    /// Adding CORS configuration to the project's API
    /// </summary>
    /// <param name="services">Collection of services from the API</param>
    /// <returns></returns>
    public static void AddCorsConfiguration(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowBlazor", policy =>
            {
                policy.WithOrigins(
                        "https://localhost:8081",
                        "https://api:80",
                        "http://localhost:8081",
                        "http://api:80",
                        "https://localhost:7272", //temp
                        "http://localhost:7272" //temp
                    )
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });
    }
}
