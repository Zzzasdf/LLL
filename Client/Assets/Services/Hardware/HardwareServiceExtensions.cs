using Microsoft.Extensions.DependencyInjection;

public static class HardwareServiceExtensions
{
    public static void AddHardwareService(this IServiceCollection services)
    {
        services.AddSingleton<IHardwareService, HardwareService>();
    }
}
