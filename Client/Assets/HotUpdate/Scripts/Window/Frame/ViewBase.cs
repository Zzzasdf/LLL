using CommunityToolkit.Mvvm.ComponentModel;
using UnityEngine;

public abstract class ViewBase<TViewModel> : MonoBehaviour, IView
    where TViewModel: class, IViewModel
{
    private IViewLocator viewLocator;
    protected TViewModel viewModel { get; private set; }
    protected IViewCheck viewCheck { get; private set; }

    void IView.BindLocator(IViewLocator viewLocator) => this.viewLocator = viewLocator;
    IViewLocator IView.GetLocator() => viewLocator;
    GameObject IView.GameObject() => gameObject;

    void IView.AddViewModel(int uniqueId)
    {
        viewModel = ViewModelGenerator.Default.GetOrAdd<TViewModel>(uniqueId);
        viewCheck = ViewCheckGenerator.Default.Get(uniqueId);
        if (viewModel is ObservableRecipient observableRecipient)
        {
            observableRecipient.IsActive = true;
            LLogger.FrameLog($"{typeof(TViewModel).Name} IsActive: {observableRecipient.IsActive}");
        }
    }

    void IView.RemoveViewModel()
    {
        if (viewModel is ObservableRecipient observableRecipient)
        {
            observableRecipient.IsActive = false;
            LLogger.FrameLog($"{typeof(TViewModel).Name} IsActive: {observableRecipient.IsActive}");
        }
        viewModel = null;
        viewCheck = null;
    }

    void IView.InitUI()
    {
        InitUI(viewCheck?.GetViewCheckValue());
    }
    public abstract void InitUI(object viewCheckValue);
    public abstract void DestroyUI();
    public abstract void BindUI();
    public abstract void UnBindUI();
}
