using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using UnityEngine;

public abstract class ViewBase<TViewModel> : MonoBehaviour, IView
    where TViewModel: class, IViewModel
{
    private bool unityEvent;
    private bool viewEvent;
    private bool isShow;
    
    protected TViewModel viewModel { get; private set; }
    [SerializeField] private ViewLayer viewLayer;
    [SerializeField] private int refCount;
    [SerializeField] private int uniqueId;

    void IView.BindLayer(ViewLayer viewLayer)
    {
        this.viewLayer = viewLayer;
        viewModel = Ioc.Default.GetRequiredService<TViewModel>();
    }

    ViewLayer IView.GetLayer() => viewLayer;
    
    void IView.RefIncrement() => refCount++;
    void IView.RefReduction() => refCount--;
    public int GetRefCount() => refCount;

    void IView.BindUniqueId(int uniqueId)
    {
        this.uniqueId = uniqueId;
        IsActive(false);
        IsActive(true);
    }
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
    private void Show_Internal() => IsActive(true);

    private void OnDestroy() => Hide_Internal();
    void IView.Hide() => Hide_Internal();
    private void Hide_Internal() => IsActive(false);
    
    private void IsActive(bool value)
    {
        if (!unityEvent || !viewEvent || viewModel == null) return;
        if (viewModel is ObservableRecipient observableRecipient)
        {
            observableRecipient.IsActive = value;
            LLogger.Log($"{typeof(TViewModel).Name} IsActive: {value}");
        }
        if (isShow != value)
        {
            isShow = value;
            if (value)
            {
                gameObject.SetActive(true);
                transform.SetAsLastSibling();
                BindUI();
            }
            else
            {
                UnBindUI();
                gameObject.SetActive(false);
                if (refCount == 0)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    protected virtual void BindUI()
    {
        
    }
    protected virtual void UnBindUI()
    {
        
    }
}
