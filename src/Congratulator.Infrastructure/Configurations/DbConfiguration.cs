using Congratulator.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Congratulator.Infrastructure.Configurations;

public static class DbConfiguration
{
    public static IServiceCollection AddDbConfiguration<TContext>(this IServiceCollection services,
        IConfiguration configuration, string connectionString) where TContext : DbContext
    {
        if (string.IsNullOrEmpty(connectionString) || !connectionString.Contains("Host="))
        {
            throw new InvalidConnectionStringException("Connection string is null or invalid.");
        }

        var migrationsAssembly = typeof(TContext).Assembly.FullName;

        //PostgreSQL
        services.AddDbContext<TContext>(options =>
        {
            options.UseNpgsql(connectionString, x => x.MigrationsAssembly(migrationsAssembly));
        });

        return services;
    }
}