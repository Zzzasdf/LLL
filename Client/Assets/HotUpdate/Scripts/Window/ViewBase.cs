using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class ViewBase<TViewModel> : MonoBehaviour, IView
    where TViewModel: class, IViewModel
{
    private bool unityEvent;
    private bool viewEvent;
    private ViewState viewState;

    [SerializeField]
    private IViewLocator viewLocator;
    [SerializeField]
    private ViewLayer viewLayer;
    [SerializeField]
    private int uniqueId;
    protected TViewModel viewModel { get; private set; }

    void IView.BindLayer(ViewLayer viewLayer)
    {
        this.viewLayer = viewLayer;
    }
    void IView.BindLocator(IViewLocator viewLocator)
    {
        this.viewLocator = viewLocator;
    }

    ViewLayer IView.GetLayer() => viewLayer;
    ViewState IView.GetViewState() => viewState;

    void IView.BindUniqueId(int uniqueId)
    {
        this.uniqueId = uniqueId;
    }

    int IView.GetUniqueId() => uniqueId;

    GameObject IView.GameObject() => gameObject;
    
    private void Awake()
    {
        unityEvent = true;
        Show_Internal().Forget();
    }
    void IView.Show()
    {
        viewEvent = true;
        Show_Internal().Forget();
    }
    private async UniTask Show_Internal()
    {
        if (!unityEvent || !viewEvent) return;
        if (viewState is ViewState.NONE or ViewState.INVISIBLE)
        {
            viewModel = ViewModelGenerator.Default.GetOrAdd<TViewModel>(uniqueId);
            if (viewModel is ObservableRecipient observableRecipient)
            {
                observableRecipient.IsActive = true;
                LLogger.FrameLog($"{typeof(TViewModel).Name} IsActive: {viewState}");
            }
            InitUI();
            gameObject.SetActive(true);
            viewState = ViewState.VISIBLE;
            
            viewState = ViewState.ENTER_ANIMATION_BEGIN;
            if (viewLocator.EnterAnimation != null)
            {
                if (cts != null)
                {
                    cts.Cancel();
                    cts.Dispose();
                    cts = null;
                }
                await viewLocator.EnterAnimation.DOPlayAsync();
            }
            viewState = ViewState.ENTER_ANIMATION_END;
            BindUI();
            viewState = ViewState.ACTIVATED;
        }
    }

    private CancellationTokenSource cts;
    private void OnDestroy()
    {
        if (cts != null)
        {
            cts.Cancel();
            cts.Dispose();
            cts = null;
        }
        Hide_Internal().Forget();
    }

    void IView.Hide() => Hide_Internal().Forget();

    private async UniTask Hide_Internal()
    {
        if (!unityEvent || !viewEvent || viewModel == null) return;
        if (viewState is ViewState.ACTIVATED)
        {
            UnBindUI();
            DestroyUI();
            if (viewModel is ObservableRecipient observableRecipient)
            {
                observableRecipient.IsActive = false;
                LLogger.FrameLog($"{typeof(TViewModel).Name} IsActive: {viewState}");
            }
            viewModel = null;
            viewState = ViewState.PASSIVATED;
            
            viewState = ViewState.EXIT_ANIMATION_BEGIN;
            if (viewLocator.ExitAnimation != null)
            {
                cts = new CancellationTokenSource();
                await viewLocator.ExitAnimation.DOPlayAsync(cts.Token);
            }
            viewState = ViewState.EXIT_ANIMATION_END;
            gameObject.SetActive(false);
            viewState = ViewState.INVISIBLE;
        }
    }

    protected abstract void InitUI();
    protected abstract void DestroyUI();
    protected abstract void BindUI();
    protected abstract void UnBindUI();
}
