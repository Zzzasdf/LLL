using CommunityToolkit.Mvvm.DependencyInjection;
using UnityEngine;

public abstract class ViewBase<TViewModel> : MonoBehaviour, IView
    where TViewModel: class, IViewModel
{
    protected TViewModel viewModel { get; private set; }
    private ViewLayer viewLayer;
    private int uniqueId;
    
    void IView.Init(ViewLayer viewLayer, int uniqueId)
    {
        viewModel = Ioc.Default.GetRequiredService<TViewModel>();
        this.viewLayer = viewLayer;
        this.uniqueId = uniqueId;
    }

    ViewLayer IView.GetLayer() => viewLayer;
    int IView.GetUniqueId() => uniqueId;
    GameObject IView.GameObject() => gameObject;
}
