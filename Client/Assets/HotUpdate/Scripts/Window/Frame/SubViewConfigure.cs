using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

public class SubViewConfigure: ISubViewConfigure
{
    private IServiceCollection services;
    
    private Type subViewType;
    private IViewCheck subViewCheck;
    private List<SubViewAKA> subViewTypes;
    
    public SubViewConfigure(IServiceCollection services)
    {
        this.services = services;
    }
    
    public SubViewConfigure AddView<TView, TViewModel>()
        where TView : IView
        where TViewModel : class, IViewModel
    {
        subViewType = typeof(TView);
        services.AddTransient<TViewModel>();
        return this;
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

    public SubViewConfigure AddCheck(IViewCheck viewCheck)
    {
        this.subViewCheck = viewCheck;
        return this;
    }

    public SubViewConfigure AddAKAs(List<SubViewAKA> subViewTypes)
    {
        this.subViewTypes = subViewTypes;
        return this;
    }

    Type ISubViewConfigure.GetSubViewType() => subViewType;
    List<SubViewAKA> ISubViewConfigure.GetSubViewAKAs() => subViewTypes;
    bool ISubViewConfigure.IsFuncOpen() => subViewCheck?.IsFuncOpen() ?? true;
    bool ISubViewConfigure.IsFuncOpenWithTip() => subViewCheck?.IsFuncOpenWithTip() ?? true;
}
