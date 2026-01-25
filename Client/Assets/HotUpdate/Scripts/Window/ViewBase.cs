using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using UnityEngine;

public abstract class ViewBase<TViewModel> : MonoBehaviour, IView
    where TViewModel: class, IViewModel
{
    private bool unityEvent;
    private bool viewEvent;
    private bool isActive;

    private ViewLayer viewLayer;
    
    private int uniqueId;
    protected TViewModel viewModel { get; private set; }

    void IView.BindLayer(ViewLayer viewLayer)
    {
        this.viewLayer = viewLayer;
    }

    ViewLayer IView.GetLayer() => viewLayer;

    void IView.BindUniqueId(int uniqueId) => this.uniqueId = uniqueId;

    int IView.GetUniqueId() => uniqueId;

    GameObject IView.GameObject() => gameObject;
    
    private void Awake()
    {
        unityEvent = true;
        Show_Internal();
    }
    void IView.Show()
    {
        viewEvent = true;
        Show_Internal();
    }
    private void Show_Internal()
    {
        if (!unityEvent || !viewEvent) return;
        if (isActive) return;
        isActive = true;
        
        viewModel = ViewModelGenerator.Default.GetOrAdd<TViewModel>(uniqueId);
        if (viewModel is ObservableRecipient observableRecipient)
        {
            observableRecipient.IsActive = isActive;
            LLogger.Log($"{typeof(TViewModel).Name} IsActive: {isActive}");
        }
        BindUI();
        
        gameObject.SetActive(true);
    }

    private void OnDestroy() => Hide_Internal();
    void IView.Hide() => Hide_Internal();
    private void Hide_Internal()
    {
        if (!unityEvent || !viewEvent || viewModel == null) return;
        if (!isActive) return;
        isActive = false;
        
        UnBindUI();
        if (viewModel is ObservableRecipient observableRecipient)
        {
            observableRecipient.IsActive = isActive;
            LLogger.Log($"{typeof(TViewModel).Name} IsActive: {isActive}");
        }
        viewModel = null;
        
        gameObject.SetActive(false);
    }

    protected abstract void BindUI();
    protected abstract void UnBindUI();
}
