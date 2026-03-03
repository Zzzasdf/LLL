using System.Threading;
using Cysharp.Threading.Tasks;

public partial class ViewDriverBase
{
    protected ViewState viewState { get; private set; }

    private IViewCheck viewCheck;
    private CancellationTokenSource cts;
    
    private IAnimation EnterAnimation;
    private IAnimation ExitAnimation;
    
    private async UniTask Show_Internal()
    {
        if (cts != null)
        {
            cts.Cancel();
            cts.Dispose();
            cts = null;
        }
        if (viewState is ViewState.NONE or ViewState.INVISIBLE)
        {
            view.AddViewModel();
            view.InitUI(viewCheck);
            view.gameObject.SetActive(true);
            viewState = ViewState.VISIBLE;
            
            viewState = ViewState.ENTER_ANIMATION_BEGIN;
            if (EnterAnimation != null)
            {
                cts = new CancellationTokenSource();
                await EnterAnimation.DOPlayAsync(cts.Token);
            }
            viewState = ViewState.ENTER_ANIMATION_END;
            view.BindUI();
            viewState = ViewState.ACTIVATED;
        }
    }

    private void OnDestroy_Internal()
    {
        if (cts != null)
        {
            cts.Cancel();
            cts.Dispose();
            cts = null;
            viewState = ViewState.INVISIBLE;
        }
    }
    private async UniTask Hide_Internal()
    {
        if (cts != null)
        {
            cts.Cancel();
            cts.Dispose();
            cts = null;
        }
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
            if (this != null) view.gameObject.SetActive(false);
            viewState = ViewState.INVISIBLE;
            if (cts is { IsCancellationRequested: false })
            {
                view = null;
            }
        }
    }

    private void InitAnimations()
    {
        UIAnimation[] animations = view.gameObject.GetComponents<UIAnimation>();
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
                AlphaAnimation animation = view.gameObject.AddComponent<AlphaAnimation>();
                animation.AnimationType = UIAnimationType.Enter;
                animation.from = 0f;
                animation.to = 1f;
                animation.duration = 0.3f;
                EnterAnimation = animation;
            }
            // 添加默认退出动画
            {
                AlphaAnimation animation = view.gameObject.AddComponent<AlphaAnimation>();
                animation.AnimationType = UIAnimationType.Exit;
                animation.from = 1f;
                animation.to = 0f;
                animation.duration = 0.3f;
                ExitAnimation = animation;
            }
        }
    }
}
