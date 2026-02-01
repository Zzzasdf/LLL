using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class SubViewUnitLocator: MonoBehaviour, ISubViewLocator
{ 
    protected ViewState viewState;

    private ViewLayer viewLayer;
    private IView view;
    private IViewCheck viewCheck;
    private int uniqueId;
    private CancellationTokenSource cts;

    private Canvas canvas;
    private GraphicRaycaster graphicRaycaster;
    private CanvasGroup canvasGroup;
    private IAnimation EnterAnimation;
    private IAnimation ExitAnimation;

    void ISubViewLocator.Bind(ViewLayer viewLayer, IView view, IViewCheck viewCheck) { }
    void ISubViewLocator.Bind(IView view, IViewCheck viewCheck)
    {
        this.view = view;
        this.viewCheck = viewCheck;
    }

    ViewLayer ISubViewLocator.GetLayer() { throw new System.NotImplementedException(); }

    void ISubViewLocator.BindUniqueId(int uniqueId)
    {
        this.uniqueId = uniqueId;
    }
    int ISubViewLocator.GetUniqueId() => uniqueId;
    GameObject ISubViewLocator.GameObject() => gameObject;
    
    protected virtual void Awake()
    {
        canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        graphicRaycaster = gameObject.AddComponent<GraphicRaycaster>();

        // 添加CanvasGroup用于控制整组UI
        CanvasGroup canvasGroup = gameObject.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        
        InitAnimations();
    }
    void ISubViewLocator.Show()
    {
        Show_Internal().Forget();
    }
    private async UniTask Show_Internal()
    {
        if (cts != null)
        {
            cts.Cancel();
            cts.Dispose();
            cts = null;
            viewState = ViewState.INVISIBLE;
        }
        if (viewState is ViewState.NONE or ViewState.INVISIBLE)
        {
            view.AddViewModel(uniqueId);
            view.InitUI(viewCheck);
            gameObject.SetActive(true);
            viewState = ViewState.VISIBLE;
            
            viewState = ViewState.ENTER_ANIMATION_BEGIN;
            if (EnterAnimation != null)
            {
                await EnterAnimation.DOPlayAsync();
            }
            viewState = ViewState.ENTER_ANIMATION_END;
            view.BindUI();
            viewState = ViewState.ACTIVATED;
        }
    }

    private void OnDestroy()
    {
        if (cts != null)
        {
            cts.Cancel();
            cts.Dispose();
            cts = null;
            viewState = ViewState.INVISIBLE;
        }
    }
    void ISubViewLocator.Hide() => Hide_Internal().Forget();
    private async UniTask Hide_Internal()
    {
        if (viewState is ViewState.ACTIVATED)
        {
            view.UnBindUI();
            view.DestroyUI();
            view.RemoveViewModel();
            viewState = ViewState.PASSIVATED;
            
            viewState = ViewState.EXIT_ANIMATION_BEGIN;
            if (ExitAnimation != null)
            {
                cts = new CancellationTokenSource();
                await ExitAnimation.DOPlayAsync(cts.Token);
            }
            viewState = ViewState.EXIT_ANIMATION_END;
            if (this != null) gameObject.SetActive(false);
            viewState = ViewState.INVISIBLE;
        }
    }

    private void InitAnimations()
    {
        UIAnimation[] animations = gameObject.GetComponents<UIAnimation>();
        if (animations.Length > 0)
        {
            for (int i = 0; i < animations.Length; i++)
            {
                UIAnimation animation = animations[i];
                switch (animation.AnimationType)
                {
                    case UIAnimationType.Enter:
                    {
                        EnterAnimation = animation;
                        break;
                    }
                    case UIAnimationType.Exit:
                    {
                        ExitAnimation = animation;
                        break;
                    }
                }
            }
        }
        else
        {
            // 添加默认进入动画
            {
                AlphaAnimation animation = gameObject.AddComponent<AlphaAnimation>();
                animation.AnimationType = UIAnimationType.Enter;
                animation.from = 0f;
                animation.to = 1f;
                animation.duration = 0.5f;
                EnterAnimation = animation;
            }
            // 添加默认退出动画
            {
                AlphaAnimation animation = gameObject.AddComponent<AlphaAnimation>();
                animation.AnimationType = UIAnimationType.Exit;
                animation.from = 1f;
                animation.to = 0f;
                animation.duration = 0.3f;
                ExitAnimation = animation;
            }
        }
    }
}
