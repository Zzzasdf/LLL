using Microsoft.Extensions.DependencyInjection;

public static class LogServiceExtensions
{
    public static void AddLogService(this IServiceCollection services)
    {
        services.AddSingleton<ILogService, LogService>();
    }
}
