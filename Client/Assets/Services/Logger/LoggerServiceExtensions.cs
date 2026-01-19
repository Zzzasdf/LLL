using Microsoft.Extensions.DependencyInjection;

public static class LoggerServiceExtensions
{
    public static void AddLoggerService(this IServiceCollection services)
    {
        services.AddSingleton<IMyLogger, MyLogger>();
    }
}
