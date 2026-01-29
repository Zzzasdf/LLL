using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

public class ViewConfigure: IViewConfigure
{
    private IServiceCollection services;
    
    private Type viewType;
    private IViewCheck viewCheck;

    private SubViewDisplay subViewDisplay;
    private Dictionary<SubViewType, IViewCheck> subViewTypes;
    
    private ViewLayer viewLayer;

    public ViewConfigure(IServiceCollection services)
    {
        this.services = services;
    }

    void IViewConfigure.AddView<TView, TViewModel>()
    {
        viewType = typeof(TView);
        services.AddTransient<TViewModel>();
    }

    public ViewConfigure AddAccountModel<TModel>()
        where TModel: class, IAccountLevelModel, new()
    {
        services.AddAccountLevelModel<TModel>();
        return this;
    }
    public ViewConfigure AddModel<TModel>()
        where TModel: class, new()
    {
        services.AddRoleLevelModel<TModel>();
        return this;
    }

    public ViewConfigure AddCheck(IViewCheck viewCheck)
    {
        this.viewCheck = viewCheck;
        return this;
    }

    public ViewConfigure AddSubType(SubViewDisplay subViewDisplay, Dictionary<SubViewType, IViewCheck> subViewTypes)
    {
        this.subViewDisplay = subViewDisplay;
        this.subViewTypes = subViewTypes;
        return this;
    }
    
    void IViewConfigure.AddLayer(ViewLayer viewLayer) => this.viewLayer = viewLayer;
    Dictionary<SubViewType, IViewCheck> IViewConfigure.GetSubViewTypes() => subViewTypes;

    Type IViewConfigure.GetViewType() => viewType;
    IViewCheck IViewConfigure.GetViewCheck() => viewCheck;
    ViewLayer IViewConfigure.GetViewLayer() => viewLayer;
}
