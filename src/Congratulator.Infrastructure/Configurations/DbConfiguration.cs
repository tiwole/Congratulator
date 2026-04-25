using Congratulator.Infrastructure.Exceptions;
using Congratulator.Infrastructure.Repositories;
using Congratulator.Infrastructure.Services;
using Congratulator.SharedKernel.Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Congratulator.Infrastructure.Configurations;

public static class DbConfiguration
{
    public static void AddDbConfiguration<TContext>(this IServiceCollection services, string connectionString) where TContext : DbContext
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
    }
}