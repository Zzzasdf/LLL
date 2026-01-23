using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using YooAsset;

public class LoadingView : ViewBase<LoadingViewModel>
{
    [SerializeField] private Slider sliderProgress;

    // private void LoadSceneAsync(string location, LocalPhysicsMode physicsMode = LocalPhysicsMode.None, bool suspendLoad = false, uint priority = 100)
    // {
    //     YooAssets.LoadSceneAsync(location, LoadSceneMode.Single)
    // }
}
