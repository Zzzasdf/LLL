using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.Messaging;
using Cysharp.Threading.Tasks;

public partial class ViewService:
    IRecipient<ViewShowAsyncRequestEvent>,
    IRecipient<ViewHideAsyncRequestEvent>,
    IRecipient<ViewAllHideAsyncRequestEvent>
{
    private async UniTask<bool> ShowAsync_Internal(Type type)
    {
        if (viewShows.TryGetValue(type, out IViewConfigure viewConfigure))
        {
            if (viewConfigure.TryGetViewCheck(out IViewCheck viewCheck))
            {
                if (!viewCheck.IsFuncOpenWithTip())
                {
                    return false;
                }
            }
            if (viewConfigure.TryGetSubViewConfigures(out List<ISubViewConfigure> subViewConfigures))
            {
                for (int i = 0; i < subViewConfigures.Count; i++)
                {
                    ISubViewConfigure subViewConfigure = subViewConfigures[i];
                    if (!subViewConfigure.TryGetViewCheck(out IViewCheck subViewCheck)
                        || subViewCheck.IsFuncOpen())
                    {
                        IView view = await ShowMainAsync_Internal(viewConfigure);
                        AddSubViewLocator(view, viewConfigure);
                        return true;
                    }
                }
            }
            else
            {
                IView view = await ShowMainAsync_Internal(viewConfigure);
                AddSubViewLocator(view, viewConfigure);
            }
        }
        return false;
    }
    
    private async UniTask<IView> ShowMainAsync_Internal(IViewConfigure viewConfigure)
    {
        Type type = viewConfigure.GetViewType();
        ViewLayer viewLayer = viewConfigure.GetViewLayer();
        ILayerContainer layerContainer = layerLocators[viewLayer].GetContainer();
        (IView view, int? removeId) = await layerContainer.ShowViewAndTryRemoveAsync(type);
        
        // 同层界面推入 弹出处理
        if(removeId.HasValue)
        {
            ClearUpperLayerStash(viewLayer, removeId.Value);
        }
        // 上层界面遮挡暂存
        IViewLocator viewLocator = view.GetLocator();
        int uniqueId = viewLocator.GetUniqueId();
        foreach (var item in Enum.GetValues(typeof(ViewLayer)))
        {
            ViewLayer layer = (ViewLayer)item;
            if (layer <= viewLayer) continue;
            ILayerContainer container = layerLocators[layer].GetContainer();
            container.Stash(uniqueId);
        }
        return view;
    }
    
    private async UniTask<bool> HideAsync_Internal(IView view)
    {
        IViewLocator viewLocator = view.GetLocator();
        ViewLayer viewLayer = viewLocator.GetLayer();
        ILayerContainer layerContainer = layerLocators[viewLayer].GetContainer();
        // 清空上层所有激活界面
        foreach (var item in Enum.GetValues(typeof(ViewLayer)))
        {
            ViewLayer layer = (ViewLayer)item;
            if (layer <= viewLayer) continue;
            ILayerContainer container = layerLocators[layer].GetContainer();
            container.HideAllActivateView();
        }
        
        int uniqueId = viewLocator.GetUniqueId();
        List<int> popIds = layerContainer.HideViewTryPop(uniqueId);
        if (popIds is { Count: > 0 })
        {
            await PopShow(viewLayer, popIds);
        }
        foreach (var item in Enum.GetValues(typeof(ViewLayer)))
        {
            ViewLayer layer = (ViewLayer)item;
            if (layer <= viewLayer) continue;
            ILayerContainer container = layerLocators[layer].GetContainer();
            if (!container.TryStashPop(uniqueId, out List<int> ids)) continue;
            await PopShow(layer, ids);
        }
        return true;
    }
    private async UniTask PopShow(ViewLayer viewLayer, List<int> popIds)
    {
        ILayerContainer layerContainer = layerLocators[viewLayer].GetContainer();
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
            ILayerContainer container = layerLocators[layer].GetContainer();
            container.StashClear(uniqueId);
        }
    }
    
    private void HideAll_Internal()
    {
        foreach (var pair in layerLocators)
        {
            ILayerContainer layerContainer = pair.Value.GetContainer();
            layerContainer.HideAllView();
        }
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
        HideAll_Internal();
    }
}