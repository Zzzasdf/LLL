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
    private Dictionary<ViewLayer, ILayerContainer> layerContainers;
    private Dictionary<Type, IViewConfigure> views;
    
    private Dictionary<Type, Type> sub2MainMaps;
    private Dictionary<SubViewAKA, Type> subAka2MainMaps;

    private Dictionary<Type, ISubViewConfigure> subViews;
    private Dictionary<SubViewAKA, ISubViewConfigure> subViewAKAs;
    
    public ViewService(Dictionary<ViewLayer, ILayerContainer> layerContainers, 
        Dictionary<ViewLayer, List<IViewConfigure>> views, 
        IReadOnlyList<(Func<ISubViewContainer> subViewContainer, List<ISubViewConfigure> subViewConfigures)> subViews)
    {
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
                viewConfigure.Build(viewLayer);
                
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

        this.subViews = new Dictionary<Type, ISubViewConfigure>();
        this.subViewAKAs = new Dictionary<SubViewAKA, ISubViewConfigure>();
        for (int i = 0; i < subViews.Count; i++)
        {
            // TODO ZZZ
            LLogger.Warning("!!!!!!!!!!!!!!");
            (Func<ISubViewContainer> subViewContainer, List<ISubViewConfigure> subViewConfigures) = subViews[i];
            for (int j = 0; j < subViewConfigures.Count; j++)
            {
                ISubViewConfigure subViewConfigure = subViewConfigures[j];
                Type subViewType = subViewConfigure.GetSubViewType();
                List<SubViewAKA> subViewAKAs = subViewConfigure.GetSubViewAKAs();

                this.subViews.Add(subViewType, subViewConfigure);
                for (int k = 0; k < subViewAKAs?.Count; k++)
                {
                    SubViewAKA subViewAka = subViewAKAs[k];
                    this.subViewAKAs.Add(subViewAka, subViewConfigure);
                }
            }
        }
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