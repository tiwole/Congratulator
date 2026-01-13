using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Congratulator.Infrastructure.Middleware;

public static class MigrateDbOnStartupConfiguration
{
    public static void EnsureMigration<T>(this IApplicationBuilder builder, IConfiguration configuration)
        where T : DbContext
    {
        var shouldMigrate = configuration.GetValue<bool>("MigrateDbOnStartup");
        if (!shouldMigrate)
        {
            return;
        }

        using var scope = builder.ApplicationServices.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<T>();

        try
        {
            var migrations = context.Database.GetPendingMigrations();
            if (migrations == null || !migrations.Any())
            {
                context.Database.EnsureCreated();
            }
            else
            {
                context.Database.Migrate();
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Error during migration: {exception.Message}");
        }
    }
}