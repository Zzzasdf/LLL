using Microsoft.Extensions.DependencyInjection;

public static class DataServiceExtensions
{
    public static void AddDataService(this IServiceCollection services)
    {
        services.AddSingleton<IDataService, DataService>();
    }
}
