using Microsoft.Extensions.DependencyInjection;

public static class DeviceServiceExtensions
{
    public static void AddDeviceService(this IServiceCollection services)
    {
        services.AddSingleton<IDeviceService, DeviceService>();
    }
}
