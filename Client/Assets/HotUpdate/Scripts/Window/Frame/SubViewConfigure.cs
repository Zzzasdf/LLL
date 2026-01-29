using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

public class SubViewConfigure: ISubViewConfigure
{
    private IServiceCollection services;

    private Type type;
    private List<SubViewType> subViewTypes;
    
    public SubViewConfigure(IServiceCollection services)
    {
        this.services = services;
    }
    void ISubViewConfigure.AddView<TView, TViewModel>(List<SubViewType> subViewTypes)
    {
        type = typeof(TView);
        services.AddTransient<TViewModel>();
        this.subViewTypes = subViewTypes;
    }

    public SubViewConfigure AddAccountModel<TModel>()
        where TModel: class, IAccountLevelModel, new()
    {
        services.AddAccountLevelModel<TModel>();
        return this;
    }
    public SubViewConfigure AddModel<TModel>()
        where TModel: class, new()
    {
        services.AddRoleLevelModel<TModel>();
        return this;
    }

    Type ISubViewConfigure.GetSubViewType() => type;
    List<SubViewType> ISubViewConfigure.GetSubViewTypes() => subViewTypes;
}
