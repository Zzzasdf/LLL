using CommunityToolkit.Mvvm.ComponentModel;
using UnityEngine;

public abstract class ViewEntityBase : EntityBase<IView>, IView
{
    public abstract void BindLocator(IEntityLocator entityLocator);
    public abstract IEntityLocator GetLocator();
    public abstract GameObject GameObject();
    public abstract void AddViewModel(int uniqueId);
    public abstract void RemoveViewModel();
    public abstract void InitUI(IViewCheck viewCheck);
    public abstract void DestroyUI();
    public abstract void BindUI();
    public abstract void UnBindUI();
}

public abstract class ViewEntityBase<TViewModel>: ViewEntityBase
    where TViewModel: class, IViewModel
{
    private IEntityLocator entityLocator;
    protected TViewModel viewModel { get; private set; }

    public override void BindLocator(IEntityLocator entityLocator) => this.entityLocator = entityLocator;
    public override IEntityLocator GetLocator() => entityLocator;
    public override GameObject GameObject() => gameObject;

    public override void AddViewModel(int uniqueId)
    {
        UniqueId = uniqueId;
        viewModel = ViewModelGenerator.Default.GetOrAdd<TViewModel>(uniqueId);
        if (viewModel is ObservableRecipient observableRecipient)
        {
            observableRecipient.IsActive = true;
            LLogger.FrameLog($"{typeof(TViewModel).Name} IsActive: {observableRecipient.IsActive}");
        }
    }

    public override void RemoveViewModel()
    {
        if (viewModel is ObservableRecipient observableRecipient)
        {
            observableRecipient.IsActive = false;
            LLogger.FrameLog($"{typeof(TViewModel).Name} IsActive: {observableRecipient.IsActive}");
        }
        viewModel = null;
    }
}
