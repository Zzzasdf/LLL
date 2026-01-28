using CommunityToolkit.Mvvm.Messaging;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

public class ViewMaskTransparentClickHelper : ViewRaycastBlockingHelper
{
    private IView view;
    private Button btnMask;
    
    protected override void Awake()
    {
        base.Awake();

        view = GetComponent<IView>();
        btnMask = goMask.AddComponent<Button>();
        btnMask.image = imgMask;
        btnMask.onClick.AddListener(OnBtnCloseClick);
    }
    private void OnDestroy()
    {
        if (btnMask == null) return;
        btnMask.onClick.RemoveAllListeners();
    }

    private void OnBtnCloseClick() => OnBtnCloseClickAsync().Forget();
    private async UniTask OnBtnCloseClickAsync()
    {
        if (view.GetViewState() != ViewState.ACTIVATED) return;
        await WeakReferenceMessenger.Default.SendViewHideAsync(view);
    }
}
