using UnityEngine;
using UnityEngine.UI;

public class ViewUnitHelper: MonoBehaviour, IViewHelper
{
    public IAnimation EnterAnimation { get; set; }
    public IAnimation ExitAnimation { get; set; }
    
    protected Canvas canvas;
    protected GraphicRaycaster graphicRaycaster;

    protected CanvasGroup canvasGroup;
    
    protected virtual void Awake()
    {
        canvas = gameObject.AddComponent<Canvas>();
        graphicRaycaster = gameObject.AddComponent<GraphicRaycaster>();
        
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
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
