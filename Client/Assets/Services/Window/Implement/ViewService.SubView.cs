using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.Messaging;
using Cysharp.Threading.Tasks;
using UnityEngine;
using YooAsset;

public partial class ViewService:
    IRecipient<ViewSubShowAsyncRequestEvent>
{
    private Dictionary<SubViewDisplay, ISubViewCollectContainer> subViewContainers;
    private Dictionary<SubViewType, IViewCheck> subViewChecks;
    private Dictionary<SubViewType, ISubViewConfigure> subViewTypes;
    private Dictionary<SubViewType, Type> sub2MainMaps;

    private void InitSubViews(Dictionary<SubViewDisplay, ISubViewCollectContainer> subViewContainers, List<ISubViewConfigure> subViewConfigures)
    {
        foreach (var pair in subViewContainers)
        {
            SubViewDisplay subViewDisplay = pair.Key;
            ISubViewCollectContainer subViewCollectContainer = pair.Value;
            subViewCollectContainer.AddSubViewContainerType(subViewDisplay);
        }
        this.subViewContainers = subViewContainers;
        this.subViewTypes = new Dictionary<SubViewType, ISubViewConfigure>();
        for (int i = 0; i < subViewConfigures.Count; i++)
        {
            ISubViewConfigure subViewConfigure = subViewConfigures[i];
            List<SubViewType> subViewTypes = subViewConfigure.GetSubViewTypes();
            for (int j = 0; j < subViewTypes?.Count; j++)
            {
                SubViewType subViewType = subViewTypes[j];
                this.subViewTypes.Add(subViewType, subViewConfigure);
            }
        }
    }
    
    private async UniTask<bool> ShowAsync_Internal(SubViewType subViewType)
    {
        if (subViewChecks.TryGetValue(subViewType, out IViewCheck subViewCheck))
        {
            if (subViewCheck != null && !subViewCheck.IsFuncOpenWithTip())
            {
                return false;
            }
            if (!sub2MainMaps.TryGetValue(subViewType, out Type mainType))
            {
                LLogger.FrameError($"子界面：{subViewType} 未绑定主界面");
                return false;
            }
            IViewConfigure viewConfigure = views[mainType];
            IViewCheck viewCheck = viewConfigure.GetViewCheck();
            if (viewCheck != null && !viewCheck.IsFuncOpenWithTip())
            {
                return false;
            }
            IView mainView = await ShowMainAsync_Internal(viewConfigure);
            IViewLocator viewLocator = mainView.GameObject().GetComponent<IViewLocator>();
            viewLocator.SetFirstSubView(subViewType);
            return true;
        }
        return false;
    }

    private bool SwitchAsync_Internal(IView view, SubViewType subViewType)
    {
        if (!sub2MainMaps.TryGetValue(subViewType, out Type mainType))
        {
            LLogger.FrameError($"子界面：{subViewType} 未绑定主界面");
            return false;
        }
        IViewConfigure viewConfigure = views[mainType];
        IViewCheck viewCheck = viewConfigure.GetViewCheck();
        if (viewCheck != null && !viewCheck.IsFuncOpenWithTip())
        {
            return false;
        }
        IViewLocator viewLocator = view.GameObject().GetComponent<IViewLocator>();
        viewLocator.SwitchSubView(subViewType);
        return true;
    }
    
    private async UniTask<IView> AddSubViewAsync_Internal(ISubViewConfigure subViewConfigure)
    {
        Type type = subViewConfigure.GetSubViewType();
        string name = type.Name;
        var handle = YooAssets.LoadAssetAsync<GameObject>(name);
        await handle.Task;
        if (handle.Status != EOperationStatus.Succeed)
        {
            LLogger.FrameError($"未找到该资源 {name}");
            return default;
        }
        GameObject assetObject = handle.AssetObject as GameObject;
        if (assetObject == null)
        {
            LLogger.FrameError($"加载的资源不是 GameObject: {name}");
            return default;
        }
        GameObject instantiatedObject = UnityEngine.Object.Instantiate(assetObject);
        if (instantiatedObject == null)
        {
            LLogger.FrameError($"实例化失败: {name}");
            return default;
        }
        IView view = instantiatedObject.GetComponent(type) as IView;
        return view;
    }
    
    void IRecipient<ViewSubShowAsyncRequestEvent>.Receive(ViewSubShowAsyncRequestEvent message)
    {
        message.Reply(ShowAsync_Internal(message.SubViewType).AsTask());
    }
}
