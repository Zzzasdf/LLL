using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Cysharp.Threading.Tasks;
using UnityEngine;
using YooAsset;

public class ViewService: ObservableRecipient,
    IViewService,
    IRecipient<ViewShowAsyncRequestEvent>,
    IRecipient<ViewHideAsyncRequestEvent>,
    IRecipient<ViewAllHideAsyncRequestEvent>
{
    private Dictionary<Type, ViewLayer> viewLayers;
    private UniqueIdGenerator uniqueIdGenerator;

    private Dictionary<Type, IView> viewDict;
    private Dictionary<int, IView> uniqueViewDict;
    
    private Dictionary<ViewLayer, ILayerContainer> layerContainers;

    public ViewService(Dictionary<ViewLayer, ILayerContainer> layerContainers, Dictionary<ViewLayer, List<Type>> viewLayers)
    {
        this.viewLayers = new Dictionary<Type, ViewLayer>();
        foreach (var pair  in viewLayers)
        {
            ViewLayer viewLayer = pair.Key;
            List<Type> types = pair.Value;
            for (int i = 0; i < types.Count; i++)
            {
                Type type = types[i];
                this.viewLayers.Add(type, viewLayer);
            }
        }
        uniqueIdGenerator = new UniqueIdGenerator();
        
        viewDict = new Dictionary<Type, IView>();
        uniqueViewDict = new Dictionary<int, IView>();
        this.layerContainers = layerContainers;
        foreach (var pair in layerContainers)
        {
            ILayerContainer layerContainer = pair.Value;
            layerContainer.BindGetView(GetView);
        }
        IsActive = true;
        return;

        IView GetView(int uniqueId) => uniqueViewDict[uniqueId];
    }

    Dictionary<ViewLayer, ILayerContainer> IViewService.GetLayerContainers() => layerContainers;
    
    private async UniTask<IView> ShowAsync_Internal(Type type)
    {
        ViewLayer viewLayer = viewLayers[type];
        
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
            if (!layerContainers.TryGetValue(viewLayer, out ILayerContainer container))
            {
                LLogger.FrameError($"未支持的层级容器类型 {viewLayer}");
                return await new UniTask<IView>(default);
            }
            windowRt.SetParent(container.GetLocator().GetRectTransform());
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
        view.BindUniqueId(uniqueId);
        view.RefIncrement();
        uniqueViewDict.Add(uniqueId, view);
        
        // 层级集合
        if (!layerContainers.TryGetValue(viewLayer, out ILayerContainer layerContainer))
        {
            LLogger.FrameError($"未支持的层级容器类型 {viewLayer}");
            return await new UniTask<IView>(default);
        }
        if(layerContainer.AddAndTryOutRemoveId(uniqueId, out int removeId))
        {
            ClearUpperLayerStorage(removeId);
        }

        // 上层界面遮挡暂存
        foreach (var item in Enum.GetValues(typeof(ViewLayer)))
        {
            ViewLayer layer = (ViewLayer)item;
            if (layer <= viewLayer) continue;
            ILayerContainer container = layerContainers[layer];
            container.PushStorage(uniqueId);
        }
        view.Show();
        return view;
    }
    // 清空关联的上层界面遮挡暂存 ！！
    private void ClearUpperLayerStorage(int uniqueId)
    {
        IView view = uniqueViewDict[uniqueId];
        uniqueViewDict.Remove(uniqueId);
        uniqueIdGenerator.DeleteUniqueId(uniqueId);

        Type type = view.GetType();
        view.RefReduction();
        if (view.GetRefCount() == 0)
        {
            viewDict.Remove(type);
            view.Hide();
        }
        
        ViewLayer viewLayer = view.GetLayer();
        foreach (var item in Enum.GetValues(typeof(ViewLayer)))
        {
            ViewLayer layer = (ViewLayer)item;
            if (layer <= viewLayer) continue;
            ILayerContainer container = layerContainers[layer];
            if (!container.TryPopStorage(uniqueId, out Queue<int> storage)) continue;
            foreach (var removeUniqueId in storage)
            {
                ClearUpperLayerStorage(removeUniqueId);
            }
        }
    }

    private UniTask<bool> HideAsync_Internal(IView view)
    {
        int uniqueId = view.GetUniqueId();
        uniqueViewDict.Remove(uniqueId);
        uniqueIdGenerator.DeleteUniqueId(uniqueId);
        
        view.RefReduction();
        if (view.GetRefCount() == 0)
        {
            Type type = view.GetType();
            viewDict.Remove(type);
        }
        view.Hide();
        
        ViewLayer viewLayer = view.GetLayer();
        ILayerContainer layerContainer = layerContainers[viewLayer];
        if (layerContainer.RemoveAndTryPopId(uniqueId, out int popId))
        {
            PopShow(popId);
        }
        
        foreach (var item in Enum.GetValues(typeof(ViewLayer)))
        {
            ViewLayer layer = (ViewLayer)item;
            if (layer <= viewLayer) continue;
            ILayerContainer container = layerContainers[layer];
            if (!container.TryPopStorage(uniqueId, out Queue<int> storage)) continue;
            foreach (var showUniqueId in storage)
            {
                PopShow(showUniqueId);
            }
        }
        return UniTask.FromResult(true);
    }
    private void PopShow(int uniqueId)
    {
        IView view = uniqueViewDict[uniqueId];
        Type type = view.GetType();
        
        // 装配
        ViewLayer viewLayer = viewLayers[type];
        view.BindUniqueId(uniqueId);
        view.Show();
        
        // 层级集合
        if (!layerContainers.TryGetValue(viewLayer, out ILayerContainer layerContainer))
        {
            LLogger.FrameError($"未支持的层级容器类型 {viewLayer}");
            return;
        }
        if(layerContainer.AddAndTryOutRemoveId(uniqueId, out int removeId))
        {
            ClearUpperLayerStorage(removeId);
        }
    }
    
    private async UniTask<bool> HideAllAsync_Internal()
    {
        List<IView> views = uniqueViewDict.Values.ToList();
        foreach (var view in views)
        {
            await HideAsync_Internal(view);
        }
        return await UniTask.FromResult(true);
    }
    
    void IRecipient<ViewShowAsyncRequestEvent>.Receive(ViewShowAsyncRequestEvent message)
    {
        message.Reply(ShowAsync_Internal(message.Type).AsTask());
    }
    void IRecipient<ViewHideAsyncRequestEvent>.Receive(ViewHideAsyncRequestEvent message)
    {
        message.Reply(HideAsync_Internal(message.View).AsTask());
    }
    void IRecipient<ViewAllHideAsyncRequestEvent>.Receive(ViewAllHideAsyncRequestEvent message)
    {
        message.Reply(HideAllAsync_Internal().AsTask());
    }
}