using System.Reflection;
using Congratulator.Infrastructure.Extensions;
using Congratulator.Infrastructure.Repositories;
using Congratulator.Infrastructure.Services;
using Congratulator.SharedKernel.Interfaces;
using Congratulator.SharedKernel.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Congratulator.Infrastructure.Configurations;

public static class InfrastructureConfiguration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        Assembly assembly = typeof(DbContextFactory).Assembly;

        var repositoriesNamespace = typeof(PersonRepository).Namespace;

        var repositoryTypes = assembly.GetTypes()
            .Where(type => type.Namespace == repositoriesNamespace &&
                           type.IsClass && !type.IsAbstract);

        // Add exceptions here if needed.
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddAutoMapper(assembly);
        services.AddScoped<IStorageService, YandexS3Service>();

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