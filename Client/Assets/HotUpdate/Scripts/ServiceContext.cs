using System;

public class ServiceContext
{
    private static IServiceProvider provider;
    
    public ServiceContext(IServiceProvider serviceProvider)
    {
        provider = serviceProvider;
    }
    
    public static IServiceProvider GetProvider()
    {
        return provider;
    }
}
