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
        IView view = await layerContainer.ShowViewAsync(type);
        int uniqueId = view.GetUniqueId();
        
        // 同层界面推入 弹出处理
        if(layerContainer.PushAndTryPop(uniqueId, out int popId))
        {
            ClearUpperLayerStash(viewLayer, popId);
        }

        // 上层界面遮挡暂存
        foreach (var item in Enum.GetValues(typeof(ViewLayer)))
        {
            ViewLayer layer = (ViewLayer)item;
            if (layer <= viewLayer) continue;
            ILayerContainer container = layerContainers[layer];
            container.StashPush(uniqueId);
        }
        return view;
    }

    private UniTask<bool> HideAsync_Internal(IView view)
    {
        ViewLayer viewLayer = view.GetLayer();
        ILayerContainer layerContainer = layerContainers[viewLayer];
        int uniqueId = view.GetUniqueId();
        layerContainer.HideView(uniqueId);
        
        if (layerContainer.PopAndTryPush(uniqueId, out int pushId))
        {
            PopShow(viewLayer, pushId);
        }
        
        foreach (var item in Enum.GetValues(typeof(ViewLayer)))
        {
            ViewLayer layer = (ViewLayer)item;
            if (layer <= viewLayer) continue;
            ILayerContainer container = layerContainers[layer];
            if (!container.TryStashPop(uniqueId, out Queue<int> storage)) continue;
            foreach (var showUniqueId in storage)
            {
                PopShow(layer, showUniqueId);
            }
        }
        return UniTask.FromResult(true);
    }
    private void PopShow(ViewLayer viewLayer, int uniqueId)
    {
        ILayerContainer layerContainer = layerContainers[viewLayer];
        if (!layerContainer.TryPopView(uniqueId))
        {
            return;
        }
        if (layerContainer.PushAndTryPop(uniqueId, out int popId))
        {
            ClearUpperLayerStash(viewLayer, popId);
        }
    }
    
    // 清空关联的上层界面遮挡暂存 ！！
    private void ClearUpperLayerStash(ViewLayer viewLayer, int uniqueId)
    {
        ILayerContainer layerContainer = layerContainers[viewLayer];
        layerContainer.HideView(uniqueId);
        
        foreach (var item in Enum.GetValues(typeof(ViewLayer)))
        {
            ViewLayer layer = (ViewLayer)item;
            if (layer <= viewLayer) continue;
            ILayerContainer container = layerContainers[layer];
            if (!container.TryStashPop(uniqueId, out Queue<int> popIds)) continue;
            foreach (var popId in popIds)
            {
                ClearUpperLayerStash(layer, popId);
            }
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