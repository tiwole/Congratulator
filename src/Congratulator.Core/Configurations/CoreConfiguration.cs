using System.Reflection;
using Congratulator.Core.Services;
using Congratulator.SharedKernel.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Congratulator.Core.Configurations;

public static class CoreConfiguration
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        Assembly coreAssembly = typeof(CreatePersonService).Assembly;
        Assembly sharedKernelAssembly = typeof(IUniqueIdentifier).Assembly;

        var coreServicesNamespace = typeof(CreatePersonService).Namespace;
        var sharedKernelInterfacesNamespace = typeof(IUniqueIdentifier).Namespace;

        var serviceTypes = coreAssembly.GetTypes()
            .Where(type => type.Namespace == coreServicesNamespace &&
                           type.IsClass && !type.IsAbstract);

        var interfaceTypes = sharedKernelAssembly.GetTypes()
            .Where(type => type.Namespace == sharedKernelInterfacesNamespace &&
                           type.IsInterface);

        foreach (var interfaceType in interfaceTypes)
        {
            var implementationType = coreAssembly.GetTypes()
                .FirstOrDefault(t => t.GetInterfaces().Contains(interfaceType));

            if (implementationType != null)
            {
                services.AddScoped(interfaceType, implementationType);
            }
        }
        foreach (var serviceType in serviceTypes)
        {
            services.AddScoped(serviceType);
        }

        return services;
    }
}