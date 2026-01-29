using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Cysharp.Threading.Tasks;

public partial class ViewService: ObservableRecipient,
    IViewService,
    IRecipient<ViewShowAsyncRequestEvent>,
    IRecipient<ViewAKAShowAsyncRequestEvent>
{
    private Dictionary<Type, Type> sub2MainMaps;
    private Dictionary<SubViewAKA, Type> subAka2MainMaps;
    
    public ViewService(Dictionary<ViewLayer, ILayerContainer> layerContainers, Dictionary<ViewLayer, List<IViewConfigure>> views, 
        Dictionary<SubViewContainerType, ISubViewContainer> subViewContainers, Dictionary<SubViewContainerType, List<ISubViewConfigure>> subViews)
    {
        InitViews(layerContainers, views);
        InitSubViews(subViewContainers, subViews);
        IsActive = true;
    }
    
    Dictionary<ViewLayer, ILayerContainer> IViewService.GetLayerContainers() => layerContainers;

    private async UniTask<bool> ShowAsync_Internal(Type type)
    {
        if (views.TryGetValue(type, out IViewConfigure viewConfigure))
        {
            if (!viewConfigure.IsFuncOpenWithTip())
            {
                return false;
            }
            await ShowMainAsync_Internal(viewConfigure);
            return true;
        }
        if (subViews.TryGetValue(type, out ISubViewConfigure subViewConfigure))
        {
            if (!subViewConfigure.IsFuncOpenWithTip())
            {
                return false;
            }
            if (!sub2MainMaps.TryGetValue(type, out Type mainType))
            {
                LLogger.FrameError($"子界面：{type.Name} 未绑定主界面");
                return false;
            }
            viewConfigure = views[mainType];
            if (!viewConfigure.IsFuncOpenWithTip())
            {
                return false;
            }
            IView mainView = await ShowMainAsync_Internal(viewConfigure);
            mainView.SetFirstSubView(type);
            return true;
        }
        return false;
    }
    private async UniTask<bool> ShowAsync_Internal(SubViewAKA subViewAka)
    {
        if (subViewAKAs.TryGetValue(subViewAka, out ISubViewConfigure subViewConfigure))
        {
            if (!subViewConfigure.IsFuncOpenWithTip())
            {
                return false;
            }
            if (!subAka2MainMaps.TryGetValue(subViewAka, out Type mainType))
            {
                LLogger.FrameError($"子界面 AKA：{subAka2MainMaps} 未绑定主界面");
                return false;
            }
            IViewConfigure viewConfigure = views[mainType];
            if (!viewConfigure.IsFuncOpenWithTip())
            {
                return false;
            }
            IView mainView = await ShowMainAsync_Internal(viewConfigure);
            mainView.SetFirstSubView(subViewAka);
            return true;
        }
        return false;
    }
    
    void IRecipient<ViewShowAsyncRequestEvent>.Receive(ViewShowAsyncRequestEvent message)
    {
        message.Reply(ShowAsync_Internal(message.Type).AsTask());
    }
    void IRecipient<ViewAKAShowAsyncRequestEvent>.Receive(ViewAKAShowAsyncRequestEvent message)
    {
        message.Reply(ShowAsync_Internal(message.SubViewAKA).AsTask());
    }
}