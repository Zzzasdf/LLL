using UnityEngine;
using YooAsset;

public abstract class AssetBaseLoader<TObject> : MonoBehaviour
    where TObject : Object
{
    private bool unityEvent;
    private string location;

    private AssetLoadState loadState;
    private AssetHandle handle;
    
    private void Awake()
    {
        unityEvent = true;
        if (string.IsNullOrEmpty(location))
        {
            return;
        }
        Load_Internal();
    }
    
    protected void Load(string location)
    {
        if (!unityEvent)
        {
            this.location = location;
            return;
        }
        if (location == this.location)
        {
            return;
        }
        this.location = location;
        Load_Internal();
    }
    private void Load_Internal()
    {
        if (handle != null)
        {
            handle.Completed -= OnCompleted_Internal;
            handle.Release();
        }
        handle = YooAssets.LoadAssetAsync<TObject>(location);
        handle.Completed += OnCompleted_Internal;
        loadState = AssetLoadState.Loading;
    }
    
    private void OnCompleted_Internal(AssetHandle assetHandle)
    {
        if (handle.Status != EOperationStatus.Succeed)
        {
            LLogger.FrameError($"未找到该资源 {name}");
            loadState = AssetLoadState.Completed;
            return;
        }
        TObject assetObject = handle.AssetObject as TObject;
        if (assetObject == null)
        {
            LLogger.FrameError($"加载的资源不是 GameObject: {name}");
            return;
        }
        OnCompleted(assetObject);
        loadState = AssetLoadState.Completed;
    }
    protected abstract void OnCompleted(TObject assetObject);

    private void OnDestroy()
    {
        if (handle != null)
        {
            handle.Completed -= OnCompleted_Internal;
            handle.Release();
        }
    }

    private enum AssetLoadState
    {
        Loading,
        
        Completed
    }
}
