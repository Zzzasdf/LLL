using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using UnityEngine;

public abstract class ViewBase<TViewModel> : MonoBehaviour, IView
    where TViewModel: class, IViewModel
{
    private bool unityEvent;
    
    protected TViewModel viewModel { get; private set; }
    private ViewLayer viewLayer;
    private int uniqueId;
    
    void IView.Init(ViewLayer viewLayer, int uniqueId)
    {
        viewModel = Ioc.Default.GetRequiredService<TViewModel>();
        IsActive(true);
        this.viewLayer = viewLayer;
        this.uniqueId = uniqueId;
    }

    ViewLayer IView.GetLayer() => viewLayer;
    int IView.GetUniqueId() => uniqueId;
    GameObject IView.GameObject() => gameObject;

    private void Awake()
    {
        unityEvent = true;
        IsActive(true);
    }
    private void OnDestroy()
    {
        IsActive(false);
    }
    private void IsActive(bool value)
    {
        if (!unityEvent || viewModel == null) return;
        if (viewModel is ObservableRecipient observableRecipient)
        {
            observableRecipient.IsActive = value;
            LLogger.Log($"{typeof(TViewModel).Name} IsActive: {value}");
        }
        if(value) BindUI();
        else UnBindUI();
    }

    protected virtual void BindUI()
    {
        
    }
    protected virtual void UnBindUI()
    {
        
    }
}
