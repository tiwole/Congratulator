using System.Reflection;
using Congratulator.Infrastructure.Exceptions;
using Congratulator.Infrastructure.Repositories;
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

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        Assembly assembly = typeof(DbContextFactory).Assembly;

        var repositoriesNamespace = typeof(PersonRepository).Namespace;

        var repositoryTypes = assembly.GetTypes()
            .Where(type => type.Namespace == repositoriesNamespace &&
                           type.IsClass && !type.IsAbstract);

        // Add exceptions here if needed.
        services.AddAutoMapper(assembly);

        foreach (var implType in repositoryTypes)
        {
            var interfaceType = implType.GetInterfaces()
                .FirstOrDefault(i => i.Name == $"I{implType.Name}");

            if (interfaceType != null)
            {
                services.AddScoped(interfaceType, implType);
            }
        }

        return services;
    }
}