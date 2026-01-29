using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.Messaging;
using Cysharp.Threading.Tasks;

public partial class ViewService:
    IRecipient<ViewHideAsyncRequestEvent>,
    IRecipient<ViewAllHideAsyncRequestEvent>
{
    private Dictionary<ViewLayer, ILayerContainer> layerContainers;
    private Dictionary<Type, IViewConfigure> views;
    
    private void InitViews(Dictionary<ViewLayer, ILayerContainer> layerContainers, Dictionary<ViewLayer, List<IViewConfigure>> views)
    {
        foreach (var pair in layerContainers)
        {
            ViewLayer viewLayer = pair.Key;
            ILayerContainer layerContainer = pair.Value;
            layerContainer.AddLayer(viewLayer);
        }
        this.layerContainers = layerContainers;
        
        this.views = new Dictionary<Type, IViewConfigure>();
        sub2MainMaps = new Dictionary<Type, Type>();
        subAka2MainMaps = new Dictionary<SubViewAKA, Type>();
        foreach (var pair in views)
        {
            ViewLayer viewLayer = pair.Key;
            List<IViewConfigure> viewConfigures = pair.Value;
            for (int i = 0; i < viewConfigures.Count; i++)
            {
                IViewConfigure viewConfigure = viewConfigures[i];
                viewConfigure.AddLayer(viewLayer);
                
                Type viewType = viewConfigure.GetViewType();
                this.views.Add(viewType, viewConfigure);
                
                List<Type> subViewTypes = viewConfigure.GetSubViewTypes();
                if (subViewTypes != null)
                {
                    for (int j = 0; j < subViewTypes.Count; j++)
                    {
                        Type subViewType = subViewTypes[j];
                        sub2MainMaps.Add(subViewType, viewType);
                    }
                }
                
                List<SubViewAKA> subViewAKAs = viewConfigure.GetSubViewAKAs();
                if (subViewAKAs != null)
                {
                    for (int j = 0; j < subViewAKAs.Count; j++)
                    {
                        SubViewAKA subViewAKA = subViewAKAs[j];
                        subAka2MainMaps.Add(subViewAKA, viewType);
                    }
                }
            }
        }
    }
    
    private async UniTask<IView> ShowMainAsync_Internal(IViewConfigure viewConfigure)
    {
        Type type = viewConfigure.GetViewType();
        ViewLayer viewLayer = viewConfigure.GetViewLayer();
        ILayerContainer layerContainer = layerContainers[viewLayer];
        (IView view, int? removeId) = await layerContainer.ShowViewAndTryRemoveAsync(type);
        
        // 同层界面推入 弹出处理
        if(removeId.HasValue)
        {
            ClearUpperLayerStash(viewLayer, removeId.Value);
        }
        // 上层界面遮挡暂存
        int uniqueId = view.GetUniqueId();
        foreach (var item in Enum.GetValues(typeof(ViewLayer)))
        {
            ViewLayer layer = (ViewLayer)item;
            if (layer <= viewLayer) continue;
            ILayerContainer container = layerContainers[layer];
            container.Stash(uniqueId);
        }
        return view;
    }
    
    private async UniTask<bool> HideAsync_Internal(IView view)
    {
        ViewLayer viewLayer = view.GetLayer();
        ILayerContainer layerContainer = layerContainers[viewLayer];
        // 清空上层所有激活界面
        foreach (var item in Enum.GetValues(typeof(ViewLayer)))
        {
            ViewLayer layer = (ViewLayer)item;
            if (layer <= viewLayer) continue;
            ILayerContainer container = layerContainers[layer];
            container.HideAllActivateView();
        }
        
        int uniqueId = view.GetUniqueId();
        List<int> popIds = layerContainer.HideViewTryPop(uniqueId);
        if (popIds is { Count: > 0 })
        {
            await PopShow(viewLayer, popIds);
        }
        foreach (var item in Enum.GetValues(typeof(ViewLayer)))
        {
            ViewLayer layer = (ViewLayer)item;
            if (layer <= viewLayer) continue;
            ILayerContainer container = layerContainers[layer];
            if (!container.TryStashPop(uniqueId, out List<int> ids)) continue;
            await PopShow(layer, ids);
        }
        return true;
    }
    private async UniTask PopShow(ViewLayer viewLayer, List<int> popIds)
    {
        ILayerContainer layerContainer = layerContainers[viewLayer];
        List<int> removeIds = await layerContainer.PopViewAndTryRemove(popIds);
        if (removeIds == null) return;
        foreach (var removeId in removeIds)
        {
            ClearUpperLayerStash(viewLayer, removeId);
        }
    }
    
    // 清空关联的上层界面遮挡暂存 ！！
    private void ClearUpperLayerStash(ViewLayer viewLayer, int uniqueId)
    {
        foreach (var item in Enum.GetValues(typeof(ViewLayer)))
        {
            ViewLayer layer = (ViewLayer)item;
            if (layer <= viewLayer) continue;
            ILayerContainer container = layerContainers[layer];
            container.StashClear(uniqueId);
        }
    }
    
    private void HideAll_Internal()
    {
        foreach (var pair in layerContainers)
        {
            ILayerContainer layerContainer = pair.Value;
            layerContainer.HideAllView();
        }
    }
    
    void IRecipient<ViewHideAsyncRequestEvent>.Receive(ViewHideAsyncRequestEvent message)
    {
        message.Reply(HideAsync_Internal(message.View).AsTask());
    }
    void IRecipient<ViewAllHideAsyncRequestEvent>.Receive(ViewAllHideAsyncRequestEvent message)
    {
        HideAll_Internal();
    }
}