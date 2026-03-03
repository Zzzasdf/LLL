using CommunityToolkit.Mvvm.ComponentModel;
using UnityEngine;

public abstract class ViewEntityBase : EntityBase<IView>, IView
{
    public abstract RectTransform RtChildNodeParent { get; }
    public abstract void AddViewModel();
    public abstract void RemoveViewModel();
    public abstract void InitUI(IViewCheck viewCheck);
    public abstract void DestroyUI();
    public abstract void BindUI();
    public abstract void UnBindUI();
}

public abstract class ViewEntityBase<TViewModel>: ViewEntityBase
    where TViewModel: class, IViewModel
{
    private RectTransform rtChildNodeParent;
    public override RectTransform RtChildNodeParent
    {
        get
        {
            if (rtChildNodeParent == null)
            {
                IChildNodeParent childNodeParent = GetComponent<IChildNodeParent>();
                if (childNodeParent != null)
                {
                    rtChildNodeParent = childNodeParent.GetChildNodeParent();
                }
                else
                {
                    rtChildNodeParent = GetComponent<RectTransform>();
                }
            }
            return rtChildNodeParent;
        }
    }
    
    
    protected TViewModel viewModel { get; private set; }

    public override void AddViewModel()
    {
        viewModel = ViewModelGenerator.Default.GetOrAdd<TViewModel>(UniqueId);
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
        ViewModelGenerator.Default.Delete(UniqueId);
        viewModel = null;
    }
}
