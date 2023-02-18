using System;

using Microsoft.Extensions.DependencyInjection;

namespace binstarjs03.AerialOBJ.WpfApp;

public static class ServiceConfiguration
{
    public static IServiceProvider Configure()
    {
        IServiceCollection services = new ServiceCollection();
        return services.BuildServiceProvider();
    }
}
