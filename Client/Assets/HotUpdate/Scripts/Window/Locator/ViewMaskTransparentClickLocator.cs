using CommunityToolkit.Mvvm.Messaging;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

public class ViewMaskTransparentClickLocator : ViewRaycastBlockingLocator
{
    private IView view;
    private Button btnMask;
    
    private new void Awake()
    {
        base.Awake();

        view = GetComponent<IView>();
        btnMask = goMask.AddComponent<Button>();
        btnMask.image = imgMask;
        btnMask.onClick.AddListener(OnBtnCloseClick);
    }
    private void OnDestroy()
    {
        btnMask.onClick.RemoveAllListeners();
    }

    private void OnBtnCloseClick() => OnBtnCloseClickAsync().Forget();
    private async UniTask OnBtnCloseClickAsync()
    {
        await WeakReferenceMessenger.Default.SendViewHideAsync(view);
    }
}
