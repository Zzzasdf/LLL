using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using YooAsset;

public abstract class SingleLayerContainerBase : ILayerContainer
{
    private readonly ViewLayer viewLayer;
    private readonly UniqueIdGenerator uniqueIdGenerator;

    private readonly Dictionary<Type, IView> viewDict;
    protected readonly Dictionary<int, IView> uniqueViewDict;

    private ILayerLocator layerLocator;
    
    protected SingleLayerContainerBase()
    {
        uniqueIdGenerator = new UniqueIdGenerator();
        viewDict = new Dictionary<Type, IView>();
        uniqueViewDict = new Dictionary<int, IView>();
    }

    void ILayerContainer.BindLocator(ILayerLocator layerLocator) => this.layerLocator = layerLocator;

    async UniTask<IView> ILayerContainer.ShowViewAsync(Type type)
    {
        // 资源获取
        if (!viewDict.TryGetValue(type, out IView view))
        {
            string name = type.Name;
            var handle = YooAssets.LoadAssetAsync<GameObject>(name);
            await handle.Task;
            
            if (handle.Status != EOperationStatus.Succeed)
            {
                LLogger.FrameError($"未找到该资源 {name}");
                return await new UniTask<IView>(default);
            }
            GameObject assetObject = handle.AssetObject as GameObject;
            if (assetObject == null)
            {
                LLogger.FrameError($"加载的资源不是 GameObject: {name}");
                return await new UniTask<IView>(default);
            }
            GameObject instantiatedObject = UnityEngine.Object.Instantiate(assetObject);
            if (instantiatedObject == null)
            {
                LLogger.FrameError($"实例化失败: {name}");
                return await new UniTask<IView>(default);
            }
            view = instantiatedObject.GetComponent<IView>();
            view.BindLayer(viewLayer);  
            RectTransform windowRt = instantiatedObject.GetComponent<RectTransform>();
            windowRt.SetParent(layerLocator.GetRectTransform());
            windowRt.localPosition = Vector3.zero;
            windowRt.localScale = Vector3.one;
            windowRt.anchoredPosition = Vector2.zero;
            windowRt.anchorMin = Vector2.zero;
            windowRt.anchorMax = Vector2.one;
            windowRt.offsetMin = Vector2.zero;
            windowRt.offsetMax = Vector2.zero;
            viewDict.Add(type, view);
        }
        // 装配
        int uniqueId = uniqueIdGenerator.CreateUniqueId();
        uniqueViewDict.Add(uniqueId, view);
        view.BindUniqueId(uniqueId);
        view.RefIncrement();
        view.Show();
        return view;
    }

    public void HideView(int uniqueId)
    {
        IView view = uniqueViewDict[uniqueId];
        view.RefReduction();
        if (view.GetRefCount() == 0)
        {
            Type type = view.GetType();
            viewDict.Remove(type);
        }
        uniqueViewDict.Remove(uniqueId);
        uniqueIdGenerator.DeleteUniqueId(uniqueId);
        view.Hide();
    }

    public abstract void HideAllView();

    bool ILayerContainer.TryPopView(int uniqueId)
    {
        if (!uniqueViewDict.TryGetValue(uniqueId, out IView view))
        {
            return false;
        }
        view.BindUniqueId(uniqueId);
        view.Show();
        return true;
    }

    public abstract bool PushAndTryPop(int uniqueId, out int popId);
    public abstract bool PopAndTryPush(int uniqueId, out int pushId);

    public abstract void StashPush(int uniqueId);
    public abstract bool TryStashPop(int uniqueId, out Queue<int> popIds);
}
