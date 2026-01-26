using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Cysharp.Threading.Tasks;

public class ViewService: ObservableRecipient,
    IViewService,
    IRecipient<ViewShowAsyncRequestEvent>,
    IRecipient<ViewHideAsyncRequestEvent>,
    IRecipient<ViewAllHideAsyncRequestEvent>
{
    private Dictionary<Type, ViewLayer> viewLayers;
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
        this.layerContainers = layerContainers;
        IsActive = true;
    }

    Dictionary<ViewLayer, ILayerContainer> IViewService.GetLayerContainers() => layerContainers;
    
    private async UniTask<IView> ShowAsync_Internal(Type type)
    {
        ViewLayer viewLayer = viewLayers[type];
        ILayerContainer layerContainer = layerContainers[viewLayer];
        (IView view, int? removeId) = await layerContainer.ShowViewAndTryRemoveAsync(type);
        
        // 同层界面推入 弹出处理
        if(removeId.HasValue)
        {
            await ClearUpperLayerStash(viewLayer, removeId.Value);
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
        (int? popId, int siblingIndex) = layerContainer.HideViewTryPop(uniqueId);
        if (!popId.HasValue)
        {
            return true;
        }
        await PopShow(viewLayer, popId.Value, siblingIndex);
        foreach (var item in Enum.GetValues(typeof(ViewLayer)))
        {
            ViewLayer layer = (ViewLayer)item;
            if (layer <= viewLayer) continue;
            ILayerContainer container = layerContainers[layer];
            if (!container.TryStashPop(uniqueId, out Queue<int> storage)) continue;
            foreach (var showUniqueId in storage)
            {
                await PopShow(layer, showUniqueId, -1);
            }
        }
        return true;
    }
    private async UniTask PopShow(ViewLayer viewLayer, int uniqueId, int siblingIndex)
    {
        ILayerContainer layerContainer = layerContainers[viewLayer];
        int? removeId = await layerContainer.PopViewAndTryRemove(uniqueId, siblingIndex);
        if (!removeId.HasValue) return;
        await ClearUpperLayerStash(viewLayer, removeId.Value);
    }
    
    // 清空关联的上层界面遮挡暂存 ！！
    private async UniTask ClearUpperLayerStash(ViewLayer viewLayer, int uniqueId)
    {
        ILayerContainer layerContainer = layerContainers[viewLayer];
        foreach (var item in Enum.GetValues(typeof(ViewLayer)))
        {
            ViewLayer layer = (ViewLayer)item;
            if (layer <= viewLayer) continue;
            ILayerContainer container = layerContainers[layer];
            if (!container.TryStashPop(uniqueId, out Queue<int> stashIds)) continue;
            foreach (var stashId in stashIds)
            {
                await ClearUpperLayerStash(layer, stashId);
            }
        }
        (int? popId, int siblingIndex) = layerContainer.HideViewTryPop(uniqueId);
        if (popId.HasValue)
        {
            await PopShow(viewLayer, popId.Value, siblingIndex);
        }
    }
    
    private async UniTask<bool> HideAllAsync_Internal()
    {
        foreach (var pair in layerContainers)
        {
            ILayerContainer layerContainer = pair.Value;
            layerContainer.HideAllView();
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