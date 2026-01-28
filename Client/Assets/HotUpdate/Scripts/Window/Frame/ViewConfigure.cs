using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

public class ViewConfigure: IViewConfigure
{
    private IServiceCollection services;
    
    private Type viewType;
    private IViewCheck viewCheck;
    private List<Type> subViewTypes;
    private List<SubViewAKA> subViewAKAs;
    
    private ViewLayer viewLayer;

    public ViewConfigure(IServiceCollection services)
    {
        this.services = services;
    }

    public ViewConfigure AddView<TView, TViewModel>()
        where TView : IView
        where TViewModel : class, IViewModel
    {
        viewType = typeof(TView);
        services.AddTransient<TViewModel>();
        return this;
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

    public ViewConfigure AddSubType<TSubView>()
        where TSubView: IView
    {
        subViewTypes ??= new List<Type>();
        subViewTypes.Add(typeof(TSubView));
        return this;
    }
    public ViewConfigure AddSubAKAs(List<SubViewAKA> subViewAKAs)
    {
        this.subViewAKAs = subViewAKAs;
        return this;
    }
    
    void IViewConfigure.Build(ViewLayer viewLayer) => this.viewLayer = viewLayer;
    List<Type> IViewConfigure.GetSubViewTypes() => subViewTypes;
    List<SubViewAKA> IViewConfigure.GetSubViewAKAs() => subViewAKAs;

    Type IViewConfigure.GetViewType() => viewType;
    bool IViewConfigure.IsFuncOpen() => viewCheck?.IsFuncOpen() ?? true;
    bool IViewConfigure.IsFuncOpenWithTip() => viewCheck?.IsFuncOpenWithTip() ?? true;

    ViewLayer IViewConfigure.GetViewLayer() => viewLayer;
}
